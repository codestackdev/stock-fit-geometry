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
    }
}
