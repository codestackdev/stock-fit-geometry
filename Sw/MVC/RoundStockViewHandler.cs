//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeStack.Community.StockFit.Sw.MVC
{
    [ComVisible(true)]
    [Guid("923CD9C0-B8C7-44D3-8028-1039B771CA1A")]
    public class RoundStockViewHandler : IPropertyManagerPage2Handler8, IDisposable
    {
        public event Action<int> ValueChanged;
        public event Action<bool> Closed;
        public event Action<bool> Closing;
        public event Action Help;
        public event Action WhatsNew;

        private bool m_IsOk;

        public void AfterActivation()
        {
        }

        public void AfterClose()
        {
            Closed?.Invoke(m_IsOk);
        }

        public int OnActiveXControlCreated(int Id, bool Status)
        {
            return 0;
        }

        public void OnButtonPress(int Id)
        {
        }

        public void OnCheckboxCheck(int Id, bool Checked)
        {
            ValueChanged.Invoke(Id);
        }

        public void OnClose(int Reason)
        {
            m_IsOk = (Reason == (int)swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Okay);
            Closing?.Invoke(m_IsOk);
        }

        public void OnComboboxEditChanged(int Id, string Text)
        {
        }

        public void OnComboboxSelectionChanged(int Id, int Item)
        {
            ValueChanged.Invoke(Id);
        }

        public void OnGainedFocus(int Id)
        {
        }

        public void OnGroupCheck(int Id, bool Checked)
        {
        }

        public void OnGroupExpand(int Id, bool Expanded)
        {
        }

        public bool OnHelp()
        {
            Help?.Invoke();
            return true;
        }

        public bool OnKeystroke(int Wparam, int Message, int Lparam, int Id)
        {
            return true;
        }

        public void OnListboxRMBUp(int Id, int PosX, int PosY)
        {
        }

        public void OnListboxSelectionChanged(int Id, int Item)
        {
        }

        public void OnLostFocus(int Id)
        {
        }

        public bool OnNextPage()
        {
            return true;
        }

        public void OnNumberboxChanged(int Id, double Value)
        {
            ValueChanged.Invoke(Id);
        }

        public void OnOptionCheck(int Id)
        {
        }

        public void OnPopupMenuItem(int Id)
        {
        }

        public void OnPopupMenuItemUpdate(int Id, ref int retval)
        {
        }

        public bool OnPreview()
        {
            return true;
        }

        public bool OnPreviousPage()
        {
            return true;
        }

        public void OnRedo()
        {
        }

        public void OnSelectionboxCalloutCreated(int Id)
        {
        }

        public void OnSelectionboxCalloutDestroyed(int Id)
        {
        }

        public void OnSelectionboxFocusChanged(int Id)
        {
        }

        public void OnSelectionboxListChanged(int Id, int Count)
        {
            ValueChanged.Invoke(Id);
        }

        public void OnSliderPositionChanged(int Id, double Value)
        {
        }

        public void OnSliderTrackingCompleted(int Id, double Value)
        {
        }

        public bool OnSubmitSelection(int Id, object Selection, int SelType, ref string ItemText)
        {
            return true;
        }

        public bool OnTabClicked(int Id)
        {
            return true;
        }

        public void OnTextboxChanged(int Id, string Text)
        {
        }

        public void OnUndo()
        {
        }

        public void OnWhatsNew()
        {
            WhatsNew?.Invoke();
        }

        public int OnWindowFromHandleControlCreated(int Id, bool Status)
        {
            return 0;
        }

        public void Dispose()
        {
        }
    }
}
