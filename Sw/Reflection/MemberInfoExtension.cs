//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Reflection
{
    public static class MemberInfoExtension
    {
        /// <summary>
        /// Attempts to get the attribute from the class member
        /// </summary>
        /// <typeparam name="TAtt">Attribute type</typeparam>
        /// <param name="membInfo">Pointer to member (field or property)</param>
        /// <returns>Pointer to attribute or null if not found</returns>
        public static TAtt TryGetAttribute<TAtt>(this MemberInfo membInfo)
            where TAtt : Attribute
        {
            var atts = membInfo.GetCustomAttributes(typeof(TAtt), true);

            if (atts != null && atts.Any())
            {
                return atts.First() as TAtt;
            }
            else
            {
                return null;
            }
        }
    }
}
