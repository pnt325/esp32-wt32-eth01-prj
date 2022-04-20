using System.Collections.Generic;

[assembly: Xamarin.Forms.Dependency(typeof(WT32EHT01.Droid.Services.BLEPer))]
namespace WT32EHT01.Droid.Services
{
    public class BLEPremission : WT32EHT01.Services.IBLEPermission
    {
        bool result = false;
        public async void Request()
        {
            var ret = await Xamarin.Essentials.Permissions.RequestAsync<BLEPer>();
            if (ret == Xamarin.Essentials.PermissionStatus.Granted)
            {
                result = true;
            }
            else
            {
                result = false;
            }
        }

        public bool Result()
        {
            return result;
        }
    }


    public class BLEPer : Xamarin.Essentials.Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new List<(string androidPermission, bool isRuntime)>
        {
            (Android.Manifest.Permission.BluetoothScan, true),
            (Android.Manifest.Permission.BluetoothConnect, true)
        }.ToArray();
    }
}