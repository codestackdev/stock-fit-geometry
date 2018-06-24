using CodeStack.Community.DevTools.Sw.Testing.Services;
using CodeStack.Community.DevTools.Sw.Testing.TempDisplay;
using CodeStack.Community.StockFit.Stocks.Cylinder;
using CodeStack.Community.StockFit.Sw;
using CodeStack.Community.StockFit.Sw.Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sw.Tests
{
    [TestClass]
    public class RoundStockToolTest : SwUnitTest
    {
        [TestMethod]
        public void TestCreateCylindricalStockSimpleRound()
        {
            var body = Sw.App.WithDocument<IBody2>("Data\\t1.sldprt", 
                m => 
                {
                    var input = Sw.Doc.GetEntityByName<IFace>(m as IPartDoc, "INPUT", EntityType_e.swSelFACES);
                    var tempBody = CreateCylindricalStock(m, input);
                    Sw.Doc.TempDisplayBody(tempBody, m, new SyncFormDisposeToken());
                    return tempBody;
                });
        }

        [TestMethod]
        public void TestCreateCylindricalStockSimpleRoundReoriented()
        {
            var body = Sw.App.WithDocument<IBody2>("Data\\t2.sldprt",
                m =>
                {
                    var input = Sw.Doc.GetEntityByName<IFace>(m as IPartDoc, "INPUT", EntityType_e.swSelFACES);
                    var tempBody = CreateCylindricalStock(m, input);
                    Sw.Doc.TempDisplayBody(tempBody, m, new SyncFormDisposeToken());
                    return tempBody;
                });
        }

        [TestMethod]
        public void TestCreateCylindricalStockSimpleRect()
        {
            var body = Sw.App.WithDocument<IBody2>("Data\\t3.sldprt",
                m =>
                {
                    var input = Sw.Doc.GetEntityByName<IFace>(m as IPartDoc, "INPUT", EntityType_e.swSelFACES);
                    var tempBody = CreateCylindricalStock(m, input);
                    Sw.Doc.TempDisplayBody(tempBody, m, new SyncFormDisposeToken());
                    return tempBody;
                });
        }

        [TestMethod]
        public void TestCreateCylindricalStockSimpleRectRotated()
        {
            var body = Sw.App.WithDocument<IBody2>("Data\\t4.sldprt",
                m =>
                {
                    var input = Sw.Doc.GetEntityByName<IFace>(m as IPartDoc, "INPUT", EntityType_e.swSelFACES);
                    var tempBody = CreateCylindricalStock(m, input);
                    Sw.Doc.TempDisplayBody(tempBody, m, new SyncFormDisposeToken());
                    return tempBody;
                });
        }


        [TestMethod]
        public void TestCreateCylindricalStockSimpleRectSquare()
        {
            var body = Sw.App.WithDocument<IBody2>("Data\\t5.sldprt",
                m =>
                {
                    var input = Sw.Doc.GetEntityByName<IFace>(m as IPartDoc, "INPUT", EntityType_e.swSelFACES);
                    var tempBody = CreateCylindricalStock(m, input);
                    Sw.Doc.TempDisplayBody(tempBody, m, new SyncFormDisposeToken());
                    return tempBody;
                });
        }

        [TestMethod]
        public void TestCreateCylindricalStockSimpleRectCut()
        {
            var body = Sw.App.WithDocument<IBody2>("Data\\t6.sldprt",
                m =>
                {
                    var input = Sw.Doc.GetEntityByName<IFace>(m as IPartDoc, "INPUT", EntityType_e.swSelFACES);
                    var tempBody = CreateCylindricalStock(m, input);
                    Sw.Doc.TempDisplayBody(tempBody, m, new SyncFormDisposeToken());
                    return tempBody;
                });
        }

        private IBody2 CreateCylindricalStock(IModelDoc2 model, object inputObj,
            bool concentric = false)
        {
            CylinderParams cylParams;
            var tempBody = m_StockTool.CreateCylindricalStock(model as IPartDoc, 
                inputObj, concentric, 0, out cylParams);

            return tempBody;
        }
    }
}
