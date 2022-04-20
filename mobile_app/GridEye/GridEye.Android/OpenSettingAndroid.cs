using Android.Content;
using WT32EHT01.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(OpenSettingAndroid))]
namespace WT32EHT01.Droid
{
    public class OpenSettingAndroid : IOpenSettingInterface
    {
        public void OpenSetting()
        {
            Android.App.Application.Context.StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings).SetFlags(ActivityFlags.NewTask));
        }
    }
}