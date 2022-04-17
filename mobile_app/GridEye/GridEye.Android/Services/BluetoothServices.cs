using Android.App;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.Runtime;
using GridEye.Droid.Services;
using GridEye.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;

[assembly: Xamarin.Forms.Dependency(typeof(BluetoothServices))]
namespace GridEye.Droid.Services
{
    public class BluetoothServices : GridEye.Services.IBluetoothServices
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