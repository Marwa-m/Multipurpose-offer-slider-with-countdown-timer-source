using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Widgets.DailyOffers.Data;
using Nop.Plugin.Widgets.DailyOffers.Domain;
using Nop.Plugin.Widgets.DailyOffers.Factories;
using Nop.Plugin.Widgets.DailyOffers.Services;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.DailyOffers.Domain.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {

        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            //services
            builder.RegisterType<OfferService>().As<IOfferService>().InstancePerLifetimeScope();
            //Factories
            builder.RegisterType<OfferModelFactory>().As<IOfferModelFactory>().InstancePerLifetimeScope();


        }

        public int Order => 1;
    }
}
