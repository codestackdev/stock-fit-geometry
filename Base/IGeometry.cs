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
    /// Represents the 3D geometry of entity or body
    /// </summary>
    public interface IGeometry
    {
        /// <summary>
        /// Finds the extreme points in the specified and opposite directions
        /// </summary>
        /// <param name="dir">Direction to get extreme points from</param>
        /// <param name="startPt">First extreme point</param>
        /// <param name="endPt">Second extreme point</param>
        void GetExtremePoints(Vector dir, out Point startPt, out Point endPt);
    }
}
