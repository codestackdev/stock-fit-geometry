//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.Community.StockFit.Sw.Pmp.Attributes
{
    /// <summary>
    /// Attribute to decorate the control id specified in enumerator
    /// </summary>
    /// <remarks>This allows to dynamically generate property page controls</remarks>
    public class PmpControlTypeAttribute : Attribute
    {
        /// <summary>
        /// Type of the control on property page
        /// </summary>
        public swPropertyManagerPageControlType_e Type { get; private set; }

        public PmpControlTypeAttribute(swPropertyManagerPageControlType_e type)
        {
            Type = type;
        }
    }
}
