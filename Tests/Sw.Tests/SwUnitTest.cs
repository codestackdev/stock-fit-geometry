using CodeStack.Community.Testing.Sw;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sw.Tests.Framework;
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
        protected SwUnitTestService Sw { get; private set; }

        [TestInitialize]
        public void OnInitialize()
        {
            var prms = new SwUnitTestParameters()
            {
                LoadOption = SwLoadOption_e.ConnectToProcess,
                OptionDetails = new ConnectToProcessOptionDetails()
                {
                    ProcessToConnect = 11076
                }
            };

            Sw = new SwUnitTestService(prms);
        }

        [TestCleanup]
        public void OnCleanup()
        {
            Sw.Dispose();
        }
    }
}
