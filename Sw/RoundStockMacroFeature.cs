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
    [Icon(typeof(Resources), nameof(Resources.round_stock_icon), "CodeStack\\StockMaster\\Icons")]
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

        private void OnFeatureEditingCompleted(RoundStockFeatureParameters parameters, IPartDoc part, IFeature feat, bool isOk)
        {
            if (isOk)
            {
                var featData = feat.GetDefinition() as IMacroFeatureData;

                bool isOutdated;
                SetParameters(part as IModelDoc2, feat, parameters, out isOutdated);
                
                if (isOutdated)
                {
                    if (m_Controller.App.SendMsgToUser2("This features is an older version. It is required to replace it. Do you want to replace this feature?",
                        (int)swMessageBoxIcon_e.swMbWarning,
                        (int)swMessageBoxBtn_e.swMbYesNo) == (int)swMessageBoxResult_e.swMbHitYes)
                    {
                        feat = (part as IModelDoc2).FeatureManager.ReplaceComFeature<RoundStockMacroFeature>(feat);
                        return;
                    }
                }

                feat.ModifyDefinition(featData, part, null);
            }
            else
            {
                (feat.GetDefinition() as IMacroFeatureData).ReleaseSelectionAccess();
            }

            EnsureNotRolledBack(feat, part);
        }

        private void EnsureNotRolledBack(IFeature feat, IPartDoc part)
        {
            if (feat != null)
            {
                if (feat.IsRolledBack())
                {
                    Debug.Assert(false, "by some reasons roll back state doesn't go");
                    (part as IModelDoc2).FeatureManager.EditRollback(
                        (int)swMoveRollbackBarTo_e.swMoveRollbackBarToEnd, null);
                }
            }
        }

        protected override bool OnEditDefinition(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            (feature.GetDefinition() as IMacroFeatureData).AccessSelections(model, null);

            m_Controller.ShowPage(GetParameters(feature, model), model as IPartDoc, feature);

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

            bool isOutdated;
            SetParameters(model, feature, parameters, out isOutdated);

            if (isOutdated)
            {
                //TODO: display warning
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
            DimensionDataCollection dims, RoundStockFeatureParameters parameters)
        {
            var stockModel = ServicesContainer.Instance.GetService<RoundStockModel>();

            var cylParams = GetCylinderParams(model, parameters);

            var startPt = new Point(cylParams.Origin.ToArray());
            var heightDir = new Vector(cylParams.Axis.ToArray());
            var endPt = startPt.Move(heightDir, cylParams.Height);
            
            Vector diamDir = null;

            var yVec = new Vector(0, 1, 0);
            if (heightDir.IsSame(yVec))
            {
                diamDir = new Vector(1, 0, 0);
            }
            else
            {
                diamDir = yVec.Cross(heightDir);
            }

            var startExtraDiamPt = endPt.Move(diamDir, cylParams.Radius - parameters.ExtraRadius);

            var diamExtVec = diamDir.Cross(heightDir);

            dims[(int)RoundStockFeatureDimensions_e.Radius].Dimension.SetDirection(endPt, diamDir, cylParams.Radius, diamExtVec);
            dims[(int)RoundStockFeatureDimensions_e.Radius].Dimension.DrivenState = (int)swDimensionDrivenState_e.swDimensionDriven;
            dims[(int)RoundStockFeatureDimensions_e.Radius].Dimension.ReadOnly = true;

            dims[(int)RoundStockFeatureDimensions_e.Height].Dimension.SetDirection(startPt, heightDir, cylParams.Height);
            dims[(int)RoundStockFeatureDimensions_e.Height].Dimension.DrivenState = (int)swDimensionDrivenState_e.swDimensionDriven;
            dims[(int)RoundStockFeatureDimensions_e.Height].Dimension.ReadOnly = true;

            dims[(int)RoundStockFeatureDimensions_e.ExtaRadius].Dimension.SetDirection(
                startExtraDiamPt, diamDir, parameters.ExtraRadius);
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
