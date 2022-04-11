using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GridEye.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NavigateBarTemplate : ContentView
    {
        public static readonly BindableProperty BackTitleProperty = BindableProperty.Create("BackTitle", typeof(string), typeof(NavigateBarTemplate), "Back", BindingMode.TwoWay);
        public static readonly BindableProperty NextTitleProperty = BindableProperty.Create("NextTitle", typeof(string), typeof(NavigateBarTemplate), "Next", BindingMode.TwoWay);
        public static readonly BindableProperty CaptionTitleProperty = BindableProperty.Create("CaptionTitle", typeof(string), typeof(NavigateBarTemplate), "Navigation Bar", BindingMode.TwoWay);
        public static readonly BindableProperty HasBackProperty = BindableProperty.Create("HasBack", typeof(bool), typeof(NavigateBarTemplate), false, BindingMode.TwoWay);
        public static readonly BindableProperty HasNextProperty = BindableProperty.Create("HasNext", typeof(bool), typeof(NavigateBarTemplate), false, BindingMode.TwoWay);
        public static readonly BindableProperty BackEnableProperty = BindableProperty.Create("BackEnable", typeof(bool), typeof(NavigateBarTemplate), false, BindingMode.TwoWay, propertyChanged:OnBackEnableChanged);

        private static void OnBackEnableChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var nbar = bindable as NavigateBarTemplate;
            if (newValue != null)
            {
                nbar.btnBack.IsEnabled = (bool)newValue;
            }
        }

        public static readonly BindableProperty NextEnableProperty = BindableProperty.Create("NextEnable", typeof(bool), typeof(NavigateBarTemplate), false, BindingMode.TwoWay, propertyChanged:OnNextEnableChanged);

        private static void OnNextEnableChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var root = bindable as NavigateBarTemplate;
            if(newValue != null)
            {
                root.btnNext.IsEnabled = (bool)newValue;
            }
        }

        public event EventHandler BackClicked;
        public event EventHandler NextClicked;

        public NavigateBarTemplate()
        {
            InitializeComponent();
        }

        public string BackTitle
        {
            get { return (string)GetValue(BackTitleProperty); }
            set { SetValue(BackTitleProperty, value); }
        }

        public string NextTitle
        {
            get { return (string)GetValue(NextTitleProperty); }
            set { SetValue(NextTitleProperty, value); }
        }

        public string CaptionTitle
        {
            get { return (string)GetValue(CaptionTitleProperty); }
            set { SetValue(CaptionTitleProperty, value); }
        }

        public bool HasBack
        {
            get { return (bool)GetValue(HasBackProperty); }
            set { SetValue(HasBackProperty, value); }
        }

        public bool HasNext
        {
            get { return (bool)GetValue(HasNextProperty); }
            set { SetValue(HasNextProperty, value); }
        }

        public bool BackEnable
        {
            get { return (bool)GetValue(BackEnableProperty); }
            set { SetValue(BackEnableProperty, value); }
        }

        public bool NextEnable
        {
            get { return (bool)GetValue(NextEnableProperty); }
            set { SetValue(NextEnableProperty, value); }
        }

        private void btnBack_Clicked(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.InputTransparent = true;
            BackClicked?.Invoke(sender, e);
            btn.InputTransparent = false;   
        }

        private void btnNext_Clicked(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.InputTransparent = true;
            NextClicked?.Invoke(sender, e);
            btn.InputTransparent = false;
        }
    }
}