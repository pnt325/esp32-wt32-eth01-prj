using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace GridEye.Views.Controls
{
    public class TextAlignmentButton : Button
    {
        public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create(
           "HorizontalTextAlignment",
           typeof(TextAlignment),
           typeof(TextAlignmentButton),
           TextAlignment.Center);

        public TextAlignment HorizontalTextAlignment
        {
            get => (TextAlignment)GetValue(HorizontalTextAlignmentProperty);
            set => SetValue(HorizontalTextAlignmentProperty, value);
        }
    }
}
