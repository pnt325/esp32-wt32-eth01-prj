using System;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GridEye.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeviceScanPage : ContentPage
    {
        private ViewModels.DeviceScanViewModel deviceScanViewModel = new ViewModels.DeviceScanViewModel();
        bool forInstall;

        public DeviceScanPage(bool forInstall)
        {
            InitializeComponent();
            this.forInstall = forInstall;
            this.Appearing += DeviceScanPage_Appearing;
            this.Disappearing += DeviceScanPage_Disappearing;
            this.BindingContext = deviceScanViewModel;
        }

        private void DeviceScanPage_Disappearing(object sender, EventArgs e)
        {
            App.BluetoothScan.BluetoothScanReceived -= BluetoothScan_BluetoothScanReceived;
            App.BluetoothScan.Stop();
        }

        private void ShowGuide()
        {
            deviceScanViewModel.ShowGuide = deviceScanViewModel.Items.Count > 0 ? false : true;
        }

        private void DeviceScanPage_Appearing(object sender, EventArgs e)
        {
            deviceScanViewModel.Items.Clear();
            ShowGuide();
            if (App.BluetoothScan.Start())
            {
                App.BluetoothScan.BluetoothScanReceived += BluetoothScan_BluetoothScanReceived;
            }
        }

        private void AddNewDevice(Services.BluetoothScanEventArgs args)
        {
            deviceScanViewModel.Items.Add(new Models.DeviceScanModel() { Address = args.Address, Name = args.Name, Rssi = args.Rssi });
            ShowGuide();
        }

        bool addNew = true;
        private void BluetoothScan_BluetoothScanReceived(object sender, Services.BluetoothScanEventArgs args)
        {
            switch (args.Report)
            {
                case Services.BluetoothScanReport.Add:
                    AddNewDevice(args);
                    break;
                case Services.BluetoothScanReport.Update:
                    addNew = true;
                    for (int i = 0; i < deviceScanViewModel.Items.Count; i++)
                    {
                        if (deviceScanViewModel.Items[i].Address == args.Address)
                        {
                            deviceScanViewModel.Items[i].Name = args.Name;
                            deviceScanViewModel.Items[i].Address = args.Address;
                            deviceScanViewModel.Items[i].Rssi = args.Rssi;
                            addNew = false;
                            break;
                        }
                    }

                    if(addNew)
                    {
                        AddNewDevice(args);
                    }

                    break;
                case Services.BluetoothScanReport.Remove:
                    for (int i = 0; i < deviceScanViewModel.Items.Count; i++)
                    {
                        if (deviceScanViewModel.Items[i].Address == args.Address)
                        {
                            deviceScanViewModel.Items.RemoveAt(i);
                            ShowGuide();
                            break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void ListViewDevices_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as Models.DeviceScanModel;

            App.BluetoothScan.Stop();
            deviceScanViewModel.ShowProcess = true;
            App.BluetoothGatt.Connect((string)item.Address.Clone());
            App.BluetoothGatt.Connected += BluetoothGatt_Connected;
        }

        private void BluetoothGatt_Connected(bool result)
        {
            App.BluetoothGatt.Connected -= BluetoothGatt_Connected;

            if (!result)
            {
                deviceScanViewModel.Items.Clear();
                App.BluetoothScan.Start();
                deviceScanViewModel.ShowProcess = false;
            }

            /*TODO: Get sensor info to know senor has cooking area configured or not */
            Dispatcher.BeginInvokeOnMainThread(async () =>
            {
                if (result)
                {
                    if (this.forInstall)
                    {
                        // Do nothing
                    }
                    else
                    {
                        await Navigation.PushAsync(new Views.MainPage());
                    }
                }
                else
                {
                    var toast = DependencyService.Get<Services.IToast>();
                    toast.Show("Connection failure");
                }
            });
        }

        private async void barCtrl_BackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}