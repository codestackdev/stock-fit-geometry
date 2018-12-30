//**********************
//Stock Master
//Copyright(C) 2018 www.codestack.net
//Product: https://www.codestack.net/labs/solidworks/stock-fit-geometry/
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using CodeStack.Community.StockFit.MVC;
using CodeStack.Community.StockFit.Sw.Options;
using CodeStack.Community.StockFit.Sw.Properties;
using CodeStack.SwEx.PMPage;
using CodeStack.SwEx.PMPage.Attributes;
using CodeStack.SwEx.PMPage.Base;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using CodeStack.SwEx.PMPage.Controls;
using System.Diagnostics;
using CodeStack.SwEx.AddIn.Attributes;

namespace CodeStack.Community.StockFit.Sw.MVC
{
    [PageOptions(typeof(Resources), nameof(Resources.round_stock_icon),
    swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton |
            swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton
            | swPropertyManagerPageOptions_e.swPropertyManagerOptions_WhatsNew)]
    [Message("Select cylindrical face or plane to generate the round stock", "Stock Feature")]
    [Help("https://www.codestack.net/labs/solidworks/stock-fit-geometry/",
    "https://www.codestack.net/labs/solidworks/stock-fit-geometry/whats-new")]
    [DisplayName("Round Stock")]
    public class RoundStockViewModel
    {
        public static RoundStockViewModel FromParameters(RoundStockFeatureParameters parameters)
        {
            var step = parameters.StockStep;
            var customStep = false;
            StockSteps_e defStep = StockSteps_e.Step0;

            if (step == 0)
            {
                defStep = StockSteps_e.Step0;
            }
            else if (step == 0.0015875)
            {
                defStep = StockSteps_e.Step1_16;
            }
            else if (step == 0.003175)
            {
                defStep = StockSteps_e.Step1_8;
            }
            else
            {
                customStep = true;
            }

            return new RoundStockViewModel()
            {
                Conditions = new ConditionOptions()
                {
                    ConcentricWithCylindricalFace = parameters.ConcenticWithCylindricalFace,
                    CreateSolidBody = parameters.CreateSolidBody,
                    Direction = parameters.Direction
                },
                Rounding = new RoundingOptions()
                {
                    UseCustomStep = customStep,
                    StockStep = defStep,
                    CustomStep = step
                },
                ExtraMaterial = new ExtraMaterialOptions()
                {
                    AdditionalRadius = parameters.ExtraRadius
                }
            };
        }

        public void WriteToParameters(RoundStockFeatureParameters parameters)
        {
            parameters.Direction = Conditions.Direction;
            parameters.CreateSolidBody = Conditions.CreateSolidBody;
            parameters.ConcenticWithCylindricalFace = Conditions.ConcentricWithCylindricalFace;
            parameters.ExtraRadius = ExtraMaterial.AdditionalRadius;

            double stockStep = 0;

            if (Rounding.UseCustomStep)
            {
                stockStep = Rounding.CustomStep;
            }
            else
            {
                switch (Rounding.StockStep)
                {
                    case StockSteps_e.Step0:
                        stockStep = 0;
                        break;

                    case StockSteps_e.Step1_16:
                        stockStep = 0.0015875;
                        break;

                    case StockSteps_e.Step1_8:
                        stockStep = 0.003175;
                        break;
                }
            }

            parameters.StockStep = stockStep;
        }

        public enum RoundStepControls_e
        {
            StockStep,
            UseCustomStep,
            CustomStep
        }

        public class StockStepDependencyHandler : DependencyHandler
        {
            protected override void UpdateControlState(IPropertyManagerPageControlEx control, IPropertyManagerPageControlEx[] parents)
            {
                if (parents != null && parents.Length == 1
                    && RoundStepControls_e.UseCustomStep.Equals(parents.First().Tag))
                {
                    var useCustom = (bool)parents.First().GetValue();

                    switch ((RoundStepControls_e)control.Tag)
                    {
                        case RoundStepControls_e.CustomStep:
                            control.Enabled = useCustom;
                            break;

                        case RoundStepControls_e.StockStep:
                            control.Enabled = !useCustom;
                            break;
                    }
                }
                else
                {
                    Debug.Assert(false, "Invalid dependency. This handler should only be applied to rounding step controls");
                }
            }
        }

        public class RoundingOptions
        {
            [Description("Predefined rounding step")]
            [ControlTag(RoundStepControls_e.StockStep)]
            [DependentOn(typeof(StockStepDependencyHandler), RoundStepControls_e.UseCustomStep)]
            [ControlAttribution(typeof(Resources), nameof(Resources.round_step_predefined))]
            public StockSteps_e StockStep { get; set; }

            [DisplayName("Use custom round step")]
            [ControlTag(RoundStepControls_e.UseCustomStep)]
            public bool UseCustomStep { get; set; }

            [Description("Custom rounding step")]
            [NumberBoxOptions(swNumberboxUnitType_e.swNumberBox_Length, 0, 1000, 0.001,
                true, 0.01, 0.0005, swPropMgrPageNumberBoxStyle_e.swPropMgrPageNumberBoxStyle_Thumbwheel)]
            [DependentOn(typeof(StockStepDependencyHandler), RoundStepControls_e.UseCustomStep)]
            [ControlTag(RoundStepControls_e.CustomStep)]
            [ControlAttribution(typeof(Resources), nameof(Resources.round_step_custom))]
            public double CustomStep { get; set; }
        }

        public class ExtraMaterialOptions
        {
            [Description("Add additional material to cylinder diameter")]
            [NumberBoxOptions(swNumberboxUnitType_e.swNumberBox_Length, 0, 1000, 0.001,
                true, 0.01, 0.0005, swPropMgrPageNumberBoxStyle_e.swPropMgrPageNumberBoxStyle_Thumbwheel)]
            [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_Diameter)]
            public double AdditionalRadius { get; set; }
        }

        public class ConditionOptions
        {
            [SelectionBox(swSelectType_e.swSelFACES, swSelectType_e.swSelDATUMPLANES)]
            [DisplayName("Reference geometry defining the direction of round stock")]
            [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_SelectFace)]
            public object Direction { get; set; }

            [DisplayName("Concentric with cylindrical face")]
            [Description("Specifies if the round stock needs to be concentric with selected cylindrical face")]
            public bool ConcentricWithCylindricalFace { get; set; } = true;

            [DisplayName("Create Solid Body")]
            [Description("Specifies if solid body needs to be created")]
            public bool CreateSolidBody { get; set; } = true;
        }
        
        public ConditionOptions Conditions { get; set; }
        
        public RoundingOptions Rounding { get; set; }

        [DisplayName("Extra Material")]
        public ExtraMaterialOptions ExtraMaterial { get; set; }
    }

    public enum StockSteps_e
    {
        [SwEx.Common.Attributes.Title("0")]
        Step0, //0

        [SwEx.Common.Attributes.Title("1/16")]
        Step1_16,//0.0015875

        [SwEx.Common.Attributes.Title("1/8")]
        Step1_8//0.003175
    }

    public class RoundStockView : PropertyManagerPageEx<RoundStockViewHandler, RoundStockViewModel>
    {
        public RoundStockView(ISldWorks app) : base(app)
        {
        }
    }
}
