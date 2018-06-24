using CodeStack.Community.StockFit.Sw.Pmp.Attributes;
using CodeStack.Community.StockFit.Sw.Reflection;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CodeStack.Community.StockFit.Sw.Pmp
{
    public interface IStockFeaturePage : IDisposable
    {
        event Action<RoundStockFeatureParameters> ParametersChanged;
        event Action<bool, RoundStockFeatureParameters> Closed;
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

        public StockFeaturePage(ISldWorks app, ISwRoundStockTool stockTool)
        {
            m_App = app;
            m_StockTool = stockTool;

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
        }

        public void Show(RoundStockFeatureParameters parameters, IModelDoc2 model)
        {
            const int DEFAULT_OPTION = 0;

            m_CurParameters = parameters;
            m_CurModel = model;
            
            (m_Controls[Controls_e.CreateSolidBody] as IPropertyManagerPageCheckbox).Checked = parameters.CreateSolidBody;

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
