using CodeStack.Community.StockFit.Sw.Reflection;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using SolidWorksTools;
using SolidWorksTools.File;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeStack.Community.StockFit.Sw
{

    [Guid("DAA5615D-0BA6-461A-90DD-9E016E24C7AB"), ComVisible(true)]
    [SwAddin(
        Description = "Stock Fit Geometry (Preview)",
        Title = "Stock Fit Geometry",
        LoadAtStartup = true
        )]
    public class SwStockFirGeometryAddIn : ISwAddin
    {
        private enum CommandsGroups_e
        {
            [EnumDisplayName("Stock Fit Geometry")]
            [Description("Stock Fit Geometry")]
            Main
        }

        private enum CommandItemEnableState_e
        {
            //Deselects and disables the item
            DeselectDisable = 0,

            //Deselects and enables the item; this is the default state if no update function is specified
            DeselectEnable = 1,

            //Selects and disables the item
            SelectDisable = 2,

            //Selects and enables the item 
            SelectEnable = 3
        }

        private enum Commands_e
        {
            [EnumDisplayName("Create Stock Feature")]
            [Description("Creates Stock Feature")]
            CreateStockFeature
        }
        
        private ISldWorks m_App;
        private int m_AddInId;
        private ICommandManager m_CmdMgr;

        #region SolidWorks Registration

        [ComRegisterFunction]
        public static void RegisterFunction(Type t)
        {
            try
            {
                var att = t.GetCustomAttributes(false).OfType<SwAddinAttribute>().FirstOrDefault();

                if (att == null)
                {
                    throw new NullReferenceException($"{typeof(SwAddinAttribute).FullName} is not set on {t.GetType().FullName}");
                }

                Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey hkcu = Microsoft.Win32.Registry.CurrentUser;

                string keyname = "SOFTWARE\\SolidWorks\\Addins\\{" + t.GUID.ToString() + "}";
                Microsoft.Win32.RegistryKey addinkey = hklm.CreateSubKey(keyname);
                addinkey.SetValue(null, 0);

                addinkey.SetValue("Description", att.Description);
                addinkey.SetValue("Title", att.Title);

                keyname = "Software\\SolidWorks\\AddInsStartup\\{" + t.GUID.ToString() + "}";
                addinkey = hkcu.CreateSubKey(keyname);
                addinkey.SetValue(null, Convert.ToInt32(att.LoadAtStartup), Microsoft.Win32.RegistryValueKind.DWord);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while registering the addin: " + ex.Message);
            }
        }

        [ComUnregisterFunction]
        public static void UnregisterFunction(Type t)
        {
            try
            {
                Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey hkcu = Microsoft.Win32.Registry.CurrentUser;

                string keyname = "SOFTWARE\\SolidWorks\\Addins\\{" + t.GUID.ToString() + "}";
                hklm.DeleteSubKey(keyname);

                keyname = "Software\\SolidWorks\\AddInsStartup\\{" + t.GUID.ToString() + "}";
                hkcu.DeleteSubKey(keyname);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while unregistering the addin: " + e.Message);
            }
        }

        #endregion

        public bool ConnectToSW(object ThisSW, int cookie)
        {
            m_App = ThisSW as ISldWorks;
            m_AddInId = cookie;

            m_App.SetAddinCallbackInfo(0, this, m_AddInId);

            m_CmdMgr = m_App.GetCommandManager(m_AddInId);

            AddCommandMgr();

            return true;
        }

        public bool DisconnectFromSW()
        {
            RemoveCommandMgr();
            
            Marshal.ReleaseComObject(m_CmdMgr);
            m_CmdMgr = null;

            Marshal.ReleaseComObject(m_App);
            m_App = null;
            
            GC.Collect();
            GC.WaitForPendingFinalizers();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            return true;
        }
        
        private void AddCommandMgr()
        {                        
            var title = CommandsGroups_e.Main.GetAttribute<DisplayNameAttribute>().DisplayName;
            var toolTip = CommandsGroups_e.Main.GetAttribute<DescriptionAttribute>().Description;
            
            int cmdGroupErr = 0;
            bool ignorePrevious = false;

            object registryIDs;
            
            bool getDataResult = m_CmdMgr.GetGroupDataFromRegistry((int)CommandsGroups_e.Main, out registryIDs);

            var knownIDs = (Enum.GetValues(typeof(Commands_e)) as Commands_e[]).Select(e => (int)e).ToArray();
            
            if (getDataResult)
            {
                if (!CompareIDs((int[])registryIDs, knownIDs)) //if the IDs don't match, reset the commandGroup
                {
                    ignorePrevious = true;
                }
            }

            var cmdGroup = m_CmdMgr.CreateCommandGroup2((int)CommandsGroups_e.Main, title, toolTip,
                "", -1, ignorePrevious, ref cmdGroupErr);

            var bmpHelper = new BitmapHandler();

            cmdGroup.LargeIconList = GetIcon(bmpHelper, "ToolbarLarge.bmp");
            cmdGroup.SmallIconList = GetIcon(bmpHelper, "ToolbarSmall.bmp");
            cmdGroup.LargeMainIcon = GetIcon(bmpHelper, "MainIconLarge.bmp");
            cmdGroup.SmallMainIcon = GetIcon(bmpHelper, "MainIconSmall.bmp");
            
            foreach (Enum cmd in Enum.GetValues(typeof(Commands_e)))
            {
                var cmdTitle = cmd.GetAttribute<DisplayNameAttribute>().DisplayName;
                var cmdToolTip = cmd.GetAttribute<DescriptionAttribute>().Description;

                var cmdId = Convert.ToInt32(cmd);

                cmdGroup.AddCommandItem2(cmdTitle, -1, cmdToolTip,
                    cmdTitle, 0, $"{nameof(OnCommandClick)}({cmdId})", $"{nameof(OnCommandEnable)}({cmdId})", cmdId,
                    (int)(swCommandItemType_e.swMenuItem | swCommandItemType_e.swToolbarItem));
            }

            cmdGroup.HasToolbar = true;
            cmdGroup.HasMenu = true;
            cmdGroup.Activate();
            
            bmpHelper.Dispose();
        }

        private string GetIcon(BitmapHandler bmp, string name)
        {
            var assm = Assembly.GetAssembly(this.GetType());

            return bmp.CreateFileFromResourceBitmap($"{this.GetType().Namespace}.Icons.{name}", assm);
        }

        public void OnCommandClick(int cmd)
        {
        }

        public int OnCommandEnable(int cmd)
        {
            return (int)CommandItemEnableState_e.DeselectEnable;
        }

        private void RemoveCommandMgr()
        {
            m_CmdMgr.RemoveCommandGroup((int)CommandsGroups_e.Main);
        }

        private bool CompareIDs(int[] storedIDs, int[] addinIDs)
        {
            var storedList = storedIDs.ToList();
            var addinList = addinIDs.ToList();

            addinList.Sort();
            storedList.Sort();

            return addinList.AreEqualItemWise(storedIDs);
        }
    }
}
