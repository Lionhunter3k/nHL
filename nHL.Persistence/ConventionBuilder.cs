using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace nHL.Persistence
{
    public class ConventionBuilder
    {
        public ConventionBuilder()
        {
            this.Conventions = new List<IAmConvention>();
        }

        public List<IAmConvention> Conventions { get; set; }

        public Action<ModelMapper> AutoMappingOverride { set; get; }

        public string SessionFactoryName { get; set; }

        //public ValidatorEngine MappingsValidatorEngine { get; private set; }

        public string OutputXmlMappingsFile { set; get; }

        public bool ShowLogs { set; get; }

        public void ProcessConfiguration(Configuration cfg, IEnumerable<Type> entities)
        {
            initConfiguration(cfg);
            HbmMapping mapping = this.getMappings(entities);
            cfg.AddMapping(mapping);
            //injectValidationAndFieldLengths(cfg);
        }

        private HbmMapping getMappings(IEnumerable<Type> entities)
        {
            //Using the built-in auto-mapper
            var mapper = new ConventionModelMapper();
            foreach (var convention in this.Conventions)
                convention.ProcessMapper(mapper, entities);
            if (AutoMappingOverride != null) AutoMappingOverride(mapper);
            var mapping = mapper.CompileMappingFor(entities);
            showOutputXmlMappings(mapping);
            return mapping;
        }

        private void showOutputXmlMappings(HbmMapping mapping)
        {
            var outputXmlMappings = mapping.AsString();
            if (ShowLogs)
                Console.WriteLine(outputXmlMappings);
            if (OutputXmlMappingsFile == null) return;
            File.WriteAllText(OutputXmlMappingsFile, outputXmlMappings);
        }

        private void initConfiguration(Configuration configure)
        {
            if (this.SessionFactoryName != null)
                configure.SessionFactoryName(SessionFactoryName);
        }
    }
}
