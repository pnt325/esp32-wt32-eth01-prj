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

            vmMainPage.WifiSsid = "test_wifi";
            vmMainPage.WifiPassword = "test_password";
            vmMainPage.WorkHour1 = "0";
            vmMainPage.WorkHour2 = "0";
            vmMainPage.WorkHour3 = "0";
            vmMainPage.NtcTempOffset1 = "0";
            vmMainPage.NtcTempOffset2 = "0";
            vmMainPage.NtcTempOffset3 = "0";
            vmMainPage.TempLimit1 = "95";
            vmMainPage.TempLimit2 = "95";
            vmMainPage.TempLimit3 = "95";
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
                case Protocol.Command.BLE_CMD_TEMP_OFFSET:
                    {
                        float[] toffset = new float[3];
                        for (int i = 0; i < 3; i++)
                        {
                            toffset[i] = BitConverter.ToSingle(packet.Data, i * 4);
                        }
                        vmMainPage.NtcTempOffset1 = toffset[0].ToString();
                        vmMainPage.NtcTempOffset2 = toffset[1].ToString();
                        vmMainPage.NtcTempOffset3 = toffset[2].ToString();
                    }
                    break;
                case Protocol.Command.BLE_CMD_TEMP_LIMIT:
                    {
                        vmMainPage.TempLimit1 = packet.Data[0].ToString();
                        vmMainPage.TempLimit2 = packet.Data[1].ToString();
                        vmMainPage.TempLimit3 = packet.Data[2].ToString();
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
                case Protocol.Command.BLE_CMD_DEVICE_TOKEN_1:
                    vmMainPage.DeviceToken1 = Encoding.UTF8.GetString(packet.Data);
                    break;
                case Protocol.Command.BLE_CMD_DEVICE_TOKEN_2:
                    vmMainPage.DeviceToken2 = Encoding.UTF8.GetString(packet.Data);
                    break;
                case Protocol.Command.BLE_CMD_DEVICE_TOKEN_3:
                    vmMainPage.DeviceToken3 = Encoding.UTF8.GetString(packet.Data);
                    break;
                case Protocol.Command.BLE_CMD_DEVICE_ENABLE:
                    vmMainPage.DeviceEnable1 = packet.Data[0] != 0;
                    vmMainPage.DeviceEnable2 = packet.Data[1] != 0;
                    vmMainPage.DeviceEnable3 = packet.Data[2] != 0;
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
            float[] tempOffset = new float[3];
            byte[] tempLimit = new byte[3];

            try
            {
                
                tempOffset[0] = float.Parse(vmMainPage.NtcTempOffset1);
                tempOffset[1] = float.Parse(vmMainPage.NtcTempOffset2);
                tempOffset[2] = float.Parse(vmMainPage.NtcTempOffset3);
                tempLimit[0] = byte.Parse(vmMainPage.TempLimit1);
                tempLimit[1] = byte.Parse(vmMainPage.TempLimit2);
                tempLimit[2] = byte.Parse(vmMainPage.TempLimit3);
            }
            catch (Exception)
            {
                showItoast("Temperature configure invalid");
                return;
            }

            for (int i = 0; i < tempOffset.Length; i++)
            { 
                if(tempOffset[i] < -5 || tempOffset[i] > 5)
                {
                    showItoast("Temperature offset invalid");
                    return;
                }
            }

            // Device token
            if(vmMainPage.DeviceEnable1  && vmMainPage.DeviceToken1.Length < 10)
            {
                showItoast("Device token invalid");
                return;
            }

            if (vmMainPage.DeviceEnable2 && vmMainPage.DeviceToken2.Length < 10)
            {
                showItoast("Device token invalid");
                return;
            }

            if (vmMainPage.DeviceEnable3 && vmMainPage.DeviceToken3.Length < 10)
            {
                showItoast("Device token invalid");
                return;
            }

            //! start write data
            vmMainPage.ShowProcess = true;
            writeConfigureProcess(work_hours, tempOffset, tempLimit);
        }

        //EventWaitHandle eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        Protocol.Command waitCmd = Protocol.Command.BLE_CMD_ACK;
        bool onWriteConfigure = false;
        private void writeConfigureProcess(float[] work_hour, float[] tempOffset, byte[] tempLimit)
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

                    // Work hour
                    UInt32[] sec = new UInt32[3];
                    sec[0] = (UInt32)(float.Parse(vmMainPage.WorkHour1) * 3600);
                    sec[1] = (UInt32)(float.Parse(vmMainPage.WorkHour2) * 3600);
                    sec[2] = (UInt32)(float.Parse(vmMainPage.WorkHour3) * 3600);

                    byte[] work_data = new byte[4 * 3];
                    for (int i = 0; i < 3; i++)
                    {
                        var wh = BitConverter.GetBytes(sec[i]);
                        work_data[i * 4 + 0] = wh[0];
                        work_data[i * 4 + 1] = wh[1];
                        work_data[i * 4 + 2] = wh[2];
                        work_data[i * 4 + 3] = wh[3];
                    }

                    if (sendConfig(Protocol.Command.BLE_CMD_WORK_HOUR, work_data, work_data.Length) == false)
                    {
                        break;
                    }

                    // Temp offset
                    byte[] toffset_data = new byte[12];
                    int index = 0;

                    byte[] tmp = BitConverter.GetBytes(tempOffset[0]);
                    toffset_data[index++] = tmp[0];
                    toffset_data[index++] = tmp[1];
                    toffset_data[index++] = tmp[2];
                    toffset_data[index++] = tmp[3];
                    tmp = BitConverter.GetBytes(tempOffset[1]);
                    toffset_data[index++] = tmp[0];
                    toffset_data[index++] = tmp[1];
                    toffset_data[index++] = tmp[2];
                    toffset_data[index++] = tmp[3];
                    tmp = BitConverter.GetBytes(tempOffset[2]);
                    toffset_data[index++] = tmp[0];
                    toffset_data[index++] = tmp[1];
                    toffset_data[index++] = tmp[2];
                    toffset_data[index++] = tmp[3];

                    if (sendConfig(Protocol.Command.BLE_CMD_TEMP_OFFSET, toffset_data, toffset_data.Length) == false)
                    {
                        break;
                    }

                    // Temp limit
                    if (sendConfig(Protocol.Command.BLE_CMD_TEMP_LIMIT, tempLimit, tempLimit.Length) == false)
                    {
                        break;
                    }

                    // Device enable
                    byte[] dev_en = new byte[3];
                    const byte en_ = 1;
                    const byte dis_ = 0;
                    dev_en[0] = vmMainPage.DeviceEnable1 ? en_ : dis_;
                    dev_en[1] = vmMainPage.DeviceEnable2 ? en_ : dis_;
                    dev_en[2] = vmMainPage.DeviceEnable3 ? en_ : dis_;
                    if (sendConfig(Protocol.Command.BLE_CMD_DEVICE_ENABLE, dev_en, dev_en.Length) == false)
                    {
                        break;
                    }

                    // Device token
                    byte[] token;
                    if (vmMainPage.DeviceEnable1)
                    {
                        token = Encoding.UTF8.GetBytes(vmMainPage.DeviceToken1);
                        if (sendConfig(Protocol.Command.BLE_CMD_DEVICE_TOKEN_1, token, token.Length) == false)
                        {
                            break;
                        }
                    }

                    if (vmMainPage.DeviceEnable2)
                    {
                        token = Encoding.UTF8.GetBytes(vmMainPage.DeviceToken2);
                        if (sendConfig(Protocol.Command.BLE_CMD_DEVICE_TOKEN_2, token, token.Length) == false)
                        {
                            break;
                        }
                    }

                    if (vmMainPage.DeviceEnable3)
                    {
                        token = Encoding.UTF8.GetBytes(vmMainPage.DeviceToken3);
                        if (sendConfig(Protocol.Command.BLE_CMD_DEVICE_TOKEN_3, token, token.Length) == false)
                        {
                            break;
                        }
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


        private void Button_Clicked(object sender, EventArgs e)
        {
        }
    }
}