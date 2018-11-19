//**********************
//Stock Master
//Copyright(C) 2018 www.codestack.net
//Product: https://www.codestack.net/labs/solidworks/stock-fit-geometry/
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using CodeStack.Community.StockFit.Base.Math.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.Community.StockFit.Base
{
    /// <summary>
    /// Base service extracting the stock parameters
    /// </summary>
    /// <typeparam name="TParams">Parameters of the specific stock element</typeparam>
    public interface IStockFitExtractor<TParams>
    {
        /// <summary>
        /// Returns the stock parameters from the geometry
        /// </summary>
        /// <param name="geom">Geometry to get parameters from</param>
        /// <param name="heightDirection">Direction of height</param>
        /// <returns>Stock parameters</returns>
        TParams GetStockParameters(IGeometry geom, Vector heightDirection);
    }
}
