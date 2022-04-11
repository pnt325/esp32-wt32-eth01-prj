using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GridEye.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadOverlay : Popup
    {
        public LoadOverlay()
        {
            InitializeComponent();
        }
    }
}