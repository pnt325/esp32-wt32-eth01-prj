//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace GridEye.Droid
//{
//    class CustomListView
//    {
//    }
//}

using Android.Content;  
using CustomListViewDemo;  
using CustomListViewDemo.Droid;  
using Xamarin.Forms;  
using Xamarin.Forms.Platform.Android;
using GridEye.Views.Controls;
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