//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using CodeStack.Community.StockFit.Base;
using CodeStack.Community.StockFit.Base.Math;
using CodeStack.Community.StockFit.MVC;
using CodeStack.Community.StockFit.Stocks.Cylinder;
using CodeStack.Community.StockFit.Sw.Math;
using CodeStack.Community.StockFit.Sw.MVC;
using CodeStack.Community.StockFit.Sw.Options;
using CodeStack.Community.StockFit.Sw.Services;
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
    /// <summary>
    /// Dependency injection and services container
    /// </summary>
    public class ServicesContainer
    {
        private UnityContainer m_Container;

        public static ServicesContainer Instance
        {
            get;
            private set;
        }

        public ServicesContainer(ISldWorks app)
        {
            Instance = this;

            m_Container = new UnityContainer();

            m_Container.RegisterType<RoundStockModel>(
                new ContainerControlledLifetimeManager());

            m_Container.RegisterInstance(app);
            m_Container.RegisterInstance(app.IGetMathUtility() as IMathUtility);
            
            m_Container.RegisterType<IStockFitExtractor<CylinderParams>, CylindricalStockFitExtractor>(
                new ContainerControlledLifetimeManager());

            m_Container.RegisterType<IVectorMathService, SwVectorMathService>(
                new ContainerControlledLifetimeManager());

            //m_Container.RegisterType<RoundStockView>(
            //    new TransientLifetimeManager());

            m_Container.RegisterType<RoundStockController>(
                new ContainerControlledLifetimeManager());

            m_Container.RegisterType<OptionsStore>(
                new ContainerControlledLifetimeManager());

            var setts = m_Container.Resolve<RoundStockFeatureSettings>();

            m_Container.RegisterInstance(setts);
        }

        internal TService GetService<TService>()
        {
            return m_Container.Resolve<TService>();
        }
    }
}
