using Microsoft.Win32;
using System.Windows;
using System.Drawing;
using System.Runtime.InteropServices;
using System;


namespace MedianFilterProject
{
    class MedianFilter
    {
        public static Bitmap FilterBitmap(Bitmap bitmap)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var oldColor = bitmap.GetPixel(x, y);
                    var newColor = DarkenColor(oldColor, 0.5);
                    bitmap.SetPixel(x, y, newColor);
                }
            }
            return bitmap;
        }

        public static Color DarkenColor(Color inColor, double lightenAmount)
        {
            return Color.FromArgb(
              inColor.A,
              (int)Math.Max(0, inColor.R - 255 * lightenAmount),
              (int)Math.Max(0, inColor.G - 255 * lightenAmount),
              (int)Math.Max(0, inColor.B - 255 * lightenAmount));
        }
    }


}

    
