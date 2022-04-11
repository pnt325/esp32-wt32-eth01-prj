using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GridEye.Droid.Controls;
using GridEye.Views.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TextAlignmentButton), typeof(TextAlignmentButtonRenderer))]
namespace GridEye.Droid.Controls
{
    public class TextAlignmentButtonRenderer : ButtonRenderer
    {
        public TextAlignmentButtonRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            SetTextHorizontalAlignment();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == TextAlignmentButton.HorizontalTextAlignmentProperty.PropertyName)
                SetTextHorizontalAlignment();
        }

        private void SetTextHorizontalAlignment()
        {
            var button = (TextAlignmentButton)Element;
            Control?.SetAllCaps(button.TextTransform != TextTransform.None);
            switch (button.HorizontalTextAlignment)
            {
                case Xamarin.Forms.TextAlignment.Center:
                    Control.Gravity = GravityFlags.AxisSpecified | GravityFlags.CenterVertical;
                    break;
                case Xamarin.Forms.TextAlignment.Start:
                    Control.Gravity = GravityFlags.Left | GravityFlags.CenterVertical;
                    break;
                case Xamarin.Forms.TextAlignment.End:
                    Control.Gravity = GravityFlags.Right | GravityFlags.CenterVertical;
                    break;
                default:
                    Control.Gravity = GravityFlags.AxisSpecified | GravityFlags.CenterVertical;
                    break;
            }
        }
    }
}