using Android.App;
using Android.Bluetooth;
using Android.Content;
using WT32EHT01.Droid.Services;

[assembly: Xamarin.Forms.Dependency(typeof(BluetoothServices))]
namespace WT32EHT01.Droid.Services
{
    public class BluetoothServices : WT32EHT01.Services.IBluetoothServices
    {
        public bool Enabled()
        {
            var bluetoothManager = Android.App.Application.Context.GetSystemService(Context.BluetoothService) as BluetoothManager;
            BluetoothAdapter adapter = bluetoothManager.Adapter;
            return adapter.IsEnabled;
        }

        public void OpenSetting()
        {
            Context context = Application.Context;
            context.StartActivity(new Intent(Android.Provider.Settings.ActionBluetoothSettings).SetFlags(ActivityFlags.NewTask));
        }


    }
}