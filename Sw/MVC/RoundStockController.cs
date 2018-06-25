//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using CodeStack.Community.StockFit.MVC;
using CodeStack.Community.StockFit.Stocks.Cylinder;
using CodeStack.Community.StockFit.Sw.Options;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorksTools.File;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeStack.Community.StockFit.Sw.MVC
{
    /// <summary>
    /// Controller which manages view <see cref="RoundStockView"/>  and model <see cref="RoundStockModel"/> 
    /// </summary>
    public class RoundStockController : IDisposable
    {
        private RoundStockView m_View;
        private IFeature m_Feat;
        private IPartDoc m_Part;

        private IBody2 m_TempBody;

        private RoundStockModel m_StockTool;

        private ISldWorks m_App;

        private RoundStockFeatureSettings m_Setts;

        public RoundStockController(ISldWorks app, RoundStockView view,
            RoundStockModel stockModel, RoundStockFeatureSettings setts)
        {
            m_App = app;
            m_View = view;
            m_StockTool = stockModel;
            m_Setts = setts;

            m_View.ParametersChanged += OnParametersChanged;
            m_View.Closing += OnPageClosing;
            m_View.Closed += OnPageClosed;
        }

        private void OnPageClosing(bool isOk, RoundStockFeatureParameters par)
        {
            if (isOk)
            {
                Exception err;

                if (CreateBody(par, out err) == null)
                {
                    m_App.ShowBubbleTooltipAt2(0, 0,
                                (int)swArrowPosition.swArrowLeftTop, "Error", err.Message,
                                (int)swBitMaps.swBitMapTreeError, "", "", 0, 0, "", "");

                    var S_FALSE = 1;

                    throw new COMException(err.Message, S_FALSE);
                }
            }
        }

        private void OnParametersChanged(RoundStockFeatureParameters par)
        {
            ShowPreview(par);
        }

        private void ShowPreview(RoundStockFeatureParameters par)
        {
            DisposeTempBody();
            Exception err;
            m_TempBody = CreateBody(par, out err);

            if (m_TempBody != null)
            {
                const int COLORREF_YELLOW = 65535;

                m_TempBody.Display3(m_Part, COLORREF_YELLOW,
                    (int)swTempBodySelectOptions_e.swTempBodySelectOptionNone);

                m_TempBody.MaterialPropertyValues2 = new double[] { 1, 1, 0, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5 };

                (m_Part as IModelDoc2).GraphicsRedraw2();
            }
        }

        private IBody2 CreateBody(RoundStockFeatureParameters par, out Exception err)
        {
            err = null;

            try
            {
                CylinderParams cylParams;
                var step = m_Setts.StockSteps.FirstOrDefault(s => s.Key == par.StockStep).Value;

                return m_StockTool.CreateCylindricalStock(m_Part, par.Direction,
                    par.ConcenticWithCylindricalFace, step, out cylParams);
            }
            catch(Exception ex)
            {
                err = ex;

                return null;
            }
        }

        private void OnPageClosed(bool isOk, RoundStockFeatureParameters par)
        {
            DisposeTempBody();

            if (isOk)
            {
                var paramNames = new string[] 
                {
                    nameof(par.CreateSolidBody),
                    nameof(par.ConcenticWithCylindricalFace),
                    nameof(par.StockStep)
                };

                var paramTypes = new int[] 
                {
                    (int)swMacroFeatureParamType_e.swMacroFeatureParamTypeInteger,
                    (int)swMacroFeatureParamType_e.swMacroFeatureParamTypeInteger,
                    (int)swMacroFeatureParamType_e.swMacroFeatureParamTypeString
                };

                var paramValues = new string[] 
                {
                    Convert.ToString(par.CreateSolidBody),
                    Convert.ToString(par.ConcenticWithCylindricalFace),
                    par.StockStep
                };

                var scopeBody = m_StockTool.GetScopeBody(m_Part, par.Direction);

                if (m_Feat == null)
                {
                    (m_Part as IModelDoc2).ClearSelection2(true);

                    var selRes = (m_Part as IModelDoc2).SelectDispatches(
                        false, null, par.Direction, scopeBody);

                    Debug.Assert(selRes);

                    string icon = Path.Combine(Path.GetDirectoryName(
                        typeof(SwStockFitGeometryAddIn).Assembly.Location),
                        "Icons\\FeatureIcon.bmp");
                    
                    var icons = new string[] { icon, icon, icon };

                    var dimTypes = new int[] 
                    {
                        (int)swDimensionType_e.swRadialDimension,
                        (int)swDimensionType_e.swLinearDimension
                    };
                    var dimValues = new double[] { 0, 0 };

                    (m_Part as IModelDoc2).FeatureManager.InsertMacroFeature3("CodeStack.RoundStock",
                        RoundStockMacroFeature.Id, null, paramNames, paramTypes,
                        paramValues, dimTypes, dimValues, null, icons,
                        (int)swMacroFeatureOptions_e.swMacroFeatureAlwaysAtEnd);
                }
                else
                {
                    var featData = m_Feat.GetDefinition() as IMacroFeatureData;

                    featData.SetParameters(paramNames, paramTypes, paramValues);

                    featData.SetSelections2(new DispatchWrapper[]
                    {
                        new DispatchWrapper(par.Direction),
                        new DispatchWrapper(scopeBody)
                    }, new int[] { 0, 0 }, new IView[2] { null, null });

                    var modRes = m_Feat.ModifyDefinition(featData, m_Part as IModelDoc2, null);

                    Debug.Assert(modRes);
                }
            }
            else
            {
                if (m_Feat != null)
                {
                    (m_Feat.GetDefinition() as IMacroFeatureData).ReleaseSelectionAccess();
                }
            }

            EnsureNotRolledBack();

            Dispose();
        }

        private void EnsureNotRolledBack()
        {
            if (m_Feat != null)
            {
                if (m_Feat.IsRolledBack())
                {
                    Debug.Assert(false, "by some reasons roll back state doesn't go");
                    (m_Part as IModelDoc2).FeatureManager.EditRollback(
                        (int)swMoveRollbackBarTo_e.swMoveRollbackBarToEnd, null);
                }
            }
        }

        private void DisposeTempBody()
        {
            if (m_TempBody != null)
            {
                m_TempBody.Hide(m_Part);
                m_TempBody = null;
                GC.Collect();
            }
        }

        public void Process(IPartDoc part, RoundStockFeatureParameters par = null, IFeature feat = null)
        {
            m_Part = part;

            m_Feat = feat;

            if (par == null)
            {
                par = new RoundStockFeatureParameters();//TODO: load from global settings
            }

            m_View.Show(par, m_Part as IModelDoc2);

            ShowPreview(par);

            //TODO: if feat is not null - access selection and get parameters
        }

        public void Dispose()
        {
            m_View.ParametersChanged -= OnParametersChanged;
            m_View.Closing -= OnPageClosing;
            m_View.Closed -= OnPageClosed;
            m_View.Dispose();
        }
    }
}
