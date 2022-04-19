using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(GridEye.Droid.Services.BLEPer))]
namespace GridEye.Droid.Services
{
    public class BLEPremission : GridEye.Services.IBLEPermission
    {
        bool result = false;
        public async void Request()
        {
            var ret = await Xamarin.Essentials.Permissions.RequestAsync<BLEPer>();
            if(ret == Xamarin.Essentials.PermissionStatus.Granted)
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