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
    public partial class TabSub : ContentView
    {
        public static readonly BindableProperty TextTitleProperty = BindableProperty.Create("TextTitle", typeof(string), typeof(TabSub), default(string), BindingMode.TwoWay);

        public string TextTitle
        {
            get { return (string)GetValue(TextTitleProperty); }
            set { SetValue(TextTitleProperty, value); }
        }
        public TabSub()
        {
            InitializeComponent();
        }
    }
}