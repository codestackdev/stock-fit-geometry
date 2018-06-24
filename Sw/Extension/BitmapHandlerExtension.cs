//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using CodeStack.Community.StockFit.Sw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SolidWorksTools.File
{
    public static class BitmapHandlerExtension
    {
        public static string GetIcon(this BitmapHandler bmp, string name)
        {
            var type = typeof(SwStockFirGeometryAddIn);

            var assm = Assembly.GetAssembly(type);

            return bmp.CreateFileFromResourceBitmap($"{type.Namespace}.Icons.{name}", assm);
        }
    }
}
