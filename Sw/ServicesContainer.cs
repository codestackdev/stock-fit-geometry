//**********************
//Stock Master
//Copyright(C) 2018 www.codestack.net
//Product: https://www.codestack.net/labs/solidworks/stock-fit-geometry/
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
using System.Threading;
using System.Threading.Tasks;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;
using Xarial.AppLaunchKit;
using Xarial.AppLaunchKit.Base.Services;
using Xarial.AppLaunchKit.Services.About;
using Xarial.AppLaunchKit.Services.External;
using Xarial.AppLaunchKit.Services.Logger;
using Xarial.AppLaunchKit.Services.Updates;
using Xarial.AppLaunchKit.Services.UserSettings;

namespace CodeStack.Community.StockFit.Sw
{
    /// <summary>
    /// Dependency injection and services container
    /// </summary>
    public class ServicesContainer
    {
        private readonly UnityContainer m_Container;

        public static ServicesContainer Instance
        {
            get;
            private set;
        }

        private ServicesManager m_Kit;

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

            m_Container.RegisterType<RoundStockController>(
                new ContainerControlledLifetimeManager());
            
            m_Kit = new ServicesManager(this.GetType().Assembly, new IntPtr(app.IFrameObject().GetHWnd()),
                typeof(UpdatesService),
                typeof(UserSettingsService),
                typeof(SystemEventLogService),
                typeof(AboutApplicationService));

            m_Kit.HandleError += OnHandleError;

            var syncContext = SynchronizationContext.Current;

            Task.Run(() =>
            {
                SynchronizationContext.SetSynchronizationContext(
                        syncContext);
                m_Kit.StartServicesAsync().Wait();
            });
            
            m_Container.RegisterInstance(m_Kit.GetService<ILogService>());
            m_Container.RegisterInstance(m_Kit.GetService<IUserSettingsService>());
            m_Container.RegisterInstance(m_Kit.GetService<IAboutApplicationService>());
        }
        
        private bool OnHandleError(Exception ex)
        {
            try
            {
                m_Kit.GetService<ILogService>().LogException(ex);
            }
            catch
            {
            }

            return true;
        }

        internal TService GetService<TService>()
        {
            return m_Container.Resolve<TService>();
        }
    }
}
