using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GridEye.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        Entry[] pswdEntrys;
        int pswdEntryIndex = 0;
        public MainPage()
        {
            InitializeComponent();
            this.Appearing += MainPage_Appearing;
            this.Disappearing += MainPage_Disappearing;
            pswdEntrys = new Entry[4];
            //pswdEntrys[0] = pswdEntry1;
            //pswdEntrys[1] = pswdEntry2;
            //pswdEntrys[2] = pswdEntry3;
            //pswdEntrys[3] = pswdEntry4;
        }

        private void MainPage_Disappearing(object sender, EventArgs e)
        {
            App.BleProtocol.Received -= BleProtocol_Received;
        }

        private void BleProtocol_Received(object sender, Protocol.Packet packet)
        {
            Debug.WriteLine($"Received data cmd: {packet.Cmd}, len: {packet.Length}");
            return;

            switch (packet.Cmd)
            {
                case Protocol.Command.BLE_CMD_NONE:
                    break;
                case Protocol.Command.BLE_CMD_INVALID:
                    break;
                case Protocol.Command.BLE_CMD_RF_SYNC:
                    break;
                case Protocol.Command.BLE_CMD_TEMPERATURE_SCAN:
                    break;
                case Protocol.Command.BLE_CMD_MODE:
                    break;
                case Protocol.Command.BLE_CMD_STOVE_TYPE:
                    break;
                case Protocol.Command.BLE_CMD_SMOKE_SYNC:
                    break;
                case Protocol.Command.BLE_CMD_COOK_AREA:
                    break;
                case Protocol.Command.BLE_CMD_SAVE_CONFIG:
                    break;
                case Protocol.Command.BLE_CMD_SETUP:
                    {
                        requestSetting = false;
                        vmMainPage.ShowProcess = false; 
                        if (packet.Type == Protocol.PacketType.Ack)
                        {
                            this.Dispatcher.BeginInvokeOnMainThread(async () =>
                            {
                                await Navigation.PushAsync(new Views.SettingPage());
                            });
                        }
                        else
                        {
                            this.Dispatcher.BeginInvokeOnMainThread(() =>
                            {
                                var toast = DependencyService.Get<Services.IToast>();
                                toast.Show("Enter setup failure");
                            });
                        }
                        break;
                    }
                case Protocol.Command.BLE_CMD_MAIN_INFO:
                    break;
                case Protocol.Command.BLE_CMD_SENSOR_INFO:
                    {
                        if(packet.Type == Protocol.PacketType.Nack)
                        {
                            var toast = DependencyService.Get<Services.IToast>();
                            toast.Show("Sensor info failure");
                            break;
                        }
                        if(packet.Type != Protocol.PacketType.Data)
                        {
                            break;
                        }

                        var sensorInfo = new Protocol.Data.SensorInfo();
                        if (!sensorInfo.Parse(packet.Data, packet.Length))
                        {
                            break;
                        }

                        // Update system
                        if(sensorInfo.SyncMode == Protocol.Data.State.SyncMode.Main)
                        {
                            vmMainPage.System = "Main";
                        }
                        else if(sensorInfo.SyncMode == Protocol.Data.State.SyncMode.Smoke)
                        {
                            vmMainPage.System = "Main_Smoke";
                        }
                        else
                        {
                            vmMainPage.System = "None";
                        }

                        vmMainPage.Battery = sensorInfo.BatteryCapacity;
                        vmMainPage.Mode = sensorInfo.Mode.ToString();
                        if(sensorInfo.Mode == Protocol.Data.State.AppMode.System && sensorInfo.TaskState != Protocol.Data.State.TaskState.Failure)
                        {
                            vmMainPage.LockEnable = true;
                        }
                        else
                        {
                            vmMainPage.LockEnable = false;
                        }

                        // Update temperature chart/
                        vmMainPage.Temperature = sensorInfo.Temperature;
                        vmMainPage.TaskState = sensorInfo.TaskState.ToString();
                        vmMainPage.SafeProfile = sensorInfo.SafeProfile.ToString();
                        break;
                    }
                case Protocol.Command.BLE_CMD_STOVE_LOCK:
                    {
                        if(packet.Type == Protocol.PacketType.Nack)
                        {
                            if(vmMainPage.LockOverlay)
                            {
                                ShowOverlayTitleResult("Password failure");
                            }
                            else
                            {
                                var toast = DependencyService.Get<Services.IToast>();
                                toast.Show("Stove lock failure");
                            }
                            break;  
                        }

                        var stoveLock = new Protocol.Data.StoveLock();
                        if(!stoveLock.Parse(packet.Data, packet.Length))
                        {
                            System.Diagnostics.Debug.WriteLine("Packet parse failure", "BLE_RECV");
                            break;  
                        }

                        switchUpdate = true;
                        vmMainPage.StoveLock = stoveLock.Lock;
                        if(vmMainPage.LockOverlay)
                        {
                            vmMainPage.LockOverlay = false;
                        }
                    }
                    break;
                case Protocol.Command.BLE_CMD_SAFE_PROFILE:
                    {
                        if(packet.Type != Protocol.PacketType.Ack)
                        {
                            if (vmMainPage.ProfileOverlay)
                            {
                                ShowOverlayTitleResult("Profile invalid");
                                vmMainPage.OverlayOkEnable = true;
                            }
                            break;
                        }

                        if (vmMainPage.ProfileOverlay)
                        {
                            vmMainPage.ProfileOverlay = false;
                        }
                        vmMainPage.SafeProfile = safeProfile.ToString();
                    }
                    break;
                case Protocol.Command.BLE_CMD_MOVE_DATA:
                    break;
                case Protocol.Command.BLE_CMD_TEMP_ROW_0:
                    break;
                case Protocol.Command.BLE_CMD_TEMP_ROW_1:
                    break;
                case Protocol.Command.BLE_CMD_TEMP_ROW_2:
                    break;
                case Protocol.Command.BLE_CMD_TEMP_ROW_3:
                    break;
                case Protocol.Command.BLE_CMD_TEMP_ROW_4:
                    break;
                case Protocol.Command.BLE_CMD_TEMP_ROW_5:
                    break;
                case Protocol.Command.BLE_CMD_TEMP_ROW_6:
                    break;
                case Protocol.Command.BLE_CMD_TEMP_ROW_7:
                    break;
                case Protocol.Command.BLE_CMD_TEMP_ROW_8:
                    break;
                case Protocol.Command.BLE_CMD_TEMP_ROW_9:
                    break;
                case Protocol.Command.BLE_CMD_TEMP_ROW_10:
                    break;
                case Protocol.Command.BLE_CMD_TEMP_ROW_11:
                    break;
                case Protocol.Command.BLE_CMD_TEMP_ROW_12:
                    break;
                case Protocol.Command.BLE_CMD_TEMP_ROW_13:
                    break;
                case Protocol.Command.BLE_CMD_TEMP_ROW_14:
                    break;
                case Protocol.Command.BLE_CMD_TEMP_ROW_15:
                    break;
                case Protocol.Command.BLE_CMD_SAFE_ENHANCE:
                    break;
                case Protocol.Command.BLE_CMD_HUMAN_AREA:
                    {
                        var humanarea = new Protocol.Data.HumanDetect();
                        if(!humanarea.Parse(packet.Data, packet.Length))
                        {
                            //TODO get human detected
                            break;
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
            App.BleProtocol.Send(Protocol.Command.BLE_CMD_SENSOR_INFO, Protocol.PacketType.Ack, null, 0);
        }

        bool requestSetting = false;
        private void btnSetting_Clicked(object sender, EventArgs e)
        {
            if(requestSetting)
            {
                return;
            }
            vmMainPage.ShowProcess = true;
            Protocol.Data.Setup setup = new Protocol.Data.Setup { Enable = true };
            App.BleProtocol.Send(Protocol.Command.BLE_CMD_SETUP, Protocol.PacketType.Data, setup.GetBytes(), setup.GetLength());
            requestSetting = true;
        }

        private void btnExit_Clicked(object sender, EventArgs e)
        {
            BackToInit.Verify();
        }

        protected override bool OnBackButtonPressed()
        {
            BackToInit.Verify();
            return true;
        }

        private void TapGestureRecognizerSafeProfile_Tapped(object sender, EventArgs e)
        {
            vmMainPage.OverlayTitle = "Choose safe profile";
            ShowOverlayTitleResult("");
            vmMainPage.ProfileOverlay = true;
        }

        bool switchUpdate = false;
        private void switchStoveLock_Toggled(object sender, ToggledEventArgs e)
        {
            if(switchUpdate)
            {
                switchUpdate = false;
                return;
            }

            vmMainPage.LockOverlay = true;
            ShowOverlayTitleResult("");
            for (int i = 0; i < pswdEntrys.Length; i++)
            {
                pswdEntrys[i].Text = "";
            }
            pswdEntryIndex = 0;
            pswdEntrys[pswdEntryIndex].Focus();
            vmMainPage.OverlayTitle = e.Value ? "Unlock password" : "Lock password";
        }

        private void ShowOverlayTitleResult(string title)
        {
            if(string.IsNullOrEmpty(title))
            {
                vmMainPage.OverlayTitleResultVisible = false;
                return;
            }
            vmMainPage.OverlayTitleResultVisible = true;
            vmMainPage.OverlayTitleResult = title;  
        }

        private void overlayCancel_Clicked(object sender, EventArgs e)
        {
            if(vmMainPage.LockOverlay)
            {
                switchUpdate = true;
                vmMainPage.StoveLock = !vmMainPage.StoveLock;
            }

            vmMainPage.LockOverlay = false;
            vmMainPage.ProfileOverlay = false;
            vmMainPage.OverlayOkEnable = false;
        }

        private void overlayOk_Clicked(object sender, EventArgs e)
        {
            if (vmMainPage.LockOverlay)
            {
                //TODO implement send password to main.
            }

            if (vmMainPage.ProfileOverlay)
            {
                Protocol.Data.SafeProfile safeProfile = new Protocol.Data.SafeProfile { Profile = this.safeProfile };
                vmMainPage.OverlayOkEnable = false; 
                App.BleProtocol.Send(Protocol.Command.BLE_CMD_SAFE_PROFILE, Protocol.PacketType.Data, safeProfile.GetBytes(), safeProfile.GetLength());
            }
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Entry entry = sender as Entry;
            if(string.IsNullOrEmpty(e.NewTextValue))
            {
                return;
            }

            char c = (char)e.NewTextValue[0];
            if(c < '0' && c > '9')
            {
                entry.Text = e.OldTextValue;
                return;
            }

            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                if (pswdEntryIndex < 3)
                {
                    pswdEntryIndex++;
                    pswdEntrys[pswdEntryIndex].Focus();
                }
            }

            int i;
            for (i = 0; i < pswdEntrys.Length; i++)
            {
                if (string.IsNullOrEmpty(pswdEntrys[i].Text))
                {
                    break;
                }
            }
            
            if(i >= pswdEntrys.Length)
            {
                vmMainPage.OverlayOkEnable = true;
            }
            else
            {
                vmMainPage.OverlayOkEnable = false;
            }
        }

        private void pswdEntry1_Focused(object sender, FocusEventArgs e)
        {
            Entry en = sender as Entry;
            for (int i = 0; i < pswdEntrys.Length; i++)
            {
                if(en == pswdEntrys[i])
                {
                    pswdEntryIndex = i;
                    if (en.Text != null)
                    {
                        en.CursorPosition = 0;
                        en.SelectionLength = 1;
                    }

                    if(vmMainPage.OverlayTitleResultVisible)
                    {
                        ShowOverlayTitleResult("");
                    }
                    break;
                }
            }
        }

        bool profileUpdate = false;
        Protocol.Data.State.SafeProfile safeProfile;
        private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if(profileUpdate)
            {
                profileUpdate = false;
                return;
            }

            RadioButton rb = sender as RadioButton;
            if(rb.Content.ToString() == Protocol.Data.State.SafeProfile.Normal.ToString())
            {
                safeProfile = Protocol.Data.State.SafeProfile.Normal;
            }
            else if(rb.Content.ToString() == Protocol.Data.State.SafeProfile.Medium.ToString())
            {
                safeProfile = Protocol.Data.State.SafeProfile.Medium;
            }
            else if(rb.Content.ToString() == Protocol.Data.State.SafeProfile.High.ToString())
            {
                safeProfile = Protocol.Data.State.SafeProfile.High;
            }

            if(rb.Content.ToString() != vmMainPage.SafeProfile)
            {
                vmMainPage.OverlayOkEnable = true;
            }
            else
            {
                vmMainPage.OverlayOkEnable = false;
            }
        }
    }
}