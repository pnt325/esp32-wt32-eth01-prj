using WT32EHT01.Services;
using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace WT32EHT01
{

    public enum Options
    {
        [Description("Option 1")]
        Option1,
        [Description("Option 2")]
        Option2,
        [Description("Option 3")]
        Option3
    }

    public partial class App : Application
    {
        public static IBluetoothGatt BluetoothGatt;
        public static IBluetoothScan BluetoothScan;
        public static Protocol.Protocol BleProtocol;

        public App()
        {
            InitializeComponent();

            BluetoothGatt = DependencyService.Get<IBluetoothGatt>();
            BluetoothScan = DependencyService.Get<IBluetoothScan>();
            BluetoothGatt.Disconnected += BluetoothGatt_Disconnected;
            BleProtocol = new Protocol.Protocol(BluetoothGatt);

            Options op = Options.Option2;
            Console.WriteLine(op.ToString());

            MainPage = new NavigationPage(new Views.InitPage());
        }

        private void BluetoothGatt_Disconnected(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeOnMainThread(async () =>
            {
                var mainPage = Application.Current.MainPage;
                await mainPage.DisplayAlert("Bluetooth disconnected", "Grid eye device lost connection", "Ok");
                await App.Current.MainPage.Navigation.PopToRootAsync();
            });
        }

        protected override void OnStart()
        {
            Console.WriteLine("On Start");
            Services.PermissionServices.Run();
        }

        protected override void OnSleep()
        {
            Console.WriteLine("On Sleep");
        }

        protected override void OnResume()
        {
            Console.WriteLine("On Resume");
            Services.PermissionServices.Run();
        }
    }
}
