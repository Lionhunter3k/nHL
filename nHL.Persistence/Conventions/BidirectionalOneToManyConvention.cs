using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Mapping.ByCode;
using System.Reflection;

namespace nHL.Persistence.Conventions
{
    public class BidirectionalOneToManyConvention : AbstractRelationConvention
    {
        public static void MapOneToMany<TControllingEntity, TInverseEntity>(
         ModelMapper mapper, PropertyInfo property, PropertyInfo inverseProperty
        )
            where TControllingEntity : class
            where TInverseEntity : class
        {
            var controllingPropertyName = property.DeclaringType.Name;
            var controllingColumnName = string.Format("{0}Id", controllingPropertyName);
            var inverseColumnName = string.Format("{0}Id", inverseProperty.DeclaringType.Name);
            var tableName = string.Format("{0}To{1}", inverseProperty.Name, property.Name);
            var isReadOnly = !property.CanWrite;
            mapper.Class<TControllingEntity>(map => map.Set<TInverseEntity>(property.Name,
                                                            cm =>
                                                            {
                                                                if (isReadOnly)
                                                                    cm.Access(Accessor.ReadOnly);
                                                                cm.Cascade(Cascade.All | Cascade.DeleteOrphans);
                                                                cm.Inverse(true);
                                                                cm.Key(km =>
                                                                {
                                                                    km.ForeignKey(string.Format("fk_{0}_{1}",
                                                                               property.Name,
                                                                               controllingColumnName));
                                                                    km.Column(controllingColumnName);
                                                                });
                                                                cm.Lazy(CollectionLazy.NoLazy);
                                                            },
                                                            em => em.OneToMany()));
            mapper.Class<TInverseEntity>(map => map.ManyToOne<TControllingEntity>(inverseProperty.Name,
                                                        cm =>
                                                        {
                                                            cm.Column(controllingColumnName);
                                                            cm.Index("IDX_" + controllingColumnName);
                                                            cm.NotNullable(false);
                                                        }));

        }


        protected override void CallGenericRelationMethod(NHibernate.Mapping.ByCode.ConventionModelMapper mapper, System.Reflection.PropertyInfo property, System.Reflection.PropertyInfo inverseProperty)
        {
            var method = typeof(BidirectionalOneToManyConvention).GetMethod("MapOneToMany", BindingFlags.Public | BindingFlags.Static)
                                                   .MakeGenericMethod(new[] { property.DeclaringType, inverseProperty.DeclaringType });
            method.Invoke(null, new object[] { mapper, property, inverseProperty });
        }

        protected override System.Reflection.PropertyInfo GetInverseProperty(System.Reflection.PropertyInfo property)
        {
            var type = property.PropertyType;
            var to = type.DetermineCollectionElementOrDictionaryValueType();
            if (to == null)
            {
                // no generic collection or simple property
                return null;
            }

            var argument = type.GetGenericArguments()[0];
            var result = argument.GetProperties().FirstOrDefault(x => x.PropertyType == property.DeclaringType && x.Name == property.DeclaringType.Name);
            if (result != null)
                this.ShouldProcess = true;
            return result;
        }

        protected override System.Reflection.PropertyInfo GetInverseProperty(System.Reflection.MemberInfo member)
        {
            var type = member.GetPropertyOrFieldType();
            var to = type.DetermineCollectionElementOrDictionaryValueType();
            if (to == null)
            {
                // no generic collection or simple property
                return null;
            }

            var argument = type.GetGenericArguments()[0];
            return argument.GetProperties().FirstOrDefault(x => x.PropertyType == member.DeclaringType);
        }
    }
}
