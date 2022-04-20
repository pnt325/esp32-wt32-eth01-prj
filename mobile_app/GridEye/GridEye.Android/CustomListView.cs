using Android.Content;
using CustomListViewDemo.Droid;
using WT32EHT01.Views.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
[assembly: ExportRenderer(typeof(CustomListView), typeof(CustomListViewRenderer))]
namespace CustomListViewDemo.Droid
{
    public class CustomListViewRenderer : ListViewRenderer
    {
        Context _context;
        public CustomListViewRenderer(Context context) : base(context)
        {
            _context = context;
        }
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                Control.VerticalScrollBarEnabled = false;
            }
        }
    }
}