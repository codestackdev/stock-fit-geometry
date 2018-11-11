using CodeStack.Community.StockFit.Sw.Properties;
using SolidWorks.Interop.sldworks;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xarial.AppLaunchKit.Attributes;
using Xarial.AppLaunchKit.Services.Attributes;

[assembly: AssemblyTitle("Sw")]
[assembly: AssemblyDescription("SOLIDWORKS add-in to generate best-fit round stock for solid bodies")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("www.codestack.net")]
[assembly: AssemblyProduct("StockMaster for SOLIDWORKS")]
[assembly: AssemblyCopyright("Copyright © 2018 www.codestack.net")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: Guid("bf046003-0d5b-4930-9dce-b6eee9727340")]

[assembly: InternalsVisibleTo("Sw.Tests")]

[assembly: AssemblyVersion("0.5.0.0")]
[assembly: AssemblyFileVersion("0.5.0.0")]

//[assembly: UpdatesUrl(typeof(UpdatesServerMock), nameof(UpdatesServerMock.UpdateUrl))]

[assembly: Log("CodeStack", "StockMaster", true, false)]
//[assembly: UserSettings("Settings", false, typeof(CustomUserSettingsBackwardCompatibility))]
[assembly: About(typeof(Resources), nameof(Resources.Eula), nameof(Resources.Licenses), nameof(Resources.round_stock_icon))]

[assembly: ApplicationInfo(typeof(Resources), System.Environment.SpecialFolder.ApplicationData,
    nameof(Resources.WorkDir), nameof(Resources.AppTitle), nameof(Resources.round_stock_dlg_icon))]