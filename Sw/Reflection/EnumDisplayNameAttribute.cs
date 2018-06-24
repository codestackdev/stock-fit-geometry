//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CodeStack.Community.StockFit.Sw.Reflection
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumDisplayNameAttribute : DisplayNameAttribute
    {
        public EnumDisplayNameAttribute(string dispName) : base(dispName)
        {
        }
    }
}
