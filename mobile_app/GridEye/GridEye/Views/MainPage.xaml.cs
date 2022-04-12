using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
            vmMainPage.CaData = "-----BEGIN CERTIFICATE-----" +
                                "MIIEBTCCAu2gAwIBAgIUA8HYfkSeRx2l6i3Lhj1fKYRXIbowDQYJKoZIhvcNAQEL" +
                                "BQAwgZExCzAJBgNVBAYTAlZOMRQwEgYDVQQIDAtIbyBDaGkgTWluaDEQMA4GA1UE" +
                                "BwwHVGh1IER1YzEPMA0GA1UECgwGQ0EgUG50MQ0wCwYDVQQLDARUZXN0MRYwFAYD" +
                                "VQQDDA0xOTIuMTY4LjAuMTA1MSIwIAYJKoZIhvcNAQkBFhNwaGF0Lm50QGhvdG1h" +
                                "aWwuY29tMB4XDTIyMDQwOTE3MTMxN1oXDTMyMDQwNjE3MTMxN1owgZExCzAJBgNV" +
                                "BAYTAlZOMRQwEgYDVQQIDAtIbyBDaGkgTWluaDEQMA4GA1UEBwwHVGh1IER1YzEP" +
                                "MA0GA1UECgwGQ0EgUG50MQ0wCwYDVQQLDARUZXN0MRYwFAYDVQQDDA0xOTIuMTY4" +
                                "LjAuMTA1MSIwIAYJKoZIhvcNAQkBFhNwaGF0Lm50QGhvdG1haWwuY29tMIIBIjAN" +
                                "BgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqcp7wq3YRaeA5RHaVWQEoyC0GwFo" +
                                "Ahn5RNKQvreUgDqn0His6qbHLjNGv1wwMeLLBpnuuPlP + SssliEpp2jAWL564zC7" +
                                "muxOcxWG6q7HvzNkAXNlBmCSBUaCwKvg + F5kA8QI3WiQlGwyAqqK9KS / QswZUEs0" +
                                "LQQhJqh9MK1tGTn8qblWQh6ZgXVO0PjxGSsFnNyCQc0Surukoba5sm3PDG / sdCc +" +
                                "w6BodQiFjFdlaeYyM17l1bkXH5b35Knje9rtVwiTb1sf8MXHppSh2xZpqu2m6SI /" +
                                "Fr2ZWGlq2grsTp6lfvXJr1u / G4KhdsKWZpvBmVZIX1RGWXRU864yw0VIywIDAQAB" +
                                "o1MwUTAdBgNVHQ4EFgQUCLDafQSD7l7SsUsGMvNHjjKsabAwHwYDVR0jBBgwFoAU" +
                                "CLDafQSD7l7SsUsGMvNHjjKsabAwDwYDVR0TAQH / BAUwAwEB / zANBgkqhkiG9w0B" +
                                "AQsFAAOCAQEANZQbhOIiAtE1H1BFsTFMwNmH7QQT5Vndc1yGOC2hLzKmzDGrup + 3" +
                                "S5W8DMGxyyDH0jSG1I0HNY + xYAGyR / fuTBOp + lwaMQAOS0P8YAo1OcE / Exl8ZfXJ" +
                                "0dcM9lb / xPYQxKFyfTOeHMCa2QUAXh5iSr24yJo5Pf / Yxy7LP0dJYZrAeFiLY4pb" +
                                "ZCjRb7GdgNT64GjxBHkX93dqVX + lX1Z / BgLJp2mWoFqdOjjJD4cbZYfm1SqiFFC8" +
                                "j6U8I6aBJx9iHFZug / 1FQIWO5KeX++bhLH8VlcQ5 / sCK + ENArlHRsk88JvWs9578" +
                                "2mzGHMYBWaUYMYtvPsQ1AFSIhPO0S / 9PMw ==" +
                                  "-----END CERTIFICATE-----";
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
                    break;
                case Protocol.Command.BLE_CMD_TEMP_LIMIT:
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
                        if(di.Parse(packet.Data, packet.Length))
                        {
                            vmMainPage.DiInput1 = (di.Values[0] == false) ? "Active" : "No-Active";
                            vmMainPage.DiInput2 = (di.Values[1] == false) ? "Active" : "No-Active";
                            vmMainPage.DiInput3 = (di.Values[2] == false) ? "Active" : "No-Active";
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void MainPage_Appearing(object sender, EventArgs e)
        {
            App.BleProtocol.Received += BleProtocol_Received;
            // App.BleProtocol.Send(Protocol.Command.BLE_CMD_SENSOR_INFO, Protocol.PacketType.Ack, null, 0);
        }

        protected override bool OnBackButtonPressed()
        {
            BackToInit.Verify();
            return true;
        }

        int conneciton = 0;
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

                    // Temp Limit
                    byte[] tlimit_data = BitConverter.GetBytes(float.Parse(vmMainPage.TempLimit));
                    if (sendConfig(Protocol.Command.BLE_CMD_TEMP_LIMIT, tlimit_data, tlimit_data.Length) == false)
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

            //eventWaitHandle.Reset();
            //if (eventWaitHandle.WaitOne(500) == false)
            //{
            //    vmMainPage.ShowProcess = false;
            //    onWriteConfigure = false;
            //    showItoastSafe($"Write {cmd} timeout");
            //    return false;
            //}
            //else
            //{
            //    if (waitCmd == Protocol.Command.BLE_CMD_NACK)
            //    {
            //        onWriteConfigure = false;
            //        vmMainPage.ShowProcess = false;
            //        showItoastSafe($"Write {cmd} failure");
            //        return false;
            //    }
            //}

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
    }
}