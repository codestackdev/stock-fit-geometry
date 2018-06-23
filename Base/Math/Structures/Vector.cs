using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.Community.StockFit.Base.Math.Structures
{
    public class Vector : Point
    {
        public Vector(double x, double y, double z) : base(x, y, z)
        {
        }

        public Vector(double[] dir) : base(dir)
        {
        }

        public bool IsSame(Vector vec)
        {
            if (vec == null)
            {
                throw new ArgumentNullException(nameof(vec));
            }

            return IsSame(vec.X, vec.Y, vec.Z);
        }
    }
}
