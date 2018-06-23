using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using Sw.Tests.Framework;
using System;
using System.IO;

namespace CodeStack.Community.Testing.Sw
{
    public class SwUnitTestService : IDisposable
    {
        protected ISldWorks App { get; private set; }

        private bool m_CloseApp;

        public SwUnitTestService() : this(null) //TODO: load parameters from file
        {
        }

        public SwUnitTestService(SwUnitTestParameters parameters)
        {
            switch (parameters.LoadOption)
            {
                case SwLoadOption_e.ConnectToProcess:
                    ConnectToProcess(parameters.OptionDetails as ConnectToProcessOptionDetails);
                    break;
                case SwLoadOption_e.LoadEmbeded:
                    LoadEmbeded(parameters.OptionDetails as LoadEmbededOptionDetails);
                    break;
                case SwLoadOption_e.StartFromPath:
                    StartFromPath(parameters.OptionDetails as StartFromPathOptionDetails);
                    break;
            }

            if (App == null)
            {
                throw new NullReferenceException("Failed to connect to SOLIDWORKS");
            }
        }

        public TRes WithDocument<TRes>(string path, Func<IModelDoc2, TRes> func)
        {
            if (!Path.IsPathRooted(path))
            {
                Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), path);
            }

            var ext = Path.GetExtension(path).ToLower();

            var docType = swDocumentTypes_e.swDocNONE;

            switch (ext)
            {
                case "sldprt":
                    docType = swDocumentTypes_e.swDocPART;
                    break;

                case "sldasm":
                    docType = swDocumentTypes_e.swDocASSEMBLY;
                    break;

                case "slddrw":
                    docType = swDocumentTypes_e.swDocDRAWING;
                    break;
            }

            IModelDoc2 model = null;

            var res = func.Invoke(model);

            return res;
        }

        private void ConnectToProcess(ConnectToProcessOptionDetails opts)
        {
            App = AppStartTools.GetSwAppFromProcess(opts.ProcessToConnect);
            m_CloseApp = false;
        }

        private void LoadEmbeded(LoadEmbededOptionDetails opts)
        {
            AppStartTools.CreateEmbeded(opts.UseAnyVersion ? -1 : opts.RevisionNumber);
        }

        private void StartFromPath(StartFromPathOptionDetails opts)
        {
            AppStartTools.StartSwApp(opts.ExecutablePath, opts.Timeout);
            m_CloseApp = true;
        }

        public void Dispose()
        {
            if (m_CloseApp)
            {
                if (App != null)
                {
                    App.ExitApp();
                }
            }
        }
    }
}
