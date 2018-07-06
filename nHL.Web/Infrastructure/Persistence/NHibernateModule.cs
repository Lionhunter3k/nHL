using Autofac;
using Microsoft.Extensions.Configuration;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace nHL.Web.Infrastructure.Persistence
{
    public abstract class NHibernateModule : Autofac.Module
    {
        public string SchemaRootPath { get; set; }

        public string SchemaFilename { get; set; } = "db.sql";

        public event EventHandler<ISessionFactory> SessionFactoryCreated;

        protected abstract Configuration BuildConfiguration(IComponentContext context);

        private Configuration GetConfiguration(IComponentContext context)
        {
            var config = BuildConfiguration(context);
            if (!string.IsNullOrEmpty(SchemaRootPath))
            {
                var schemaExport = new SchemaExport(config);
                schemaExport
                .SetOutputFile(Path.Combine(SchemaRootPath, SchemaFilename))
                .Execute(true, false, false);
            }
            return config;
        }

        private ISessionFactory GetSessionFactory(IComponentContext context)
        {
            var sessionFactory = context.Resolve<Configuration>().BuildSessionFactory();
            SessionFactoryCreated?.Invoke(this, sessionFactory);
            return sessionFactory;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(GetConfiguration)
                        .AsSelf()
                        .SingleInstance();
            builder.Register(GetSessionFactory)
                    .As<ISessionFactory>()
                    .SingleInstance();
            builder.Register(c => c.Resolve<ISessionFactory>().OpenSession())
                  .As<ISession>()
                  .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<ISessionFactory>().OpenStatelessSession())
                .As<IStatelessSession>()
                .InstancePerLifetimeScope();
            builder.RegisterType<StatefulSessionWrapper>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<StatelessSessionWrapper>().AsSelf().InstancePerLifetimeScope();
        }
    }
}
