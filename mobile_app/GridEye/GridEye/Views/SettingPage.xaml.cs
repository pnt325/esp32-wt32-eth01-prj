using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GridEye.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        public SettingPage()
        {
            InitializeComponent();
            this.Appearing += SettingPage_Appearing;
            this.Disappearing += SettingPage_Disappearing;
            vm.Model = "Sensor Unit";
        }

        private void SettingPage_Disappearing(object sender, EventArgs e)
        {
            App.BleProtocol.Received -= BleProtocol_Received;
        }

        private void SettingPage_Appearing(object sender, EventArgs e)
        {
            App.BleProtocol.Received += BleProtocol_Received;
            App.BleProtocol.Send(Protocol.Command.BLE_CMD_SENSOR_INFO, Protocol.PacketType.Ack, null, 0);
        }

        private void BleProtocol_Received(object sender, Protocol.Packet packet)
        {
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
                    {
                        if(packet.Type == Protocol.PacketType.Nack)
                        {
                            if (vm.ShowCookerOverlay)
                            {
                                vm.OverlayOkEnable = true;
                            }
                        }
                        else
                        {
                            if (vm.ShowCookerOverlay)
                            {
                                vm.ShowCookerOverlay = false;
                                vm.CookerType = cookerType.ToString();
                                App.BleProtocol.Send(Protocol.Command.BLE_CMD_SAVE_CONFIG, Protocol.PacketType.Data, null, 0);
                            }
                        }
                        break;
                    }
                case Protocol.Command.BLE_CMD_SMOKE_SYNC:
                    break;
                case Protocol.Command.BLE_CMD_COOK_AREA:
                    break;
                case Protocol.Command.BLE_CMD_SAVE_CONFIG:
                    {
                        break;
                    }
                case Protocol.Command.BLE_CMD_SETUP:
                    break;
                case Protocol.Command.BLE_CMD_MAIN_INFO:
                    break;
                case Protocol.Command.BLE_CMD_SENSOR_INFO:
                    {
                        if (packet.Type == Protocol.PacketType.Nack)
                        {
                            var toast = DependencyService.Get<Services.IToast>();
                            toast.Show("Sensor info failure");
                            break;
                        }
                        if (packet.Type != Protocol.PacketType.Data)
                        {
                            break;
                        }

                        Protocol.Data.SensorInfo sensorInfo = new Protocol.Data.SensorInfo();
                        if (sensorInfo.Parse(packet.Data, packet.Length) == false)
                        {
                            var toast = DependencyService.Get<Services.IToast>();
                            toast.Show("Sensor info wrong format");
                            break;
                        }

                        vm.FirmwareVersion = Protocol.Data.Utils.VersionHelps.GetString(sensorInfo.Version);
                        vm.Battery = sensorInfo.BatteryCapacity;
                        vm.CookerType = sensorInfo.StoveType.ToString();
                        if(vm.SafeEnhance != sensorInfo.SafeEnhance)
                        {
                            safeEnhanceUpdate = true;
                        }
                        vm.SafeEnhance = sensorInfo.SafeEnhance;
                        break;
                    }
                case Protocol.Command.BLE_CMD_STOVE_LOCK:
                    break;
                case Protocol.Command.BLE_CMD_SAFE_PROFILE:
                    break;
                case Protocol.Command.BLE_CMD_SN:
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
                    {
                        if (packet.Type == Protocol.PacketType.Nack)
                        {
                            safeEnhanceUpdate = true;
                            vm.SafeEnhance = !vm.SafeEnhance;
                        }
                        break;
                    }
                case Protocol.Command.BLE_CMD_HUMAN_AREA:
                    break;
                default:
                    break;
            }
        }

        private async void NavigateBarTemplate_BackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(true);
        }

        private async void TabItemCookArea_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.InstallCookAreaPage());
        }

        private async void TabItemSystemSync_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.InstallSyncPage());
        }

        private void TabItemCookerType_Tapped(object sender, EventArgs e)
        {
            vm.ShowCookerOverlay = true;
        }

        private void TabItemSafeEnhance_Tapped(object sender, EventArgs e)
        {
            vm.ShowSafeEnhanceOverlay = true;
        }

        private void overlayCancel_Clicked(object sender, EventArgs e)
        {
            vm.ShowCookerOverlay = false;
            vm.ShowSafeEnhanceOverlay = false;
        }

        Protocol.Data.State.StoveType cookerType;
        private void overlayOk_Clicked(object sender, EventArgs e)
        {
            if(vm.ShowCookerOverlay)
            {
                Protocol.Data.StoveType stoveType = new Protocol.Data.StoveType { Type = cookerType };
                App.BleProtocol.Send(Protocol.Command.BLE_CMD_STOVE_TYPE, Protocol.PacketType.Data, stoveType.GetBytes(), stoveType.GetLength());
                vm.OverlayOkEnable = false; 
            }
        }


        private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            RadioButton rdb = sender as RadioButton;
            if (rdb.Content.ToString() == Protocol.Data.State.StoveType.Gas.ToString())
            {
                cookerType = Protocol.Data.State.StoveType.Gas;
            }
            else if (rdb.Content.ToString() == Protocol.Data.State.StoveType.Electric.ToString())
            {
                cookerType = Protocol.Data.State.StoveType.Electric;
            }
            else if (rdb.Content.ToString() == Protocol.Data.State.StoveType.Induction.ToString())
            {
                cookerType = Protocol.Data.State.StoveType.Induction;
            }

            if (rdb.Content.ToString() == vm.CookerType)
            {
                vm.OverlayOkEnable = false;
            }
            else
            {
                vm.OverlayOkEnable = true;
            }
        }

        bool safeEnhanceUpdate = false;
        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            Switch _switch = sender as Switch;
            if(safeEnhanceUpdate)
            {
                safeEnhanceUpdate = false;
                return;
            }

            Protocol.Data.SafeEnhance safeEnhance = new Protocol.Data.SafeEnhance { Enable = _switch.IsToggled };
            App.BleProtocol.Send(Protocol.Command.BLE_CMD_SAFE_ENHANCE,Protocol.PacketType.Data, safeEnhance.GetBytes(), safeEnhance.GetLength());
        }
    }
}