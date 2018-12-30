//**********************
//Stock Master
//Copyright(C) 2018 www.codestack.net
//Product: https://www.codestack.net/labs/solidworks/stock-fit-geometry/
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using CodeStack.Community.StockFit.Stocks.Cylinder;
using CodeStack.Community.StockFit.Sw.Options;
using CodeStack.SwEx.MacroFeature;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CodeStack.Community.StockFit.MVC;
using CodeStack.SwEx.MacroFeature.Data;
using CodeStack.SwEx.MacroFeature.Base;
using CodeStack.SwEx.MacroFeature.Attributes;
using CodeStack.Community.StockFit.Sw.MVC;
using System.Diagnostics;
using CodeStack.Community.StockFit.Sw.Properties;
using CodeStack.SwEx.MacroFeature.Exceptions;

namespace CodeStack.Community.StockFit.Sw
{
    [ComVisible(true)]
    [Guid("47827004-8897-49F5-9C65-5B845DC7F5AC")]
    [ProgId(Id)]
    [Options("CodeStack.RoundStock", swMacroFeatureOptions_e.swMacroFeatureAlwaysAtEnd)]
    [FeatureIcon(typeof(Resources), nameof(Resources.round_stock_icon), "CodeStack\\StockMaster\\Icons")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ISwComFeature))]
    public class RoundStockMacroFeature : MacroFeatureEx<RoundStockFeatureParameters>
    {
        public const string Id = "CodeStack.StockMacroFeature";

        private readonly RoundStockModel m_StockModel;
        private readonly RoundStockController m_Controller;
        
        public RoundStockMacroFeature() : base()
        {
            m_StockModel = ServicesContainer.Instance.GetService<RoundStockModel>();
            m_Controller = ServicesContainer.Instance.GetService<RoundStockController>();
            
            m_Controller.FeatureEditingCompleted += OnFeatureEditingCompleted;
        }

        private void OnFeatureEditingCompleted(RoundStockFeatureParameters parameters,
            IPartDoc part, IFeature feat, IMacroFeatureData featData, bool isOk)
        {
            if (isOk)
            {
                MacroFeatureOutdateState_e state;
                SetParameters(part as IModelDoc2, feat, featData, parameters, out state);
                
                if (state != MacroFeatureOutdateState_e.UpToDate)
                {
                    if (m_Controller.App.SendMsgToUser2("This features is outdated. It is required to replace it. Do you want to replace this feature?",
                        (int)swMessageBoxIcon_e.swMbWarning,
                        (int)swMessageBoxBtn_e.swMbYesNo) == (int)swMessageBoxResult_e.swMbHitYes)
                    {
                        feat = (part as IModelDoc2).FeatureManager
                            .ReplaceComFeature<RoundStockMacroFeature, RoundStockFeatureParameters>(
                            feat, parameters);

                        return;
                    }
                }

                feat.ModifyDefinition(featData, part, null);
            }
            else
            {
                featData.ReleaseSelectionAccess();
            }
        }

        protected override bool OnEditDefinition(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            var featData = feature.GetDefinition() as IMacroFeatureData;

            featData.AccessSelections(model, null);

            m_Controller.ShowPage(GetParameters(feature, featData, model), model as IPartDoc, feature, featData);

            return true;
        }

        protected override MacroFeatureRebuildResult OnRebuild(ISldWorks app, IModelDoc2 model,
            IFeature feature, RoundStockFeatureParameters parameters)
        {
            var cylParams = GetCylinderParams(model, parameters);
            
            //temp
            SetProperties(model, parameters, cylParams);
            //

            parameters.Height = cylParams.Height;
            parameters.Radius = cylParams.Radius;

            var featData = feature.GetDefinition() as IMacroFeatureData;

            MacroFeatureOutdateState_e state;
            SetParameters(model, feature, featData, parameters, out state);

            if (state != MacroFeatureOutdateState_e.UpToDate)
            {
                app.ShowBubbleTooltip("Stock Master",
                    $"'{feature.Name}' feature is outdated. Edit definition of the feature to update",
                    BubbleTooltipPosition_e.TopLeft, Resources.warning_icon);
            }

            if (parameters.CreateSolidBody)
            {
                var body = m_StockModel.CreateCylindricalStock(cylParams);
                return MacroFeatureRebuildResult.FromBody(body, feature.GetDefinition() as IMacroFeatureData);
            }
            else
            {
                return MacroFeatureRebuildResult.FromStatus(true);
            }
        }

        protected override void OnSetDimensions(ISldWorks app, IModelDoc2 model, IFeature feature, 
            MacroFeatureRebuildResult rebuildResult, DimensionDataCollection dims, RoundStockFeatureParameters parameters)
        {
            var stockModel = ServicesContainer.Instance.GetService<RoundStockModel>();

            var cylParams = GetCylinderParams(model, parameters);

            var startPt = new Point(cylParams.Origin.ToArray());
            var heightDir = new Vector(cylParams.Axis.ToArray());
            var endPt = startPt.Move(heightDir, cylParams.Height);

            Vector extrMatDir = null;

            var yVec = new Vector(0, 1, 0);
            if (heightDir.IsSame(yVec))
            {
                extrMatDir = new Vector(1, 0, 0);
            }
            else
            {
                extrMatDir = yVec.Cross(heightDir);
            }

            var startExtraDiamPt = endPt.Move(extrMatDir, cylParams.Radius - parameters.ExtraRadius);
            
            dims[(int)RoundStockFeatureDimensions_e.Radius].SetOrientation(endPt, heightDir);
            dims[(int)RoundStockFeatureDimensions_e.Radius].Dimension.DrivenState = (int)swDimensionDrivenState_e.swDimensionDriven;
            dims[(int)RoundStockFeatureDimensions_e.Radius].Dimension.ReadOnly = true;

            dims[(int)RoundStockFeatureDimensions_e.Height].SetOrientation(startPt, heightDir);
            dims[(int)RoundStockFeatureDimensions_e.Height].Dimension.DrivenState = (int)swDimensionDrivenState_e.swDimensionDriven;
            dims[(int)RoundStockFeatureDimensions_e.Height].Dimension.ReadOnly = true;

            dims[(int)RoundStockFeatureDimensions_e.ExtraRadius].SetOrientation(startExtraDiamPt, extrMatDir);
        }
        
        private CylinderParams GetCylinderParams(IModelDoc2 model,
            RoundStockFeatureParameters parameters)
        {
            var cylParams = m_StockModel.GetCylinderParameters(model as IPartDoc, parameters.Direction,
                parameters.ConcenticWithCylindricalFace, parameters.StockStep, parameters.ExtraRadius);

            return cylParams;
        }
        
        private static void SetProperties(object modelDoc, RoundStockFeatureParameters param, CylinderParams cylParams)
        {
            var setPrpValFunc = new Action<IModelDoc2, string, string, string>((doc, prpName, prpVal, conf) => 
            {
                var prpMgr = doc.Extension.CustomPropertyManager[conf];
                prpMgr.Add2(prpName, (int)swCustomInfoType_e.swCustomInfoText, prpVal);
                prpMgr.Set2(prpName, prpVal);
            });

            var metersToInch = new Func<double, double>((m) => System.Math.Round(m * 39.37007874, 3));

            var model = modelDoc as IModelDoc2;
            var activeConf = model.ConfigurationManager.ActiveConfiguration.Name;

            setPrpValFunc.Invoke(model, "StockVisible", Convert.ToInt32(param.CreateSolidBody).ToString(), activeConf);
            setPrpValFunc.Invoke(model, "StockDiameter", metersToInch(cylParams.Radius * 2).ToString(), activeConf);
            setPrpValFunc.Invoke(model, "StockLength", metersToInch(cylParams.Height).ToString(), activeConf);
        }
    }
}
