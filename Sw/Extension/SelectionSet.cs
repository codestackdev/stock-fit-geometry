//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SolidWorks.Interop.sldworks
{
    /// <summary>
    /// Enables selection to be used in API without changing the user selection set
    /// </summary>
    public class SelectionSet : IDisposable
    {
        private ISelectionMgr m_SelMgr;

        public SelectionSet(ISelectionMgr selMgr)
        {
            if (selMgr == null)
            {
                throw new ArgumentNullException(nameof(selMgr));
            }

            m_SelMgr = selMgr;

            m_SelMgr.SuspendSelectionList();
        }

        /// <summary>
        /// Add object to current selection list
        /// </summary>
        /// <param name="disp">Pointer to dispatch</param>
        /// <param name="selData">Optional selection data</param>
        /// <returns>Result of selection</returns>
        public bool Add(DispatchWrapper disp, ISelectData selData = null)
        {
            if (disp == null)
            {
                throw new ArgumentNullException(nameof(disp));
            }

            return m_SelMgr.AddSelectionListObject(disp, selData);
        }

        /// <summary>
        /// Adds multiple objects into selection list
        /// </summary>
        /// <param name="disps">Array of dispatches to select</param>
        /// <param name="selData">Optional selection data</param>
        /// <returns>Result of the selection</returns>
        public bool AddRange(DispatchWrapper[] disps, ISelectData selData = null)
        {
            if (disps == null)
            {
                throw new ArgumentNullException(nameof(disps));
            }

            return m_SelMgr.AddSelectionListObjects(disps, selData) == disps.Length;
        }

        public void Dispose()
        {
            m_SelMgr.ResumeSelectionList();
        }
    }
}
