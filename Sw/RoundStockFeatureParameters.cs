//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.Community.StockFit.Sw
{
    public class RoundStockFeatureParameters
    {
        public object Direction { get; set; }
        //public double Offset { get; set; }
        public bool ConcenticWithCylindricalFace { get; set; }
        public bool CreateSolidBody { get; set; }
        public string StockStep { get; set; }
    }
}
