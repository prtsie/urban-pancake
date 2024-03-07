using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Arcanoid.Measures.Converter;

namespace Arcanoid
{
    internal class RelativeObject(double relativeX, double relativeY, double relativeWidth, double relativeHeight)
    {
        public double RelativeWidth { get; set; } = relativeWidth;

        public double RelativeHeight { get; set; } = relativeHeight;

        public double RelativeHorizontalPos { get; set; } = relativeX;

        public double RelativeVerticalPos { get; set; } = relativeY;

        public Point GetPosition(Size size)
        {
            return new Point(PercentToPixels(RelativeHorizontalPos, size.Width), PercentToPixels(RelativeVerticalPos, size.Height));
        }

        public virtual Size GetSize(Size size)
        {
            return new Size(PercentToPixels(RelativeWidth, size.Width), PercentToPixels(RelativeHeight, size.Height));
        }

        public Rectangle GetRectangle(Size size)
        {
            return new Rectangle(GetPosition(size), GetSize(size));
        }
    }
}
