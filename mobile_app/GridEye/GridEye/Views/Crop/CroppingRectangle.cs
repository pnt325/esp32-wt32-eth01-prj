using System;
using System.Collections.Generic;
using System.Text;

using SkiaSharp;

namespace GridEye.Views.Crop
{
    class CroppingRectangle
    {
        const float MINIMUM = 50;   // pixels width or height
        const float REC_DIF_MINIMUM = 10;

        SKRect maxRect;             // generally the size of the bitmap
        float? aspectRatio;

        public CroppingRectangle(SKRect maxRect, float? aspectRatio = null)
        {
            this.maxRect = maxRect;
            this.aspectRatio = aspectRatio;

            // Set initial cropping rectangle
            Rect = new SKRect(0.9f * maxRect.Left + 0.1f * maxRect.Right,
                              0.9f * maxRect.Top + 0.1f * maxRect.Bottom,
                              0.1f * maxRect.Left + 0.9f * maxRect.Right,
                              0.1f * maxRect.Top + 0.5f * maxRect.Bottom);
            ARect = new SKRect(maxRect.Left,
                              0.1f * maxRect.Top + 0.7f * maxRect.Bottom,
                              maxRect.Right,
                              maxRect.Bottom);

            // Adjust for aspect ratio
            if (aspectRatio.HasValue)
            {
                SKRect rect = Rect;
                float aspect = aspectRatio.Value;

                if (rect.Width > aspect * rect.Height)
                {
                    float width = aspect * rect.Height;
                    rect.Left = (maxRect.Width - width) / 2;
                    rect.Right = rect.Left + width;
                }
                else
                {
                    float height = rect.Width / aspect;
                    rect.Top = (maxRect.Height - height) / 2;
                    rect.Bottom = rect.Top + height;
                }

                Rect = rect;
            }
        }

        public SKRect Rect { set; get; }
        public SKRect ARect { get; set; }

        public SKPoint[] Corners
        {
            get
            {
                return new SKPoint[]
                {
                    // Top
                    new SKPoint(Rect.Left + (Rect.Right - Rect.Left)/2, Rect.Top),
                    // Bottom
                    new SKPoint(Rect.Left + (Rect.Right - Rect.Left)/2, Rect.Bottom),
                    // Left
                    new SKPoint(Rect.Left, Rect.Top + (Rect.Bottom - Rect.Top)/2),
                    // right
                    new SKPoint(Rect.Right, Rect.Top + (Rect.Bottom - Rect.Top)/2),

                    // Point for bottom area
                    new SKPoint(ARect.Left + (ARect.Right - ARect.Left)/4, ARect.Top),
                    new SKPoint(ARect.Right - (ARect.Right - ARect.Left)/4 , ARect.Top)
                };
            }
        }

        public int HitTest(SKPoint point, float radius)
        {
            SKPoint[] corners = Corners;

            for (int index = 0; index < corners.Length; index++)
            {
                SKPoint diff = point - corners[index];
                
                if ((float)Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y) < radius)
                {
                    return index;
                }
            }

            return -1;
        }

        public void MoveCorner(int index, SKPoint point)
        {
            SKRect rect = Rect;
            SKRect arect = ARect;
            float dif;

            switch (index)
            {
                case 0: // TOP
                    if(point.Y < 10)
                    {
                        break;
                    }
                    rect.Top = Math.Min(Math.Max(point.Y, maxRect.Top), rect.Bottom - MINIMUM);
                    break;
                case 1: // Bottom
                    rect.Bottom = Math.Max(Math.Min(point.Y, maxRect.Bottom), rect.Top + MINIMUM);
                    rect.Bottom = Math.Min(arect.Top - REC_DIF_MINIMUM, rect.Bottom);
                    break;
                case 2: // LEFT
                    if(point.X < 10)
                    {
                        break;
                    }

                    dif = point.X - maxRect.Left;
                    rect.Left = Math.Min(Math.Max(point.X, maxRect.Left), rect.Right - MINIMUM);

                    //// Right to left
                    if (dif >= 0)
                    {
                        rect.Right = Math.Max(Math.Min(maxRect.Right, maxRect.Right - dif), rect.Left + MINIMUM);
                    }
                    else
                    {
                        rect.Right = Math.Max(Math.Min(maxRect.Right, maxRect.Right + dif), rect.Left + MINIMUM);
                    }
                    break;
                case 3: // Right
                    if(point.X > (maxRect.Right - 10))
                    {
                        break; 
                    }
                    dif = point.X - maxRect.Right;
                    rect.Right = Math.Max(Math.Min(point.X, maxRect.Right), rect.Left + MINIMUM);
                    if (dif >= 0)
                    {
                        rect.Left = Math.Max(Math.Min(maxRect.Left - dif, maxRect.Left), rect.Right - MINIMUM);
                    }
                    else
                    {
                        rect.Left = Math.Min(Math.Max(maxRect.Left - dif, maxRect.Left), rect.Right - MINIMUM);
                    }
                    break;
                case 4:
                case 5:
                    arect.Top = Math.Min(Math.Max(point.Y, maxRect.Top), arect.Bottom - MINIMUM);
                    arect.Top = Math.Max(arect.Top, rect.Bottom + REC_DIF_MINIMUM);
                    arect.Top = Math.Max(arect.Top, maxRect.Bottom/2);
                    break;
            }

            // Adjust for aspect ratio
            if (aspectRatio.HasValue)
            {
                float aspect = aspectRatio.Value;

                if (rect.Width > aspect * rect.Height)
                {
                    float width = aspect * rect.Height;

                    switch (index)
                    {
                        case 0:
                        case 3: rect.Left = rect.Right - width; break;
                        case 1:
                        case 2: rect.Right = rect.Left + width; break;
                    }
                }
                else
                {
                    float height = rect.Width / aspect;

                    switch (index)
                    {
                        case 0:
                        case 1: rect.Top = rect.Bottom - height; break;
                        case 2:
                        case 3: rect.Bottom = rect.Top + height; break;
                    }
                }
            }

            Rect = rect;
            ARect = arect;
        }
    }
}
