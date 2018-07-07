using NHibernate.Mapping.ByCode;
using PluralizationService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace nHL.Persistence.Conventions
{
    public class NamingConvention : IAmConvention
    {
        private IPluralizationApi service = new DummyPluralizationApi();

        public bool Pluralize { get; set; }

        public Func<Type, MemberInfo, string> IdColumnNaming { get; set; } = (t, p) => p.Name;

        public Func<MemberInfo, Type, string> FkConstraintNaming { get; set; } = (p, c) => string.Format("fk_{0}_{1}", p.Name, c.Name);

        public Func<MemberInfo, MemberInfo, string> FkColumnNaming { get; set; } = (p, id) => p.GetPropertyOrFieldType().Name + id.Name;

        public Func<MemberInfo, Type, MemberInfo, string> InverseFkColumnNaming { get; set; } = (p, c, id) => p.Name + id.Name;

        public Func<Type, string> IdFkConstraintNaming { get; set; } = c => string.Format("fk_{0}_{1}",
                                                c.BaseType.Name,
                                                c.Name);

        public Func<MemberInfo, MemberInfo, string> ComponentColumnNaming { get; set; } = (p, c) => p.Name + c.Name;

        public void ProcessMapper(NHibernate.Mapping.ByCode.ConventionModelMapper mapper, IEnumerable<Type> entities)
        {
            mapper.AfterMapClass += PluralizeEntityName;
            mapper.AfterMapClass += PrimaryKeyConvention;
            mapper.AfterMapManyToOne += ReferenceConvention;
            mapper.AfterMapSet += OneToManyConvention;
            mapper.AfterMapBag += OneToManyConvention;
            mapper.AfterMapList += OneToManyConvention;
            mapper.AfterMapManyToMany += ManyToManyConvention;
            mapper.AfterMapJoinedSubclass += MapJoinedSubclass;
            mapper.AfterMapProperty += ComponentNamingConvention;
        }

        private void ComponentNamingConvention(IModelInspector modelInspector, PropertyPath member, IPropertyMapper map)
        {
            var property = member.LocalMember as PropertyInfo;
            if (modelInspector.IsComponent(property.DeclaringType))
            {
                map.Column(ComponentColumnNaming(member.PreviousPath.LocalMember, member.LocalMember));
            }
        }

        private void MapJoinedSubclass(IModelInspector modelInspector, Type type, IJoinedSubclassAttributesMapper map)
        {
            if(Pluralize)
                map.Table(service.Pluralize(type.Name));
            if(IdFkConstraintNaming != null)
            {
                var idProperty = type.GetProperties().Where(modelInspector.IsPersistentId).Single();
                map.Key(x =>
                {
                    x.ForeignKey(IdFkConstraintNaming(type));
                    if (IdColumnNaming != null)
                        x.Column(IdColumnNaming(type, idProperty));
                });
            }
        }


        private void ManyToManyConvention(IModelInspector modelInspector, PropertyPath member, IManyToManyMapper map)
        {
            if(FkConstraintNaming != null)
                map.ForeignKey(
                    FkConstraintNaming(
                           member.LocalMember,
                           member.GetContainerEntity(modelInspector)));
        }

        private void OneToManyConvention(IModelInspector modelInspector, PropertyPath member, ICollectionPropertiesMapper map)
        {
            var inv = GetInverseProperty(member.LocalMember);
            if (inv == null && InverseFkColumnNaming != null)
            {
                var containerEntity = member.GetContainerEntity(modelInspector);
                var idProperty = containerEntity.GetProperties().Where(modelInspector.IsPersistentId).Single();
                map.Key(x => x.Column(InverseFkColumnNaming(member.LocalMember, containerEntity, idProperty)));
            }
        }

        private void ReferenceConvention(IModelInspector modelInspector, PropertyPath member, IManyToOneMapper map)
        {
            if(FkColumnNaming != null)
            {
                var idProperty = member.LocalMember.GetPropertyOrFieldType().GetProperties().Where(modelInspector.IsPersistentId).Single();
                map.Column(k => k.Name(FkColumnNaming(member.LocalMember, idProperty)));
            }
            if(FkConstraintNaming != null)
                map.ForeignKey(
                    FkConstraintNaming(
                           member.LocalMember,
                           member.GetContainerEntity(modelInspector)));
        }

        private void PluralizeEntityName(IModelInspector modelInspector, Type type, IClassAttributesMapper map)
        {
            if (Pluralize)
                map.Table(service.Pluralize(type.Name));
        }

        private void PrimaryKeyConvention(IModelInspector modelInspector, Type type, IClassAttributesMapper map)
        {
            map.Id(k =>
            {
                var idProperty = type.GetProperties().Where(modelInspector.IsPersistentId).Single();
                if (IdColumnNaming != null)
                    k.Column(IdColumnNaming(type, idProperty));
            });
        }

        private static PropertyInfo GetInverseProperty(MemberInfo member)
        {
            var type = member.GetPropertyOrFieldType();
            var to = type.DetermineCollectionElementOrDictionaryValueType();
            if (to == null)
            {
                // no generic collection or simple property
                return null;
            }

            var expectedInversePropertyType = type.GetGenericTypeDefinition()
                                                  .MakeGenericType(member.DeclaringType);

            var argument = type.GetGenericArguments()[0];
            return argument.GetProperties().FirstOrDefault(x => x.PropertyType == expectedInversePropertyType);
        }
    }
}
