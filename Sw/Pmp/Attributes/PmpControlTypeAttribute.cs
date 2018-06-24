using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.Community.StockFit.Sw.Pmp.Attributes
{
    public class PmpControlTypeAttribute : Attribute
    {
        public swPropertyManagerPageControlType_e Type { get; private set; }

        public PmpControlTypeAttribute(swPropertyManagerPageControlType_e type)
        {
            Type = type;
        }
    }
}
