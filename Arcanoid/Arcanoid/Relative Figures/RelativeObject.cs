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

        public Point GetPosition(Size relativeTo)
        {
            return new Point(PercentToPixels(RelativeHorizontalPos, relativeTo.Width), PercentToPixels(RelativeVerticalPos, relativeTo.Height));
        }

        public virtual Size GetSize(Size relativeTo)
        {
            return new Size(PercentToPixels(RelativeWidth, relativeTo.Width), PercentToPixels(RelativeHeight, relativeTo.Height));
        }

        public Rectangle GetRectangle(Size relativeTo)
        {
            return new Rectangle(GetPosition(relativeTo), GetSize(relativeTo));
        }
    }
}
