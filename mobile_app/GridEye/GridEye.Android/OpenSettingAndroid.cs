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

using GridEye;
using Xamarin.Forms;
using GridEye.Droid;

[assembly: Dependency(typeof(OpenSettingAndroid))]
namespace GridEye.Droid
{
    public class OpenSettingAndroid : IOpenSettingInterface
    {
        public void OpenSetting()
        {
            Android.App.Application.Context.StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings).SetFlags(ActivityFlags.NewTask));
        }
    }
}