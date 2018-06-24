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
    /// <summary>
    /// Represents the application settings
    /// </summary>
    public class RoundStockFeatureSettings
    {
        public Dictionary<string, double> StockSteps { get; set; }

        public RoundStockFeatureSettings()
        {
            StockSteps = new Dictionary<string, double>()
            {
                { "0", 0 },
                { "1/16", 0.0015875 },
                { "1/8", 0.003175 }
            };
        }
    }
}
