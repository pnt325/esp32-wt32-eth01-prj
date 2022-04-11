using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GridEye.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Locations;

[assembly:Xamarin.Forms.Dependency(typeof(GridEye.Droid.Services.LocationServices))]
namespace GridEye.Droid.Services
{
    public class LocationServices : ILocationServices
    {
        public bool Enabled()
        {
            LocationManager LM = (LocationManager)Android.App.Application.Context.GetSystemService(Context.LocationService);
            return LM.IsProviderEnabled(Android.Locations.LocationManager.GpsProvider);
        }

        public void OpenSetting()
        {
            Context context = Application.Context;
            context.StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings).SetFlags(ActivityFlags.NewTask));
        }
    }
}