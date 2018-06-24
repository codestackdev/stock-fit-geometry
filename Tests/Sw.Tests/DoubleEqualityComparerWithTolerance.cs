using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sw.Tests
{
    public class DoubleEqualityComparerWithTolerance : IEqualityComparer<double>
    {
        const double TOL = 1E-10;

        public bool Equals(double x, double y)
        {
            return Math.Abs(x - y) < TOL;
        }

        public int GetHashCode(double obj)
        {
            return 0;
        }
    }
}
