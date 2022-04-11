using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GridEye.Services
{
    public class PermissionServices
    {
        public async static void Run()
        {
            // check permission
            var getPer = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (getPer == PermissionStatus.Granted)
            {
                EnableService();
                return;
            }

            // Request permission
            var reqPer = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (reqPer != PermissionStatus.Granted)
            {
                if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>() == false)
                {
                    var mainPage = Application.Current.MainPage;
                    var result = await mainPage.DisplayAlert("Location permission is denied", "Do you want to setting permission?", "Open Setting", "Cancel");
                    if (result)
                    {
                        AppInfo.ShowSettingsUI();
                    }
                    else
                    {
                        Environment.Exit(-1);
                    }
                    return;
                }
            }

            EnableService();
        }

        private static async void EnableService()
        {
            // Location services enabled
            var locDeb = DependencyService.Get<ILocationServices>();
            if (locDeb.Enabled() == false)
            {
                var ans = await Application.Current.MainPage.DisplayAlert("Location service disabled", "Do you want to enable service?", "Open Setting", "Cancel");
                if (ans == false)
                {
                    Environment.Exit(-1);
                    return;
                }

                locDeb.OpenSetting();
                return;
            }

            // Bluetooth enabled
            var bleDeb = DependencyService.Get<IBluetoothServices>();
            if (bleDeb.Enabled() == false)
            {
                string msg = string.Format("Enable service?\r\n(Connection preference/Bluetooth)");
                var ans = await Application.Current.MainPage.DisplayAlert("Bluetooth service disabled", msg, "Open Setting", "Cancel");
                if (ans == false)
                {
                    Environment.Exit(-1);
                    return;
                }
                bleDeb.OpenSetting();
                return;
            }
        }
    }
}
