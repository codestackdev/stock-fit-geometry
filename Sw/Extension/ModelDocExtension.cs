//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SolidWorks.Interop.sldworks
{
    public static class ModelDocExtension
    {
        public static bool SelectDispatches(this IModelDoc2 model, bool append, ISelectData selData, params object[] ents)
        {
            if (ents != null && ents.Any())
            {
                var disps = ents.Select(e => new DispatchWrapper(e)).ToArray();

                var selCount = model.Extension.MultiSelect2(disps,
                    append, selData);

                return selCount == disps.Length;
            }
            else
            {
                return true;
            }
        }

        public static void SetPropertyValue(this IModelDoc2 model, string prpName, string prpVal, string conf = "")
        {
            var prpMgr = model.Extension.CustomPropertyManager[conf];
            prpMgr.Add2(prpName, (int)swCustomInfoType_e.swCustomInfoText, prpVal);
            prpMgr.Set2(prpName, prpVal);
        }
    }
}
