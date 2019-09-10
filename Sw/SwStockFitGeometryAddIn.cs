//**********************
//Stock Master
//Copyright(C) 2018 www.codestack.net
//Product: https://www.codestack.net/labs/solidworks/stock-fit-geometry/
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using CodeStack.Community.StockFit.Sw.MVC;
using CodeStack.Community.StockFit.Sw.Options;
using CodeStack.Community.StockFit.Sw.Properties;
using CodeStack.Community.StockFit.Sw.Services;
using CodeStack.SwEx.AddIn;
using CodeStack.SwEx.AddIn.Attributes;
using CodeStack.SwEx.AddIn.Enums;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Xarial.AppLaunchKit.Base.Services;

namespace CodeStack.Community.StockFit.Sw
{

    [Guid("DAA5615D-0BA6-461A-90DD-9E016E24C7AB"), ComVisible(true)]
#if DEBUG
    [AutoRegister("Stock Master", "Stock Master")]
#endif
    [ProgId(ID)]
    public class SwStockFitGeometryAddIn : SwAddInEx
    {
        static SwStockFitGeometryAddIn()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assmName = new AssemblyName(args.Name);
            
            if (assmName.Name == "Newtonsoft.Json")
            {
                var assmPath = Path.Combine(Path.GetDirectoryName(typeof(SwStockFitGeometryAddIn).Assembly.Location),
                    "Newtonsoft.Json.dll");

                return Assembly.LoadFile(assmPath);
            }

            return null;
        }

        public const string ID = "CodeStack.StockFitGeometry";
        
        [SwEx.Common.Attributes.Title("Stock Master")]
        [Description("Stock Master")]
        [SwEx.Common.Attributes.Icon(typeof(Resources), nameof(Resources.round_stock_icon))]
        private enum Commands_e
        {
            [SwEx.Common.Attributes.Title("Create Stock Feature")]
            [Description("Creates Stock Feature")]
            [CommandItemInfo(true, true, swWorkspaceTypes_e.Part)]
            [SwEx.Common.Attributes.Icon(typeof(Resources), nameof(Resources.round_stock_icon))]
            CreateStockFeature,

            [SwEx.Common.Attributes.Title("About...")]
            [Description("About Stock Master")]
            [CommandItemInfo(true, false, swWorkspaceTypes_e.All)]
            [SwEx.Common.Attributes.Icon(typeof(Resources), nameof(Resources.about_icon))]
            About
        }

        private ServicesContainer m_Container;

        private RoundStockController m_Controller;

        private IUserSettingsService m_UserSettings;

        public override bool OnConnect()
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
                m_Container = new ServicesContainer(App);

                m_UserSettings = m_Container.GetService<IUserSettingsService>();

                m_Controller = m_Container.GetService<RoundStockController>();
                m_Controller.FeatureInsertionCompleted += OnFeatureInsertionCompleted;

                AddCommandGroup<Commands_e>(OnCommandClick);

                return true;
            }
            catch(Exception ex)
            {
                try
                {
                    m_Container.GetService<ILogService>().LogException(ex);
                }
                catch
                {
                }

                App.SendMsgToUser2($"Failed to load {Resources.AppTitle}. Please see log for more details",
                    (int)swMessageBoxIcon_e.swMbStop, (int)swMessageBoxBtn_e.swMbOk);

                return false;
            }
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            m_Container.GetService<ILogService>().LogException(e.ExceptionObject as Exception);
        }

        private void OnFeatureInsertionCompleted(RoundStockFeatureParameters parameters, IPartDoc part, bool isOk)
        {
            if (isOk)
            {
                var feat = (part as IModelDoc2).FeatureManager
                    .InsertComFeature<RoundStockMacroFeature, RoundStockFeatureParameters>(parameters);

                Debug.Assert(feat != null);

                //swpuc only
                if (feat != null)
                {
                    int index = 1;
                    var featNameBase = $"Stock Block Round for config {(part as IModelDoc2).IGetActiveConfiguration().Name}";
                    var featName = featNameBase;
                    while ((part as IModelDoc2).FeatureManager.IsNameUsed(
                        (int)swNameType_e.swFeatureName, featName))
                    {
                        featName = featNameBase + index++;
                    }

                    feat.Name = featName;
                }
            }
        }

        private void OnCommandClick(Commands_e cmd)
        {
            switch (cmd)
            {
                case Commands_e.CreateStockFeature:
                    var par = m_UserSettings.ReadSettings<RoundStockFeatureParameters>(nameof(RoundStockFeatureParameters));
                    m_Controller.ShowPage(par, App.IActiveDoc2 as IPartDoc, null, null);
                    break;

                case Commands_e.About:
                    m_Container.GetService<IAboutApplicationService>().ShowAboutForm();
                    break;
            }
        }
    }
}
