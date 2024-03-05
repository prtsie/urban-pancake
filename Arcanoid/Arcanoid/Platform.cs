using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Arcanoid.Converter;

namespace Arcanoid
{
    internal class Platform(Rectangle rect, Size size)
    {
        public double RelativeWidth { get; private set; } = PixelsToPercent(rect.Width, size.Width);

        public double RelativeHeight { get; private set; } = PixelsToPercent(rect.Height, size.Height);

        public double RelativeHorizontalPos { get; set; } = PixelsToPercent(rect.Location.X, size.Width);

        public double RelativeVerticalPos { get; set; } = PixelsToPercent(rect.Location.Y, size.Height);

        public Point GetPosition(Size size)
        {
            return new Point(PercentToPixels(RelativeHorizontalPos, size.Width), PercentToPixels(RelativeVerticalPos, size.Height));
        }

        public Size GetSize(Size size)
        {
            return new Size(PercentToPixels(RelativeWidth, size.Width), PercentToPixels(RelativeHeight, size.Height));
        }

        public Rectangle GetRectangle(Size size)
        {
            return new Rectangle(GetPosition(size), GetSize(size));
        }
    }
}
