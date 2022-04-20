using Android.App;
using Android.Content;
using Android.Locations;
using WT32EHT01.Services;

[assembly: Xamarin.Forms.Dependency(typeof(WT32EHT01.Droid.Services.LocationServices))]
namespace WT32EHT01.Droid.Services
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