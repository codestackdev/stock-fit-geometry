using CodeStack.Community.StockFit.Base.Math.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.Community.StockFit.Base
{
    public interface IStockFitExtractor<TParams>
    {
        TParams GetStockParameters(IEnumerable<Point> points, Vector heightDirection);
    }
}
