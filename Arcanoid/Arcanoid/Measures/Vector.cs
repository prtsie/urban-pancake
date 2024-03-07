using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcanoid.Measures
{
    internal struct Vector(double x, double y)
    {
        public double X { get; set; } = x;

        public double Y { get; set; } = y;


        public readonly double Length => Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
    }
}
