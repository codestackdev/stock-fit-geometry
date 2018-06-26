//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using Newtonsoft.Json;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace CodeStack.Community.StockFit.Sw.Options
{
    public class RoundStockFeatureParameters
    {
        [MacroFeatureParameterSelection(0)]
        [IgnoreDataMember, XmlIgnore, JsonIgnore]
        public object Direction { get; set; }

        [MacroFeatureParameterSelection(1)]
        [IgnoreDataMember, XmlIgnore, JsonIgnore]
        public IBody2 ScopeBody { get; set; }

        public bool ConcenticWithCylindricalFace { get; set; } = true;
        public bool CreateSolidBody { get; set; } = true;
        public string StockStep { get; set; }
    }
}
