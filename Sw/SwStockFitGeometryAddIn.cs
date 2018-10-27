//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using CodeStack.Community.StockFit.Sw.UI;
using CodeStack.SwEx.AddIn;
using CodeStack.SwEx.AddIn.Attributes;
using CodeStack.SwEx.AddIn.Enums;
using SolidWorks.Interop.sldworks;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace CodeStack.Community.StockFit.Sw
{

    [Guid("DAA5615D-0BA6-461A-90DD-9E016E24C7AB"), ComVisible(true)]
    [AutoRegister("Stock Master", "Stock Master")]
    [ProgId(ID)]
    public class SwStockFitGeometryAddIn : SwAddInEx
    {
        public const string ID = "CodeStack.StockFitGeometry";
        
        [Title("Stock Master")]
        [Description("Stock Master")]
        private enum Commands_e
        {
            [Title("Create Stock Feature")]
            [Description("Creates Stock Feature")]
            [CommandItemInfo(true, true, swWorkspaceTypes_e.Part)]
            CreateStockFeature,

            [Title("About...")]
            [Description("About Stock Master")]
            [CommandItemInfo(true, false, swWorkspaceTypes_e.All)]
            About
        }

        private ServicesContainer m_Container;

        public override bool OnConnect()
        {
            m_Container = new ServicesContainer(m_App);

            AddCommandGroup<Commands_e>(OnCommandClick);
            return true;
        }
        
        private void OnCommandClick(Commands_e cmd)
        {
            switch (cmd)
            {
                case Commands_e.CreateStockFeature:
                    //var ctrl = m_Container.GetService<RoundStockController>();
                    //ctrl.Process(m_App.IActiveDoc2 as IPartDoc);
                    break;

                case Commands_e.About:
                    var aboutForm = new AboutForm();
                    aboutForm.ShowDialog();
                    break;
            }
        }
    }
}
