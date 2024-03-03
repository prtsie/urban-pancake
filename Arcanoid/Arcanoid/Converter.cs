using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcanoid
{
    static internal class Converter
    {
        public static int PercentToPixels(double percent, double size)
        {
            return (int)Math.Round(size * percent);
        }
    }
}
