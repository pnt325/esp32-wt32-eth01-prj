using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WT32EHT01.Views.Main
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExitPopup : Popup
    {
        public ExitPopup()
        {
            InitializeComponent();
            this.Opened += ExitPopup_Opened;
        }

        private void ExitPopup_Opened(object sender, PopupOpenedEventArgs e)
        {
            this.Size = new Size(frame.Width, frame.Height);
        }

        private void btnOk_Clicked(object sender, EventArgs e)
        { 
            Dismiss(true);
        }

        private void btnExit_Clicked(object sender, EventArgs e)
        {
            Dismiss(false);
        }
    }
}