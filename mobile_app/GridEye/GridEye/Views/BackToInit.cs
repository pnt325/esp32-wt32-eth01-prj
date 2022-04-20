using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.CommunityToolkit.Extensions;

namespace WT32EHT01.Views
{
    class BackToInit
    {
        public static async void Verify()
        {
            var result = await App.Current.MainPage.Navigation.ShowPopupAsync(new Views.Main.ExitPopup());
            if (result != null && (bool)result == true)
            {
                if (App.BluetoothGatt.IsConnected())
                {
                    App.BluetoothGatt.Disconnect();
                }
                await App.Current.MainPage.Navigation.PopToRootAsync();
            }
        }
    }
}
