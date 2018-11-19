//**********************
//Stock Master
//Copyright(C) 2018 www.codestack.net
//Product: https://www.codestack.net/labs/solidworks/stock-fit-geometry/
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using CodeStack.Community.StockFit.Sw.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xarial.AppLaunchKit.Services.UserSettings.Data;

namespace CodeStack.Community.StockFit.Sw.Services
{
    public class RoundStockFeatureParametersBackwardCompatibility : BaseUserSettingsVersionsTransformer<RoundStockFeatureParameters>
    {
        public RoundStockFeatureParametersBackwardCompatibility()
        {
            //TODO: call Add method to add compatibility transformers
        }
    }
}
