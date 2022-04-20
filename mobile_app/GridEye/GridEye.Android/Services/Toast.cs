using Android.App;
using Android.Widget;

[assembly: Xamarin.Forms.Dependency(typeof(WT32EHT01.Droid.Services.Toast))]
namespace WT32EHT01.Droid.Services
{
    public class Toast : WT32EHT01.Services.IToast
    {
        public void Show(string msg)
        {
            Android.Widget.Toast.MakeText(Application.Context, msg, ToastLength.Long).Show();
        }
    }
}