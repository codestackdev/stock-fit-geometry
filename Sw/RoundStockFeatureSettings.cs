using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.Community.StockFit.Sw
{
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
