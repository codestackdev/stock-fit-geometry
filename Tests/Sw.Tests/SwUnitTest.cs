using CodeStack.Community.DevTools.Sw.Testing;
using CodeStack.Community.DevTools.Sw.Testing.Parameters;
using CodeStack.Community.StockFit.MVC;
using CodeStack.Community.StockFit.Sw;
using CodeStack.Community.StockFit.Sw.Options;
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

        protected RoundStockModel m_StockModel;

        [TestInitialize]
        public void OnInitialize()
        {
            var prms = new TestServiceStartupParameters()
            {
                ConnectOption = AppConnectOption_e.ConnectToProcess,
                ConnectionDetails = new ConnectToProcessConnectionDetails()
                {
                    ProcessToConnect = 10640
                }
            };

            Sw = new TestService(prms);

            m_StockModel = new ServicesContainer(Sw.SldWorks, 
                new RoundStockFeatureSettings()).GetService<RoundStockModel>();
        }

        [TestCleanup]
        public void OnCleanup()
        {
            Sw.Dispose();
        }
    }
}
