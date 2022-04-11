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
    public partial class InstallCookerTypePage : ContentPage
    {
        public InstallCookerTypePage()
        {
            InitializeComponent();
            this.Appearing += InstallCookerTypePage_Appearing;
            this.Disappearing += InstallCookerTypePage_Disappearing;
            barCtrl.BackClicked += BarCtrl_BackClicked;
            barCtrl.NextClicked += BarCtrl_NextClicked;
        }

        private void InstallCookerTypePage_Disappearing(object sender, EventArgs e)
        {
            App.BleProtocol.Received -= BleProtocol_Received;
        }

        private void InstallCookerTypePage_Appearing(object sender, EventArgs e)
        {
            vm.OvenResult = "Choose";
            vm.CookerTypeResult = "Choose";
            App.BleProtocol.Received += BleProtocol_Received;

            // Enter setup mode
            var setup = new Protocol.Data.Setup { Enable = true };
            App.BleProtocol.Send(Protocol.Command.BLE_CMD_SETUP, Protocol.PacketType.Data, setup.GetBytes(), setup.GetLength());
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
                        vm.TypeOverlay = false;
                        if (packet.Type == Protocol.PacketType.Ack)
                        {
                            barCtrl.NextEnable = true;
                            vm.CookerTypeResult = cookerType.ToString();
                        }
                        else
                        {
                            this.Dispatcher.BeginInvokeOnMainThread(() => 
                            {
                                var toast = DependencyService.Get<Services.IToast>();
                                toast.Show("Cooker type configure failure");
                            });
                        }
                        break;
                    }
                case Protocol.Command.BLE_CMD_SMOKE_SYNC:
                    break;
                case Protocol.Command.BLE_CMD_COOK_AREA:
                    break;
                case Protocol.Command.BLE_CMD_SAVE_CONFIG:
                    break;
                case Protocol.Command.BLE_CMD_SETUP:
                    {
                        if(packet.Type == Protocol.PacketType.Ack)
                        {
                            vm.ShowProcess = false; 
                        }
                        break;
                    }
                case Protocol.Command.BLE_CMD_MAIN_INFO:
                    break;
                case Protocol.Command.BLE_CMD_SENSOR_INFO:
                    break;
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
                    break;
                case Protocol.Command.BLE_CMD_HUMAN_AREA:
                    break;
                default:
                    break;
            }
        }

        //int sendStep;
        private async void BarCtrl_NextClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.InstallSyncPage());
        }

        private void BarCtrl_BackClicked(object sender, EventArgs e)
        {
            BackToInit.Verify();
        }

        protected override bool OnBackButtonPressed()
        {
            BackToInit.Verify();
            return true;
        }

        private void btnOk_Clicked(object sender, EventArgs e)
        {
            if(vm.OvenOverlay)
            {
                vm.OvenResult = isOven ? "Yes" : "No";
                vm.OvenOverlay = false;
            }

            if(vm.TypeOverlay)
            {
                var st = new Protocol.Data.StoveType { Type = cookerType };
                App.BleProtocol.Send(Protocol.Command.BLE_CMD_STOVE_TYPE, Protocol.PacketType.Data, st.GetBytes(), st.GetLength());
                vm.OverlayOkEnable = false;
            }

            if(vm.OvenResult != "Choose" && vm.CookerTypeResult !="Choose")
            {
                barCtrl.NextEnable = true;
            }
            else
            {
                barCtrl.NextEnable = false; 
            }
        }

        private void btnCancel_Clicked(object sender, EventArgs e)
        {
            vm.OvenOverlay = false;
            vm.TypeOverlay = false;
        }

        Protocol.Data.State.StoveType cookerType;
        private void RadioButtonCookerType_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            RadioButton rab = sender as RadioButton;
            if(rab.Content.ToString() == Protocol.Data.State.StoveType.Gas.ToString())
            {
                cookerType = Protocol.Data.State.StoveType.Gas;
            }
            else if (rab.Content.ToString() == Protocol.Data.State.StoveType.Electric.ToString())
            {
                cookerType = Protocol.Data.State.StoveType.Electric;
            }
            else if (rab.Content.ToString() == Protocol.Data.State.StoveType.Induction.ToString())
            {
                cookerType = Protocol.Data.State.StoveType.Induction;
            }

            if(rab.Content.ToString() != "Choose")
            {
                vm.OverlayOkEnable = true;
            }
            else
            {
                vm.OverlayOkEnable = false; 
            }
        }

        bool isOven = false;
        private void RadioButtonOvenType_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            RadioButton rab = sender as RadioButton;
            if(rab.Content.ToString() == "Yes")
            {
                isOven = true;
            }
            else if(rab.Content.ToString() == "No")
            {
                isOven = false;
            }

            if(rab.Content.ToString() != "Choose")
            {
                vm.OverlayOkEnable = true;
            }
            else
            {
                vm.OverlayOkEnable = false; 
            }
        }

        private void TabItemCookerType_Tapped(object sender, EventArgs e)
        {
            vm.TypeOverlay = true;
            vm.OverlayOkEnable = false;
        }
        private void TabItemOven_Tapped(object sender, EventArgs e)
        {
            vm.OvenOverlay = true;
            vm.OverlayOkEnable = false;
        }
    }
}