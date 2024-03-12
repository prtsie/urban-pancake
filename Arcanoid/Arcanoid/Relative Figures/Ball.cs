using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arcanoid.Measures;
using static Arcanoid.Measures.Converter;

namespace Arcanoid
{
    internal class Ball(double size, double relativeX, double relativeY) : RelativeObject(relativeX, relativeY, size, size)
    {

        public Vector Speed { get; set; } = new Vector(0.0055, -0.0055);

        public void Move()
        {
            RelativeHorizontalPos += Speed.X;
            RelativeVerticalPos += Speed.Y;
        }

        public override Size GetSize(Size relativeTo)
        {
            var size = PercentToPixels(RelativeWidth, relativeTo.Height);
            return new Size(size, size);
        }
    }
}
