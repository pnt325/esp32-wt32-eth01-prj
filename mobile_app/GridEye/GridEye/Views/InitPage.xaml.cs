using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.CommunityToolkit.Extensions;

namespace GridEye.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InitPage : ContentPage
    {
        public InitPage()
        {
            InitializeComponent();
            this.BindingContext = this;
            this.IsBusy = true;
        }

        private async void btnConnect_Clicked(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.InputTransparent = true;
            await Navigation.PushAsync(new Views.DeviceScanPage(false));
            btn.InputTransparent = false;
        }

        private async void btnInstall_Clicked(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.InputTransparent = true;
            await Navigation.PushAsync(new Views.DeviceScanPage(true));
            btn.InputTransparent = false;
        }
    }
}