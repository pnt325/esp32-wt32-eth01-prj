﻿using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;



namespace GridEye.Droid
{
    [Activity(Label = "WT32", ScreenOrientation=ScreenOrientation.Portrait, Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::ZXing.Net.Mobile.Forms.Android.Platform.Init();
            LoadApplication(new App());

            await Xamarin.Essentials.Permissions.RequestAsync<BLEPermission>();
            //if(blePer != Xamarin.Essentials.PermissionStatus.Granted)
            //{
            //}
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}