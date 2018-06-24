//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using CodeStack.Community.StockFit.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeStack.Community.StockFit.Base.Math.Structures;
using SolidWorks.Interop.sldworks;

namespace CodeStack.Community.StockFit.Sw
{
    public class SolidBodyGeometry : IGeometry
    {
        private IBody2 m_Body;

        public SolidBodyGeometry(IBody2 body)
        {
            m_Body = body;
        }

        public void GetExtremePoints(Vector dir, out Point startPt, out Point endPt)
        {
            double x;
            double y;
            double z;

            m_Body.GetExtremePoint(dir.X, dir.Y, dir.Z, out x, out y, out z);
            startPt = new Point(x, y, z);

            m_Body.GetExtremePoint(-dir.X, -dir.Y, -dir.Z, out x, out y, out z);
            endPt = new Point(x, y, z);
        }
    }
}
