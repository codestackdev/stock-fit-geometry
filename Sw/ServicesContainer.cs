using CodeStack.Community.StockFit.Base;
using CodeStack.Community.StockFit.Base.Math;
using CodeStack.Community.StockFit.Stocks.Cylinder;
using CodeStack.Community.StockFit.Sw.Math;
using CodeStack.Community.StockFit.Sw.Pmp;
using CodeStack.Community.StockFit.Sw.Pmp.Attributes;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swpublished;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace CodeStack.Community.StockFit.Sw
{
    public class ServicesContainer
    {
        private UnityContainer m_Container;

        public static ServicesContainer Instance
        {
            get;
            private set;
        }

        public ServicesContainer(ISldWorks app, RoundStockFeatureSettings setts)
        {
            Instance = this;

            m_Container = new UnityContainer();

            m_Container.RegisterType<ISwRoundStockTool, RoundStockTool>(
                new ContainerControlledLifetimeManager());

            m_Container.RegisterInstance(app);
            m_Container.RegisterInstance(app.IGetMathUtility() as IMathUtility);

            m_Container.RegisterInstance(setts);

            m_Container.RegisterType<IStockFitExtractor<CylinderParams>, CylindricalStockFitExtractor>(
                new ContainerControlledLifetimeManager());

            m_Container.RegisterType<IVectorMathService, SwVectorMathService>(
                new ContainerControlledLifetimeManager());

            m_Container.RegisterType<IStockFeaturePage, StockFeaturePage>(
                new TransientLifetimeManager());

            m_Container.RegisterType<StockFeaturePageController>(
                new TransientLifetimeManager());
        }

        internal RoundStockTool GetStockTool()
        {
            return m_Container.Resolve<ISwRoundStockTool>() as RoundStockTool;
        }

        internal TService GetService<TService>()
        {
            return m_Container.Resolve<TService>();
        }
    }
}
