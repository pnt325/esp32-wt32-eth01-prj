using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp.Views.Forms;
using SkiaSharp;

namespace GridEye.Views.Controls
{
    class BitmapCanvasView : SKCanvasView
    {
        SKBitmap bitmap;
        public BitmapCanvasView(SKBitmap bitmap)
        {
            this.bitmap = bitmap;
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

        }
    }
}
