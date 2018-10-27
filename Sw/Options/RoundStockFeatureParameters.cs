//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using CodeStack.SwEx.MacroFeature.Attributes;
using Newtonsoft.Json;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace CodeStack.Community.StockFit.Sw.Options
{
    public enum RoundStockFeatureDimensions_e
    {
        Radius = 0,
        Height = 1
    }

    public class RoundStockFeatureParameters
    {
        [ParameterSelection(0)]
        [IgnoreDataMember, XmlIgnore, JsonIgnore]
        public object Direction { get; set; }

        [ParameterSelection(1)]
        [IgnoreDataMember, XmlIgnore, JsonIgnore]
        public IBody2 ScopeBody { get; set; }

        public bool ConcenticWithCylindricalFace { get; set; } = true;
        public bool CreateSolidBody { get; set; } = true;
        public string StockStep { get; set; }

        [ParameterDimension(swDimensionType_e.swRadialDimension, (int)RoundStockFeatureDimensions_e.Radius)]
        [IgnoreDataMember, XmlIgnore, JsonIgnore]
        public double Radius { get; set; }

        [ParameterDimension(swDimensionType_e.swLinearDimension, (int)RoundStockFeatureDimensions_e.Height)]
        [IgnoreDataMember, XmlIgnore, JsonIgnore]
        public double Height { get; set; }
    }
}
