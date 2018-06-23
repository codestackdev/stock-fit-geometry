using CodeStack.Community.StockFit.Base;
using CodeStack.Community.StockFit.Base.Math;
using CodeStack.Community.StockFit.Stocks.Cylinder;
using CodeStack.Community.StockFit.Sw.Math;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Unity;
using Unity.Injection;
using Unity.Resolution;

namespace CodeStack.Community.StockFit.Sw
{
    public class ServicesContainer
    {
        private UnityContainer m_Container;

        public ServicesContainer(ISldWorks app)
        {
            m_Container = new UnityContainer();
            m_Container.RegisterType<IStockTool, RoundStockTool>();
            m_Container.RegisterInstance(app);
            m_Container.RegisterInstance(app.IGetMathUtility() as IMathUtility);
            m_Container.RegisterType<IStockFitExtractor<CylinderParams>, CylindricalStockFitExtractor>();
            m_Container.RegisterType<IVectorMathService, SwVectorMathService>();
        }

        internal RoundStockTool GetStockTool()
        {
            return m_Container.Resolve<IStockTool>() as RoundStockTool;
        }
    }
}
