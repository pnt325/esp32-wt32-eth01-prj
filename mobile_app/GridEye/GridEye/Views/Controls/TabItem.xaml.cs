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
    public partial class TabItem : ContentView
    {
        public static readonly BindableProperty TitleProperty = BindableProperty.Create("Title", typeof(string), typeof(TabItem), "", BindingMode.TwoWay);
        public static readonly BindableProperty ResultProperty = BindableProperty.Create("Result", typeof(string), typeof(TabItem), "", BindingMode.TwoWay);
        public static readonly BindableProperty ResultSelectedProperty = BindableProperty.Create("ResultSelected", typeof(bool), typeof(TabItem), false, BindingMode.TwoWay);

        public event EventHandler Tapped;

        public TabItem()
        {
            InitializeComponent();
        }

        public bool ResultSelected
        {
            get
            {
                return (bool)GetValue(ResultSelectedProperty);
            }
            set
            {
                SetValue(ResultSelectedProperty, value);
            }
        }

        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public string Result
        {
            get
            {
                return (string)GetValue(ResultProperty);
            }
            set
            {
                SetValue(ResultProperty, value);
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if(ResultSelected)
            {
                Tapped?.Invoke(this, e);
            }
        }
    }
}
