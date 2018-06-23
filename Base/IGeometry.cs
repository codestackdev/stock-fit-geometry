using CodeStack.Community.StockFit.Base.Math.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.Community.StockFit.Base
{
    public interface IGeometry
    {
        void GetExtremePoints(Vector dir, out Point startPt, out Point endPt);
    }
}
