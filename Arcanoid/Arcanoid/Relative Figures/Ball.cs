﻿using System;
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

        public Vector Speed { get; set; } = new Vector(0.01, -0.01);

        public void Move()
        {
            RelativeHorizontalPos += Speed.X;
            RelativeVerticalPos += Speed.Y;
        }

        public override Size GetSize(Size size)
        {
            return new Size(PercentToPixels(RelativeWidth, size.Height), PercentToPixels(RelativeHeight, size.Height));
        }
    }
}