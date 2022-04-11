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

[assembly:Xamarin.Forms.Dependency(typeof(GridEye.Droid.Services.Toast))]
namespace GridEye.Droid.Services
{
    public class Toast : GridEye.Services.IToast
    {
        public void Show(string msg)
        {
            Android.Widget.Toast.MakeText(Application.Context, msg, ToastLength.Long).Show();
        }
    }
}