using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GridEye.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InstallCookAreaReviewPage : ContentPage
    {
        public InstallCookAreaReviewPage()
        {
            InitializeComponent();
            cookCanvas.SizeChanged += CookCanvas_SizeChanged;
            humanCanvas.SizeChanged += HumanCanvas_SizeChanged;
            this.Appearing += InstallCookAreaReviewPage_Appearing;
            this.Disappearing += InstallCookAreaReviewPage_Disappearing;
        }

        private void InstallCookAreaReviewPage_Disappearing(object sender, EventArgs e)
        {
            App.BleProtocol.Received -= BleProtocol_Received;
        }

        private void InstallCookAreaReviewPage_Appearing(object sender, EventArgs e)
        {
            App.BleProtocol.Received += BleProtocol_Received;
        }

        private void BleProtocol_Received(object sender, Protocol.Packet packet)
        {
            if(packet.Cmd == Protocol.Command.BLE_CMD_SETUP && packet.Type == Protocol.PacketType.Ack)
            {
                this.Dispatcher.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PushAsync(new Views.MainPage());
                });
            }
        }

        private void HumanCanvas_SizeChanged(object sender, EventArgs e)
        {
            humanCanvas.HeightRequest = humanCanvas.Width / 4;
        }

        private void CookCanvas_SizeChanged(object sender, EventArgs e)
        {
            cookCanvas.HeightRequest = cookCanvas.Width;
        }

        private void NavigateBarTemplate_NextClicked(object sender, EventArgs e)
        {
            nbar.NextEnable = false;
            var setup = new Protocol.Data.Setup { Enable = false };
            App.BleProtocol.Send(Protocol.Command.BLE_CMD_SETUP, Protocol.PacketType.Data, setup.GetBytes(), setup.GetLength());
        }

        private async void NavigateBarTemplate_BackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}