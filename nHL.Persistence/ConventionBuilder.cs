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
            this.FilterAssembly = t => true;
        }

        public List<IAmConvention> Conventions { get; set; }

        public Action<ModelMapper> AutoMappingOverride { set; get; }

        public Action<IDbIntegrationConfigurationProperties> DbIntegrationCfg { get; set; }

        public string SessionFactoryName { get; set; }

        public string DbSchemaOutputFile { set; get; }

        public bool DropTablesCreateDbSchema { set; get; }

        public Assembly MappingsAssembly { set; get; }

        //public ValidatorEngine MappingsValidatorEngine { get; private set; }

        public string OutputXmlMappingsFile { set; get; }

        public bool ShowLogs { set; get; }

        public void ProcessConfiguration(Configuration cfg)
        {
            initConfiguration(cfg);
            HbmMapping mapping = this.getMappings();
            cfg.AddDeserializedMapping(mapping, "NHSchemaTest");
            //injectValidationAndFieldLengths(cfg);
            if (DbSchemaOutputFile == null) return;
            new SchemaExport(cfg).SetOutputFile(DbSchemaOutputFile).Create(false, DropTablesCreateDbSchema);
        }

        public Func<Type, bool> FilterAssembly { get; set; }

        private HbmMapping getMappings()
        {
            //Using the built-in auto-mapper
            var mapper = new ConventionModelMapper();
            var allEntities = MappingsAssembly.GetTypes().Where(FilterAssembly).ToList();
            foreach (var convention in this.Conventions)
                convention.ProcessMapper(mapper, allEntities);
            if (AutoMappingOverride != null) AutoMappingOverride(mapper);
            var mapping = mapper.CompileMappingFor(allEntities);
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
            if (this.DbIntegrationCfg != null)
                configure.DataBaseIntegration(DbIntegrationCfg);
        }
    }
}
