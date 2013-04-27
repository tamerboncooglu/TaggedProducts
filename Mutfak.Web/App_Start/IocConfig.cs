using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MongoDB.Driver;
using Mutfak.Domain.Repo;
using Mutfak.Util;
using Mutfak.Web.Controllers;

namespace Mutfak.Web.App_Start
{
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IKernel _kernel;
        public WindsorControllerFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public override void ReleaseController(IController controller)
        {
            _kernel.ReleaseComponent(controller);
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                throw new HttpException(404, string.Format("Controller for '{0}' could not be found.", requestContext.HttpContext.Request.Path));
            }

            return (IController)_kernel.Resolve(controllerType);
        }
    }

    public class ControllersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Types.FromThisAssembly().Pick()
                                .If(Component.IsInSameNamespaceAs<BaseController>())
                                .If(t => t.Name.EndsWith("Controller"))
                                .Configure(configurer => configurer.Named(configurer.Implementation.Name))
                                .LifestylePerWebRequest());
        }
    }

    public class ServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IFormsAuthenticationService>().ImplementedBy<FormsAuthenticationService>().LifestylePerWebRequest(),
                               Component.For<MongoDatabase>().UsingFactoryMethod(x => x.Resolve<MongoServer>().GetDatabase(Consts.DBName)),
                               Component.For(typeof(IEntityRepository<>)).ImplementedBy(typeof(EntityRepository<>)));

            container.Register(Types.FromThisAssembly().Pick()
                                .If(Component.IsInSameNamespaceAs<FormsAuthenticationService>())
                                .If(t => t.Name.EndsWith("Service") && !t.Name.Contains("FormsAuthentication"))
                                .Configure(configurer => configurer.Named(configurer.Implementation.Name))
                                .LifestylePerWebRequest());
        }
    }
}