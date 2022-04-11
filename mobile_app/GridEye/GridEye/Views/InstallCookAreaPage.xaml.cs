using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp.Views.Forms;
using SkiaSharp;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GridEye.Bitmaps;
using TouchTracking;
using System.Diagnostics;
using GridEye.Views.Crop;

namespace GridEye.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InstallCookAreaPage : ContentPage
    {
        CropperCanvasView photoCropper;
        public InstallCookAreaPage()
        {
            InitializeComponent();
            SKBitmap sKBitmap = new SKBitmap(400, 400);
            photoCropper = new CropperCanvasView(sKBitmap);
            canvasView.Children.Add(photoCropper);
            canvasView.SizeChanged += CanvasView_SizeChanged;
        }

        private void CanvasView_SizeChanged(object sender, EventArgs e)
        {
            photoCropper.HeightRequest = canvasView.Width;
        }

        private async void navBar_BackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void navBar_NextClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.InstallCookAreaReviewPage());
        }

        private void ButtonConfigure_Clicked(object sender, EventArgs e)
        {
            navBar.NextEnable = true;
        }
    }
}