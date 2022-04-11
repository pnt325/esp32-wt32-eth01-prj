using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GridEye.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SyncWordInput : ContentView
    {
        public static readonly BindableProperty SyncWord1Property = BindableProperty.Create("SyncWord1", typeof(string), typeof(SyncWordInput), default(string), BindingMode.TwoWay);
        public static readonly BindableProperty SyncWord2Property = BindableProperty.Create("SyncWord2", typeof(string), typeof(SyncWordInput), default(string), BindingMode.TwoWay);
        public static readonly BindableProperty SyncWord3Property = BindableProperty.Create("SyncWord3", typeof(string), typeof(SyncWordInput), default(string), BindingMode.TwoWay);
        public static readonly BindableProperty SyncWord4Property = BindableProperty.Create("SyncWord4", typeof(string), typeof(SyncWordInput), default(string), BindingMode.TwoWay);
        
        public event EventHandler ShowCameraClicked;

        public string SyncWord1
        {
            get { return (string)GetValue(SyncWord1Property); }
            set { SetValue(SyncWord1Property, value); }
        }

        public string SyncWord2
        {
            get { return (string)GetValue(SyncWord2Property); }
            set { SetValue(SyncWord2Property, value); }
        }
        public string SyncWord3
        {
            get { return (string)GetValue(SyncWord3Property); }
            set { SetValue(SyncWord3Property, value); }
        }
        public string SyncWord4
        {
            get { return (string)GetValue(SyncWord4Property); }
            set { SetValue(SyncWord4Property, value); }
        }

        public SyncWordInput()
        {
            InitializeComponent();
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            ImageButton ibtn = sender as ImageButton;
            ibtn.InputTransparent = true;
            ShowCameraClicked?.Invoke(sender, e);
            ibtn.InputTransparent = false;
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(e.NewTextValue.Length == 0)
            {
                return;
            }

            char cc = e.NewTextValue[e.NewTextValue.Length - 1];
            if ((cc >= '0' && cc <= '9') || (cc >= 'A' && cc <= 'F') || (cc >= 'a' && cc <= 'f'))
            {
                return;
            }
            (sender as Entry).Text = e.NewTextValue.Remove(e.NewTextValue.Length - 1);
        }
    }
}