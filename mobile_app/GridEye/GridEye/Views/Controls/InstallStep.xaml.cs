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
    public partial class InstallStep : ContentView
    {
        public static readonly BindableProperty StepTitleProperty = BindableProperty.Create("StepTitle", typeof(string), typeof(InstallStep), default(string), BindingMode.TwoWay);

        public string StepTitle
        {
            get
            {
                return (string)GetValue(StepTitleProperty);
            }
            set
            {
                SetValue(StepTitleProperty, value);
            }
        }
        
        public static readonly BindableProperty StepDetailProperty = BindableProperty.Create("StepDetail", typeof(string), typeof(InstallStep), default(string), BindingMode.TwoWay);

        public string StepDetail
        {
            get { return (string)GetValue(StepDetailProperty); }
            set { SetValue(StepDetailProperty, value); }
        }


        public InstallStep()
        {
            InitializeComponent();
            this.BindingContext = this;
        }
    }
}