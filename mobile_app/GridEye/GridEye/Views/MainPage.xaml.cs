using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Java.IO;
using System.IO;

namespace GridEye.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        const int CONNECTION_WIFI = 1;
        const int CONNECTION_ETH = 2;

        bool tempOffsetPreview = false;
        float tempOffsetVal = 0;
        public MainPage()
        {
            InitializeComponent();
            this.Appearing += MainPage_Appearing;
            this.Disappearing += MainPage_Disappearing;

            vmMainPage.WifiSsid = "wifi ssid";
            vmMainPage.MqttHost = "192.168.0.1";
            vmMainPage.MqttPort = "8883";
            vmMainPage.WorkHour1 = "0";
            vmMainPage.WorkHour2 = "0";
            vmMainPage.WorkHour3 = "0";
            vmMainPage.NtcTempOffset = "0";
            vmMainPage.TempLimit = "95";
            vmMainPage.CaData = "";
        }

        private void MainPage_Disappearing(object sender, EventArgs e)
        {
            App.BleProtocol.Received -= BleProtocol_Received;
        }

        private void BleProtocol_Received(object sender, Protocol.Packet packet)
        {
            // TODO handle data recieved
            switch (packet.Cmd)
            {
                case Protocol.Command.BLE_CMD_NONE:
                    break;
                case Protocol.Command.BLE_CMD_ACK:
                    if(onWriteConfigure)
                    {
                        waitCmd = Protocol.Command.BLE_CMD_ACK;
                    }
                    break;
                case Protocol.Command.BLE_CMD_NACK:
                    if(onWriteConfigure)
                    {
                        waitCmd = Protocol.Command.BLE_CMD_NACK;
                    }
                    break;
                case Protocol.Command.BLE_CMD_CONFIG_COMMIT:
                    break;
                case Protocol.Command.BLE_CMD_WIFI_SSID:
                    break;
                case Protocol.Command.BLE_CMD_WIFI_PASSWORD:
                    break;
                case Protocol.Command.BLE_CMD_MQTT_PORT:
                    break;
                case Protocol.Command.BLE_CMD_MQTT_HOST:
                    break;
                case Protocol.Command.BLE_CMD_MQTT_CA_BEGIN:
                    break;
                case Protocol.Command.BLE_CMD_MQTT_CA_DATA:
                    break;
                case Protocol.Command.BLE_CMD_MQTT_CA_END:
                    break;
                case Protocol.Command.BLE_CMD_TEMP_OFFSET:
                    {
                        float toffset = BitConverter.ToSingle(packet.Data, 0);
                        vmMainPage.NtcTempOffset = toffset.ToString();
                    }
                    break;
                case Protocol.Command.BLE_CMD_TEMP_LIMIT:
                    {
                        vmMainPage.TempLimit = packet.Data[0].ToString();
                    }
                    break;
                case Protocol.Command.BLE_CMD_CONNECTION:
                    break;
                case Protocol.Command.BLE_CMD_PROBE_TEMP:
                    {
                        Protocol.Data.Temperature temp = new Protocol.Data.Temperature();
                        if(temp.Parse(packet.Data, packet.Length))
                        {
                            if(tempOffsetPreview)
                            {
                                temp.Values[0] = temp.Values[0] + tempOffsetVal;
                                temp.Values[1] = temp.Values[1] + tempOffsetVal;
                                temp.Values[2] = temp.Values[2] + tempOffsetVal;
                            }

                            vmMainPage.NtcTemp1 = temp.Values[0].ToString();
                            vmMainPage.NtcTemp2 = temp.Values[1].ToString();
                            vmMainPage.NtcTemp3 = temp.Values[2].ToString();
                        }
                    }
                    break;
                case Protocol.Command.BLE_CMD_PROBE_DI:
                    {
                        Protocol.Data.Digital di = new Protocol.Data.Digital();
                        if (di.Parse(packet.Data, packet.Length))
                        {
                            vmMainPage.DiInput1 = (di.Values[0] == false) ? "Active" : "No-Active";
                            vmMainPage.DiInput2 = (di.Values[1] == false) ? "Active" : "No-Active";
                            vmMainPage.DiInput3 = (di.Values[2] == false) ? "Active" : "No-Active";
                        }
                    }
                    break;
                case Protocol.Command.BLE_CMD_DEVICE_ID:
                    {
                        vmMainPage.DeviceId = Encoding.UTF8.GetString(packet.Data);
                    }
                    break;
                case Protocol.Command.BLE_CMD_WORK_HOUR:
                    {
                        uint[] work_hours = new uint[3];
                        for (int i = 0; i < 3; i++)
                        {
                            work_hours[i] = BitConverter.ToUInt32(packet.Data, i * 4);
                        }

                        float[] wh_single = new float[3];
                        for (int i = 0; i < 3; i++)
                        {
                            wh_single[i] = (float)work_hours[i] / 3600.0f;
                        }

                        vmMainPage.WorkHour1 = wh_single[0].ToString();
                        vmMainPage.WorkHour2 = wh_single[1].ToString();
                        vmMainPage.WorkHour3 = wh_single[2].ToString();
                    }
                    break;
                default:
                    break;
            }
        }

        private void MainPage_Appearing(object sender, EventArgs e)
        {
            App.BleProtocol.Received += BleProtocol_Received;
            App.BleProtocol.Send(Protocol.Command.BLE_CMD_DEVICE_ID, null, 0);
        }

        protected override bool OnBackButtonPressed()
        {
            BackToInit.Verify();
            return true;
        }

        int conneciton = CONNECTION_WIFI;    // default
        private void radio_wifi_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            RadioButton rbtn = sender as RadioButton;
            if(rbtn == radio_wifi && rbtn.IsChecked)
            {
                conneciton = CONNECTION_WIFI;
                Debug.WriteLine("Set conection: WIFI");
            }
            if (rbtn == radio_eth && rbtn.IsChecked)
            {
                Debug.WriteLine("Set conection: ETH");
                conneciton = CONNECTION_ETH;
            }
        }

        private void btnTempOffset_Clicked(object sender, EventArgs e)
        {
            tempOffsetPreview = true;
            try
            {
                float offset = float.Parse(vmMainPage.NtcTempOffset);
                tempOffsetVal = offset;
            }
            catch
            {
                tempOffsetPreview = false;
                var toast = DependencyService.Get<Services.IToast>();
                toast.Show("Temperature offset invalid");
            }
        }

        private void btnWriteConfig_Clicked(object sender, EventArgs e)
        {
            //! check connection
            if(conneciton == 1)
            {
                if(string.IsNullOrEmpty(vmMainPage.WifiSsid) || string.IsNullOrEmpty(vmMainPage.WifiPassword))
                {
                    showItoast("WIFI configure invalid");
                    return;
                }
            }

            //! check MQTT
            if(string.IsNullOrEmpty(vmMainPage.MqttHost) || string.IsNullOrEmpty(vmMainPage.MqttPort) ||
                string.IsNullOrEmpty(vmMainPage.CaData))
            {
                showItoast("MQTT configure invalid");
                return;
            }

            //! Work hour
            float[] work_hours = new float[3];
            try
            {
                work_hours[0] = float.Parse(vmMainPage.WorkHour1);
                work_hours[1] = float.Parse(vmMainPage.WorkHour2);
                work_hours[2] = float.Parse(vmMainPage.WorkHour3);
            }
            catch (Exception)
            {
                showItoast("Work hour data invalid");
                return;
            }

            // Temperature
            float tempOffset;
            float tempLimit;

            try
            {
                tempOffset = float.Parse(vmMainPage.NtcTempOffset);
                tempLimit = float.Parse(vmMainPage.TempLimit); 
            }
            catch (Exception)
            {
                showItoast("Tmeperature configure invlaid");
                return;
            }

            //! start write data
            vmMainPage.ShowProcess = true;
            writeConfigureProcess(work_hours, tempOffset, tempLimit);
        }

        //EventWaitHandle eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        Protocol.Command waitCmd = Protocol.Command.BLE_CMD_ACK;
        bool onWriteConfigure = false;
        private void writeConfigureProcess(float[] work_hour, float tempOffet, float tempLimit)
        {
            Task.Run(() =>
            {
                //onWriteConfigure = true;
                while (true)
                {
                    // Write connection
                    if (sendConfig(Protocol.Command.BLE_CMD_CONNECTION, new byte[] { (byte)conneciton }, 1) == false)
                    {
                        break;
                    }

                    // Write wifi connection
                    if (conneciton == CONNECTION_WIFI)
                    {
                        // SSID
                        byte[] ssid = Encoding.UTF8.GetBytes(vmMainPage.WifiSsid);
                        if (sendConfig(Protocol.Command.BLE_CMD_WIFI_SSID, ssid, ssid.Length) == false)
                        {
                            break;
                        }

                        // Password
                        byte[] pswd = Encoding.UTF8.GetBytes(vmMainPage.WifiPassword);
                        if (sendConfig(Protocol.Command.BLE_CMD_WIFI_PASSWORD, pswd, pswd.Length) == false)
                        {
                            break;
                        }
                    }

                    // MQTT host, uri
                    string uri = $"mqtts://{vmMainPage.MqttHost}:{vmMainPage.MqttPort}";
                    var uri_data = Encoding.UTF8.GetBytes(uri);
                    if (sendConfig(Protocol.Command.BLE_CMD_MQTT_HOST, uri_data, uri.Length) == false)
                    {
                        break;
                    }

                    // MQTT CA data
                    if (sendConfig(Protocol.Command.BLE_CMD_MQTT_CA_BEGIN, null, 0) == false)
                    {
                        break;
                    }

                    byte[] ca_data = Encoding.UTF8.GetBytes(vmMainPage.CaData);
                    int index = 0;
                    bool err = false;

                    byte[] send_buf = new byte[Protocol.Protocol.DATA_SIZE_MAX];
                    while (true)
                    {
                        int len = ca_data.Length - index;
                        if (len > Protocol.Protocol.DATA_SIZE_MAX)
                        {
                            len = Protocol.Protocol.DATA_SIZE_MAX;
                        }

                        if (len == 0)
                        {
                            break;
                        }

                        for (int i = 0; i < len; i++)
                        {
                            send_buf[i] = ca_data[index++];
                        }

                        if (sendConfig(Protocol.Command.BLE_CMD_MQTT_CA_DATA, send_buf, len) == false)
                        {
                            err = true;
                            break;
                        }

                        if (len < Protocol.Protocol.DATA_SIZE_MAX)
                        {
                            break;
                        }
                    }

                    if (err)
                    {
                        break;
                    }

                    if (sendConfig(Protocol.Command.BLE_CMD_MQTT_CA_END, null, 0) == false)
                    {
                        break;
                    }

                    // Work hour
                    UInt32[] sec = new UInt32[3];
                    sec[0] = (UInt32)(float.Parse(vmMainPage.WorkHour1) * 3600);
                    sec[1] = (UInt32)(float.Parse(vmMainPage.WorkHour2) * 3600);
                    sec[2] = (UInt32)(float.Parse(vmMainPage.WorkHour3) * 3600);

                    byte[] work_data = new byte[4 * 3];
                    for (int i = 0; i < 4; i += 4)
                    {
                        var tmp = BitConverter.GetBytes(sec[i]);
                        work_data[i + 0] = tmp[0];
                        work_data[i + 1] = tmp[1];
                        work_data[i + 2] = tmp[2];
                        work_data[i + 3] = tmp[3];
                    }

                    if (sendConfig(Protocol.Command.BLE_CMD_WORK_HOUR, work_data, work_data.Length) == false)
                    {
                        break;
                    }

                    // Temp offset
                    byte[] toffset_data = BitConverter.GetBytes(float.Parse(vmMainPage.NtcTempOffset));
                    if (sendConfig(Protocol.Command.BLE_CMD_TEMP_OFFSET, toffset_data, toffset_data.Length) == false)
                    {
                        break;
                    }

                    // Temp limit
                    byte tlimit = byte.Parse(vmMainPage.TempLimit);
                    if (sendConfig(Protocol.Command.BLE_CMD_TEMP_LIMIT, new byte[] {tlimit}, 1) == false)
                    {
                        break;
                    }

                    // Write configure confirm
                    if (sendConfig(Protocol.Command.BLE_CMD_CONFIG_COMMIT, null, 0) == false)
                    {
                        break;
                    }

                    vmMainPage.ShowProcess = false;
                    showItoastSafe("Write Configure success");
                    break;
            }

                onWriteConfigure = false;
            });
        }

        private bool sendConfig(Protocol.Command cmd, byte[] data, int len)
        {
            onWriteConfigure = true;
            if(App.BleProtocol.Send(cmd, data, len) == false)
            {
                showItoastSafe("BLE send error");
                vmMainPage.ShowProcess = false;
                onWriteConfigure = false;
                return false;
            }

            waitCmd = Protocol.Command.BLE_CMD_NONE;

            int time_out = 0;
            while(true)
            {
                if(waitCmd != Protocol.Command.BLE_CMD_NONE)
                {
                    break;
                }
                Thread.Sleep(10);
                time_out += 10;

                if(time_out >= 1000)
                {
                    Debug.WriteLine("Config write timeout");
                    vmMainPage.ShowProcess = false;
                    showItoastSafe($"Write {cmd} timeout");
                    return false;
                }
            }

            if (waitCmd == Protocol.Command.BLE_CMD_NACK)
            {
                onWriteConfigure = false;
                vmMainPage.ShowProcess = false;
                showItoastSafe($"Write {cmd} failure");
                return false;
            }

            Debug.WriteLine($"Config CMD: {cmd}");
            return true;
        }

        private void showItoast(string msg)
        {
            Debug.WriteLine($"Toast message: {msg}");
            var toast = DependencyService.Get<Services.IToast>();
            if(toast != null)
            {
                toast.Show(msg);
            }
        }

        private void showItoastSafe(string msg)
        {
            Dispatcher.BeginInvokeOnMainThread(() =>
            {
                showItoast(msg);
            });
        }


        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                var filePicker = await FilePicker.PickAsync();
                if(filePicker == null)
                {
                    showItoast("Choose file failure");
                    return;
                }

                if (filePicker.FileName.EndsWith("crt") == false)
                {
                    showItoast("File type invalid");
                    return;
                }

                var stream = await filePicker.OpenReadAsync();
                if (stream == null)
                {
                    showItoast("Open file failure");
                    return;
                }

                using (var reader = new StreamReader(stream))
                {
                    vmMainPage.CaData = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                showItoast("Open file error");
            }
        }
    }
}