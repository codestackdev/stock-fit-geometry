//**********************
//Stock Master
//Copyright(C) 2018 www.codestack.net
//Product: https://www.codestack.net/labs/solidworks/stock-fit-geometry/
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using CodeStack.Community.StockFit.Sw.Properties;
using CodeStack.SwEx.MacroFeature.Attributes;
using CodeStack.SwEx.MacroFeature.Base;
using CodeStack.SwEx.MacroFeature.Data;
using CodeStack.SwEx.MacroFeature.Mocks;
using CodeStack.SwEx.MacroFeature.Placeholders;
using CodeStack.SwEx.PMPage.Attributes;
using Newtonsoft.Json;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using Xarial.AppLaunchKit.Services.UserSettings.Attributes;

namespace CodeStack.Community.StockFit.Sw.Options
{
    public enum RoundStockFeatureDimensions_e
    {
        Radius = 0,
        Height = 1,
        ExtraRadius = 2
    }

    public class RoundStockFeatureParametersVersionConverter : ParametersVersionConverter
    {   
        private class Converter_0_0To1_0 : ParameterConverter
        {
            public override IDisplayDimension[] ConvertDisplayDimensions(IModelDoc2 model, IFeature feat, IDisplayDimension[] dispDims)
            {
                return new IDisplayDimension[]
                {
                    dispDims[0],
                    dispDims[1],
                    new DisplayDimensionPlaceholder()
                };
            }

            public override Dictionary<string, string> ConvertParameters(IModelDoc2 model, IFeature feat, Dictionary<string, string> parameters)
            {
                string stockStep;
                if (parameters.TryGetValue(nameof(RoundStockFeatureParameters.StockStep), out stockStep))
                {
                    double val = 0;

                    switch (stockStep)
                    {
                        case "":
                        case "0":
                            val = 0;
                            break;

                        case "1/16":
                            val = 0.0015875;
                            break;

                        case "1/8":
                            val = 0.003175;
                            break;
                    }

                    parameters[nameof(RoundStockFeatureParameters.StockStep)] = val.ToString();
                }

                return parameters;
            }

            public override object[] ConvertSelections(IModelDoc2 model, IFeature feat, object[] selection)
            {
                return base.ConvertSelections(model, feat, selection);
            }

            public override IBody2[] ConvertEditBodies(IModelDoc2 model, IFeature feat, IBody2[] editBodies)
            {
                return base.ConvertEditBodies(model, feat, editBodies);
            }
        }

        private class Converter_1_0To1_1 : ParameterConverter
        {
            public override Dictionary<string, string> ConvertParameters(IModelDoc2 model, IFeature feat,
                Dictionary<string, string> parameters)
            {
                var dirIndex = 0;
                var scopeBodyIndex = 1;

                parameters.Add(nameof(RoundStockFeatureParameters.Direction), dirIndex.ToString());
                parameters.Add(nameof(RoundStockFeatureParameters.ScopeBody), scopeBodyIndex.ToString());
                parameters.Add(nameof(RoundStockFeatureParameters.Radius), ((int)RoundStockFeatureDimensions_e.Radius).ToString());
                parameters.Add(nameof(RoundStockFeatureParameters.Height), ((int)RoundStockFeatureDimensions_e.Height).ToString());
                parameters.Add(nameof(RoundStockFeatureParameters.ExtraRadius), ((int)RoundStockFeatureDimensions_e.ExtraRadius).ToString());

                return parameters;
            }
        }

        public RoundStockFeatureParametersVersionConverter()
        {
            Add(new Version("1.0"), new Converter_0_0To1_0());
            Add(new Version("1.1"), new Converter_1_0To1_1());
        }
    }

    //TODO: make settings a separate class and separate the macro feature parameter from here
    [UserSettingVersion("1.0")]
    [ParametersVersion("1.1", typeof(RoundStockFeatureParametersVersionConverter))]
    public class RoundStockFeatureParameters
    {
        [ParameterSelection]
        [IgnoreDataMember, XmlIgnore, JsonIgnore]
        public object Direction { get; set; }

        [ParameterSelection]
        [IgnoreDataMember, XmlIgnore, JsonIgnore]
        public IBody2 ScopeBody { get; set; }

        public bool ConcenticWithCylindricalFace { get; set; } = true;
        public bool CreateSolidBody { get; set; } = true;
        public double StockStep { get; set; }

        [ParameterDimension(swDimensionType_e.swRadialDimension)]
        [IgnoreDataMember, XmlIgnore, JsonIgnore]
        public double Radius { get; set; }

        [ParameterDimension(swDimensionType_e.swLinearDimension)]
        [IgnoreDataMember, XmlIgnore, JsonIgnore]
        public double Height { get; set; }

        [ParameterDimension(swDimensionType_e.swLinearDimension)]
        public double ExtraRadius { get; set; }
    }
}
