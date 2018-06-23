using CodeStack.Community.DevTools.Sw.Testing;
using CodeStack.Community.DevTools.Sw.Testing.Parameters;
using CodeStack.Community.StockFit.Sw;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sw.Tests
{
    [TestClass]
    public class SwUnitTest
    {
        protected TestService Sw { get; private set; }

        protected RoundStockTool m_StockTool;

        [TestInitialize]
        public void OnInitialize()
        {
            var prms = new TestServiceStartupParameters()
            {
                ConnectOption = AppConnectOption_e.ConnectToProcess,
                ConnectionDetails = new ConnectToProcessConnectionDetails()
                {
                    ProcessToConnect = 11076
                }
            };

            Sw = new TestService(prms);

            m_StockTool = new ServicesContainer(Sw.SldWorks).GetStockTool();
        }

        [TestCleanup]
        public void OnCleanup()
        {
            Sw.Dispose();
        }
    }
}
