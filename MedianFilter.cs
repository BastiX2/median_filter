using Microsoft.Win32;
using System.Windows;
using System.Drawing;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

namespace MedianFilterProject
{
    class MedianFilter
    {


        public static Bitmap FilterBitmap(Bitmap bitmap, int filterStrength)
        {
            Bitmap filteredBitmap = (Bitmap)bitmap.Clone();
            var termsList = new List<Color>();
            Color c;

            // Durchläuft alle Pixel in der Bitmap
            for (int i = 0; i <= filteredBitmap.Width - filterStrength; i++)
            {
                for (int j = 0; j <= filteredBitmap.Height - filterStrength; j++)
                {

                    // Durchläuft X pixel um Zielpixel
                    for (int x = i; x < i + filterStrength; x++)
                    {
                        for (int y = j; y < j + filterStrength; y++)
                        {
                            // Fügt die Pixel der Liste hinzu
                            c = filteredBitmap.GetPixel(x, y);
                            termsList.Add(c);
                        }
                    }
                    // Pixel werden in Array gespeichert
                    var terms = new List<Color>(termsList);
                    //Liste wird geleert
                    termsList.Clear();

                    terms.Sort((color1, color2) =>
                    (color1.GetBrightness()).CompareTo(color2.GetBrightness()));

                    c = terms[terms.Count/2];

                    filteredBitmap.SetPixel(i + 1, j + 1, c);
                }
            }

            return filteredBitmap;
        }

    }

}

    
