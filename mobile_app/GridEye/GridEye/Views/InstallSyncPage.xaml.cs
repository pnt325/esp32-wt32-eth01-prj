using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GridEye.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InstallSyncPage : ContentPage
    {
        public InstallSyncPage()
        {
            InitializeComponent();
            vm.SyncTypeResult = "Choose";
            vm.MainSwVisible = false;
            vm.SmokeSwVisible = false;

            scanCode.Options = new ZXing.Mobile.MobileBarcodeScanningOptions
            {
                PossibleFormats = new List<ZXing.BarcodeFormat>
                {
                    ZXing.BarcodeFormat.QR_CODE
                }
            };
        }

        protected override bool OnBackButtonPressed()
        {
            if (vm.CameraOvelay)
            {
                vm.CameraOvelay = false;
                qrScanOverlay.BottomText = "";
                return true;
            }
            return base.OnBackButtonPressed();
        }

        Protocol.Data.State.SyncMode syncMode;
        private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            RadioButton rab = sender as RadioButton;
            if (rab.Content.ToString() == "Main")
            {
                syncMode = Protocol.Data.State.SyncMode.Main;
            }
            else if (rab.Content.ToString() == "Main + Smoke")
            {
                syncMode = Protocol.Data.State.SyncMode.Smoke;
            }
            else if (rab.Content.ToString() == "None")
            {
                syncMode = Protocol.Data.State.SyncMode.None;
            }

            if (rab.Content.ToString() != "Chooose")
            {
                vm.OverlayOkEnable = true;
            }
            else
            {
                vm.OverlayOkEnable = false;
            }
        }

        private async void NavigateBarTemplate_BackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void NavigateBarTemplate_NextClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.InstallCookAreaPage());
        }

        private void ZXingScannerView_OnScanResult(ZXing.Result result)
        {
            // check that qr code is valid;
            if (string.IsNullOrEmpty(result.Text))
            {
                Dispatcher.BeginInvokeOnMainThread(() =>
                {
                    qrScanOverlay.BottomText = $"QR_CODE invalid {result.Text}";
                });
                return;
            }


            string[] qrs = result.Text.ToLower().Split('-');
            if (qrs.Length != 4)
            {
                qrScanOverlay.BottomText = $"QR_CODE format invalid {result.Text}";
                return;
            }

            for (int i = 0; i < 4; i++)
            {
                if (qrs[i].Length != 2)
                {
                    qrScanOverlay.BottomText = $"QR_CODE data invalid {result.Text}";
                    return;
                }

                for (int j = 0; j < 2; j++)
                {
                    char frChar = qrs[i][j];
                    if ((frChar >= '0' && frChar <= '9') || (frChar >= 'a' && frChar <= 'f'))
                    {
                        continue;
                    }
                    else
                    {
                        qrScanOverlay.BottomText = $"QR_CODE data invalid {result.Text}";
                        return;
                    }
                }
            }

            vm.CameraOvelay = false;
            if (camShow == Protocol.Data.State.SyncMode.Main)
            {
                mainSwInput.SyncWord1 = qrs[0];
                mainSwInput.SyncWord2 = qrs[1];
                mainSwInput.SyncWord3 = qrs[2];
                mainSwInput.SyncWord4 = qrs[3];
            }
            else if (camShow == Protocol.Data.State.SyncMode.Smoke)
            {
                smokeSwInput.SyncWord1 = qrs[0];
                smokeSwInput.SyncWord2 = qrs[1];
                smokeSwInput.SyncWord3 = qrs[2];
                smokeSwInput.SyncWord4 = qrs[3];
            }
        }

        Protocol.Data.State.SyncMode camShow;
        private void SyncWordInputMain_ShowCameraClicked(object sender, EventArgs e)
        {
            vm.CameraOvelay = true;
            camShow = Protocol.Data.State.SyncMode.Main;
        }
        private void SyncWordInputSmoke_ShowCameraClicked(object sender, EventArgs e)
        {
            vm.CameraOvelay = true;
            camShow = Protocol.Data.State.SyncMode.Smoke;
        }

        private void overlayCancel_Clicked(object sender, EventArgs e)
        {
            vm.SyncTypeOverlay = false;
        }

        private void overlayOk_Clicked(object sender, EventArgs e)
        {
            if (vm.SyncTypeOverlay)
            {
                vm.SyncTypeOverlay = false;
                switch (syncMode)
                {
                    case Protocol.Data.State.SyncMode.Main:
                        vm.MainSwVisible = true;
                        vm.SmokeSwVisible = false;
                        vm.SyncTypeResult = "Main";
                        vm.NextEnable = false;
                        break;
                    case Protocol.Data.State.SyncMode.Smoke:
                        vm.SmokeSwVisible = true;
                        vm.MainSwVisible = true;
                        vm.SyncTypeResult = "Main + Smoke";
                        vm.NextEnable = false;
                        break;
                    case Protocol.Data.State.SyncMode.None:
                        vm.MainSwVisible = false;
                        vm.SmokeSwVisible = false;
                        vm.SyncTypeResult = "None";
                        vm.NextEnable = true;
                        break;
                    default:
                        break;
                }
            }
        }

        private void TabItem_Tapped(object sender, EventArgs e)
        {
            vm.OverlayOkEnable = false;
            vm.SyncTypeOverlay = true;
        }

        private void ButtonSync_Clicked(object sender, EventArgs e)
        {
            bool isValid = true;
            int i;
            if (syncMode == Protocol.Data.State.SyncMode.Main)
            {
                string[] sws = new string[]
                {
                    mainSwInput.SyncWord1,
                    mainSwInput.SyncWord2,
                    mainSwInput.SyncWord3,
                    mainSwInput.SyncWord4
                };
                for (i = 0; i < 4; i++)
                {
                    if (sws[i] == null || sws[i].Length != 2)
                    {
                        this.DisplayAlert("Main sync-word", "Sync-word not fill", "Ok");
                        isValid = false;
                        break;
                    }
                }
            }

            if (syncMode == Protocol.Data.State.SyncMode.Smoke)
            {
                string[] sws = new string[]
                {
                    smokeSwInput.SyncWord1,
                    smokeSwInput.SyncWord2,
                    smokeSwInput.SyncWord3,
                    smokeSwInput.SyncWord4
                };
                for (i = 0; i < 4; i++)
                {
                    if (sws[i] == null || sws[i].Length != 2)
                    {
                        this.DisplayAlert("Smoke sync-word", "Sync-word not fill", "Ok");
                        isValid = false;
                        break;
                    }
                }
            }

            if (isValid)
            {
                Debug.WriteLine("Sync word valid", "RF_SYNC");

                // TODO send sync-word to configure on sensor unint.
                vm.NextEnable = true;
            }
        }
    }
}