using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;

namespace nHL.Web.Infrastructure
{
    public class ServiceModule : Autofac.Module
    {
        public string RootPath { get; set; }

        private readonly HashSet<Assembly> _assemblies = new HashSet<Assembly>();

        public List<string> AssemblyNames { get; set; } = new List<string>();

        public ServiceModule AddAssemblyFor<TObject>()
        {
            var assembly = typeof(TObject).Assembly;
            _assemblies.Add(assembly);
            return this;
        }

        protected override void Load(ContainerBuilder builder)
        {
            foreach(var assemblyName in AssemblyNames.Distinct())
            {
                _assemblies.Add(Assembly.LoadFrom(RootPath != null ? Path.Combine(RootPath, assemblyName) : assemblyName));
            }
            builder.RegisterAssemblyTypes(_assemblies.ToArray())
                  .Where(c => (c.Namespace?.EndsWith("Components")).GetValueOrDefault() && c.GetInterfaces().Any(t => (t.Namespace?.EndsWith("Services")).GetValueOrDefault()))
                  .As(c => c.GetInterfaces().Where(t => (t.Namespace?.EndsWith("Services")).GetValueOrDefault()))
                  .PropertiesAutowired()
                  .InstancePerLifetimeScope();
        }
    }
}
