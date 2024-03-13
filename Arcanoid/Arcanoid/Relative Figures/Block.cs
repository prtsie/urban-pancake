using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcanoid
{
    internal sealed class Block(double relativeX, double relativeY, double relativeWidth, double relativeHeight) : RelativeObject(relativeX, relativeY, relativeWidth, relativeHeight)
    {
        public bool IsBroken { get; set; }

        public Brush Brush { get; set; } = Brushes.White;
    }
}
