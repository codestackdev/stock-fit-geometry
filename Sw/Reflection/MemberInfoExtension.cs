using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Reflection
{
    public static class MemberInfoExtension
    {
        public static T TryGetAttribute<T>(this MemberInfo membInfo)
            where T : Attribute
        {
            var atts = membInfo.GetCustomAttributes(typeof(T), true);

            if (atts != null && atts.Any())
            {
                return atts.First() as T;
            }
            else
            {
                return null;
            }
        }
    }
}
