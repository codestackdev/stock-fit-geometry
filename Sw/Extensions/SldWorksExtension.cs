using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SolidWorks.Interop.sldworks
{
    public enum BubbleTooltipPosition_e
    {
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft
    }

    public static class SldWorksExtension
    { 
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
        
        public static void ShowBubbleTooltip(this ISldWorks app, 
            string title, string message, BubbleTooltipPosition_e pos, Image img = null)
        {
            var hwnd = new IntPtr(app.IFrameObject().GetHWnd());

            int x = 0;
            int y = 0;
            int xOffset = 0;
            var arrowPos = swArrowPosition.swArrowNone;

            var doc = app.IActiveDoc2;
            
            if (doc != null)
            {
                xOffset = doc.GetFeatureManagerWidth();

                var view = doc.IActiveView;

                if (view != null)
                {
                    hwnd = new IntPtr(view.GetViewHWnd());
                }
            }          
            
            RECT rect;
            GetWindowRect(hwnd, out rect);

            switch (pos)
            {
                case BubbleTooltipPosition_e.TopLeft:
                    x = rect.Left;
                    y = rect.Top;
                    arrowPos = swArrowPosition.swArrowLeftTop;
                    break;

                case BubbleTooltipPosition_e.TopRight:
                    x = rect.Right;
                    y = rect.Top;
                    arrowPos = swArrowPosition.swArrowRightTop;
                    break;

                case BubbleTooltipPosition_e.BottomRight:
                    x = rect.Right;
                    y = rect.Bottom;
                    arrowPos = swArrowPosition.swArrowRightBottom;
                    break;

                case BubbleTooltipPosition_e.BottomLeft:
                    x = rect.Left;
                    y = rect.Bottom;
                    arrowPos = swArrowPosition.swArrowLeftBottom;
                    break;
            }
            
            x += xOffset;

            var imgPath = "";

            if (img != null)
            {
                imgPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                img.Save(imgPath);
            }
                        
            app.ShowBubbleTooltipAt2(x, y, (int)arrowPos,
                            title, message, (int)swBitMaps.swBitMapUserDefined, imgPath, "", 0,
                            (int)swLinkString.swLinkStringNone, "", "");
        }
    }
}
