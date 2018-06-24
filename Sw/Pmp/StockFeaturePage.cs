//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using CodeStack.Community.StockFit.Sw.Pmp.Attributes;
using CodeStack.Community.StockFit.Sw.Reflection;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeStack.Community.StockFit.Sw.Pmp
{
    public interface IStockFeaturePage : IDisposable
    {
        /// <summary>
        /// Raises when parameter is changed and preview needs to be updated
        /// </summary>
        event Action<RoundStockFeatureParameters> ParametersChanged;

        /// <summary>
        /// Raises when page is closed
        /// </summary>
        /// <remarks>Generate feature in the event handler</remarks>
        event Action<bool, RoundStockFeatureParameters> Closed;

        /// <summary>
        /// Raises when page is about to be closed
        /// </summary>
        event Action<bool, RoundStockFeatureParameters> Closing;

        void Show(RoundStockFeatureParameters parameters, IModelDoc2 model);
    }

    public class StockFeaturePage : IStockFeaturePage
    {
        public event Action<RoundStockFeatureParameters> ParametersChanged;
        public event Action<bool, RoundStockFeatureParameters> Closed;
        public event Action<bool, RoundStockFeatureParameters> Closing;

        private enum Controls_e
        {
            [PmpControlType(swPropertyManagerPageControlType_e.swControlType_Selectionbox)]
            [EnumDisplayName("Direction")]
            [Description("Select cylindrical face or plane feature to specify the direction")]
            DirectionSelection,

            [PmpControlType(swPropertyManagerPageControlType_e.swControlType_Combobox)]
            [EnumDisplayName("Stock Step")]
            [Description("Specifies the stock step")]
            StockStep,

            [PmpControlType(swPropertyManagerPageControlType_e.swControlType_Checkbox)]
            [EnumDisplayName("Concentric")]
            [Description("Specifies if the stock should be concentric with selected cylindrical face")]
            Concentric,

            [PmpControlType(swPropertyManagerPageControlType_e.swControlType_Checkbox)]
            [EnumDisplayName("Create solid body")]
            [Description("Specifies if solid body should be created")]
            CreateSolidBody
        }

        private ISldWorks m_App;
        private ISwRoundStockTool m_StockTool;

        private IPropertyManagerPage2 m_Page;
        private StockFeaturePagePmpHandler m_Handler;

        private Dictionary<Controls_e, IPropertyManagerPageControl> m_Controls;

        private RoundStockFeatureParameters m_CurParameters;
        private IModelDoc2 m_CurModel;

        private RoundStockFeatureSettings m_Setts;

        public StockFeaturePage(ISldWorks app, ISwRoundStockTool stockTool, RoundStockFeatureSettings setts)
        {
            m_App = app;
            m_StockTool = stockTool;
            m_Setts = setts;

            m_Handler = new StockFeaturePagePmpHandler();
            m_Handler.Closing += OnClosing;
            m_Handler.Closed += OnClosed;
            m_Handler.ValueChanged += OnValueChanged;

            var options = swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton |
                swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton
                | swPropertyManagerPageOptions_e.swPropertyManagerOptions_WhatsNew;

            int errors = -1;
            m_Page = m_App.CreatePropertyManagerPage("Stock", 
                (int)options, m_Handler, ref errors) as IPropertyManagerPage2;

            m_Page.SetMessage3("Select cylindrical face or plane to generate the round stock",
                (int)swPropertyManagerPageMessageVisibility.swMessageBoxVisible,
                (int)swPropertyManagerPageMessageExpanded.swMessageBoxExpand, "Stock Feature");

            string icon = Path.Combine(Path.GetDirectoryName(
                        typeof(SwStockFirGeometryAddIn).Assembly.Location),
                        "Icons\\FeatureIcon.bmp");

            m_Page.SetTitleBitmap2(icon);

            AddControls();
        }

        private void OnClosing(bool isOk)
        {
            Closing?.Invoke(isOk, m_CurParameters);
        }

        private void OnValueChanged(int ctrlId)
        {
            var raiseChangeEvent = false;

            var ctrlEnumId = (Controls_e)ctrlId;

            switch (ctrlEnumId)
            {
                case Controls_e.DirectionSelection:
                    m_CurParameters.Direction = m_CurModel.ISelectionManager.GetSelectedObject6(
                        (m_Controls[ctrlEnumId] as IPropertyManagerPageSelectionbox).SelectionIndex[0], -1);
                    raiseChangeEvent = true;
                    break;

                case Controls_e.CreateSolidBody:
                    m_CurParameters.CreateSolidBody = (m_Controls[ctrlEnumId] as IPropertyManagerPageCheckbox).Checked;
                    break;

                case Controls_e.Concentric:
                    m_CurParameters.ConcenticWithCylindricalFace = (m_Controls[ctrlEnumId] as IPropertyManagerPageCheckbox).Checked;
                    raiseChangeEvent = true;
                    break;

                case Controls_e.StockStep:
                    m_CurParameters.StockStep = m_Setts.StockSteps.ElementAt((m_Controls[ctrlEnumId] as IPropertyManagerPageCombobox).CurrentSelection).Key;
                    raiseChangeEvent = true;
                    break;
            }
            
            if (raiseChangeEvent)
            {
                ParametersChanged?.Invoke(m_CurParameters);
            }
        }

        private void OnClosed(bool isOk)
        {
            Closed?.Invoke(isOk, m_CurParameters);
        }

        private void AddControls()
        {
            m_Controls = new Dictionary<Controls_e, IPropertyManagerPageControl>();
            
            foreach (Controls_e ctrlId in Enum.GetValues(typeof(Controls_e)))
            {
                var align = swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
                var options = swAddControlOptions_e.swControlOptions_Enabled |
                          swAddControlOptions_e.swControlOptions_Visible;
                
                var type = ctrlId.GetAttribute<PmpControlTypeAttribute>().Type;
                var title = ctrlId.GetAttribute<DisplayNameAttribute>().DisplayName;
                var tip = ctrlId.GetAttribute<DescriptionAttribute>().Description;

                var ctrl = m_Page.AddControl2((int)ctrlId,
                    (short)type, title,
                    (short)align, (int)options, tip) as IPropertyManagerPageControl;

                m_Controls.Add(ctrlId, ctrl);
            }

            m_Controls[Controls_e.DirectionSelection].SetStandardPictureLabel(
                (int)swControlBitmapLabelType_e.swBitmapLabel_SelectFace);

            var selBox = m_Controls[Controls_e.DirectionSelection] as IPropertyManagerPageSelectionbox;

            selBox.SingleEntityOnly = true;
            var filter = new int[] { (int)swSelectType_e.swSelFACES, (int)swSelectType_e.swSelDATUMPLANES };
            selBox.Height = 20;
            selBox.SetSelectionFilters(filter);

            m_Controls[Controls_e.StockStep].SetStandardPictureLabel(
                (int)swControlBitmapLabelType_e.swBitmapLabel_LinearDistance);

            var comboBox = m_Controls[Controls_e.StockStep] as IPropertyManagerPageCombobox;

            comboBox.AddItems(m_Setts.StockSteps.Select(s => s.Key).ToArray());
        }

        public void Show(RoundStockFeatureParameters parameters, IModelDoc2 model)
        {
            const int DEFAULT_OPTION = 0;

            m_CurParameters = parameters;
            m_CurModel = model;
            
            (m_Controls[Controls_e.CreateSolidBody] as IPropertyManagerPageCheckbox).Checked = parameters.CreateSolidBody;
            (m_Controls[Controls_e.Concentric] as IPropertyManagerPageCheckbox).Checked = parameters.ConcenticWithCylindricalFace;
            (m_Controls[Controls_e.StockStep] as IPropertyManagerPageCombobox).CurrentSelection = (short)m_Setts.StockSteps.Keys.ToList().IndexOf(parameters.StockStep);

            if (parameters.Direction != null)
            {
                (m_Controls[Controls_e.DirectionSelection] as IPropertyManagerPageSelectionbox).SetSelectionFocus();
                m_CurModel.SelectDispatches(false, null, parameters.Direction);
            }

            m_Page.Show2(DEFAULT_OPTION);
        }

        public void Dispose()
        {
            m_Handler.Closing -= OnClosing;
            m_Handler.Closed -= OnClosed;
            m_Handler.ValueChanged -= OnValueChanged;
            m_Handler.Dispose();
        }
    }
}
