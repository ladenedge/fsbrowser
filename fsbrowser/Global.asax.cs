
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using Ninject.Web.Common.WebHost;
using Ninject.Web.Mvc.FilterBindingSyntax;
using FSBrowser.Filters;

namespace FSBrowser
{
    public class MvcApplication : NinjectHttpApplication
    {
        protected override IKernel CreateKernel()
        {
            var kernel = new StandardKernel();

            kernel.Bind<IConfig>().To<AppSettingsConfig>();
            kernel.Bind<IFileSystem>().To<FileSystem>();

            kernel.BindFilter<PathRootingFilter>(FilterScope.Controller, 0)
                  .WhenControllerHas<HasPathInputsAttribute>();
            kernel.BindFilter<PathConvertingFilter>(FilterScope.Controller, 1)
                  .WhenControllerHas<HasPathInputsAttribute>();

            kernel.Bind<IModelBinder>().To<FileSystemInfoBinder>();
            ModelBinders.Binders.DefaultBinder = kernel.Get<IModelBinder>();

            return kernel;
        }

        protected override void OnApplicationStarted()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
