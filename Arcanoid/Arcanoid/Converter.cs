using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcanoid
{
    static internal class Converter
    {
        public static int PercentToPixels(double percent, int size)
        {
            return (int)Math.Round(size * percent);
        }

        public static double PixelsToPercent(int pixels, int size)
        {
            return pixels / (double)size;
        }
    }
}
