//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

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

        public bool IsSame(Vector vec, bool normilize = true)
        {
            if (vec == null)
            {
                throw new ArgumentNullException(nameof(vec));
            }

            if (normilize)
            {
                var thisLen = GetLength();
                var thisNorm = new Vector(X / thisLen, Y / thisLen, Z / thisLen);

                var otherLen = vec.GetLength();
                var otherNorm = new Vector(vec.X / otherLen, vec.Y / otherLen, vec.Z / otherLen);

                return thisNorm.IsSame(otherNorm.X, otherNorm.Y, otherNorm.Z)
                    || thisNorm.IsSame(-otherNorm.X, -otherNorm.Y, -otherNorm.Z);
            }
            else
            {
                return IsSame(vec.X, vec.Y, vec.Z);
            }
        }

        private double GetLength()
        {
            return System.Math.Sqrt(System.Math.Pow(X, 2) 
                + System.Math.Pow(Y, 2) 
                + System.Math.Pow(Z, 2));
        }
    }
}
