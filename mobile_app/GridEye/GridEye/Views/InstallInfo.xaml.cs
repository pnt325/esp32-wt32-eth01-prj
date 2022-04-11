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
    public partial class InstallInfo : ContentView
    {
        public static readonly BindableProperty StepTextProperty = BindableProperty.Create(
            propertyName: "StepText",
            returnType: typeof(string),
            declaringType: typeof(InstallInfo),
            defaultValue: "",
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: StepTextPropertyChanged);
        public static readonly BindableProperty StepInfoProperty = BindableProperty.Create(
            propertyName: "StepInfo",
            returnType: typeof(string),
            declaringType: typeof(InstallInfo),
            defaultValue: "",
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: StepInfoPropertyChanged);

        private static void StepInfoPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var ctrl = (InstallInfo)bindable;
            ctrl.txtInfo.Text = newValue.ToString();
        }

        private static void StepTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (InstallInfo)bindable;
            control.txtStep.Text = newValue.ToString();
        }

        public InstallInfo()
        {
            InitializeComponent();
        }

        public string StepText
        {
            get
            {
                return (string)GetValue(StepTextProperty);
            }
            set
            {
                SetValue(StepTextProperty, value);
            }
        }

        public string StepInfo
        {
            get
            {
                return (string)GetValue(StepInfoProperty);
            }
            set
            {
                SetValue(StepInfoProperty, value);
            }
        }
    }
}