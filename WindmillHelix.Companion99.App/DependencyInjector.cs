using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Data;
using WindmillHelix.Companion99.Services;

namespace WindmillHelix.Companion99.App
{
    internal static class DependencyInjector
    {
        private static object _containerLock = new object();
        private static IContainer _container;

        public static IContainer GetContainer()
        {
            if (_container == null)
            {
                lock (_containerLock)
                {
                    if (_container == null)
                    {
                        _container = BuildContainer();
                    }
                }
            }

            return _container;
        }

        public static T Resolve<T>()
        {
            var container = GetContainer();
            var result = container.Resolve<T>();
            return result;
        }


        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(typeof(AppAssemblyLocator).Assembly);
            builder.RegisterAssemblyModules(typeof(ServicesAssemblyLocator).Assembly);
            builder.RegisterAssemblyModules(typeof(DataAssemblyLocator).Assembly);

            return builder.Build();
        }
    }
}
