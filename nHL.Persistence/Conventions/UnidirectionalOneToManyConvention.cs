using NHibernate.Mapping.ByCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace nHL.Persistence.Conventions
{
    public class UnidirectionalOneToManyConvention: AbstractRelationConvention
    {
        public static void MapOneToMany<TControllingEntity, TInverseEntity>(
         ModelMapper mapper, PropertyInfo property
        )
            where TControllingEntity : class
            where TInverseEntity : class
        {
            var controllingPropertyName = property.DeclaringType.Name;
            var controllingColumnName = string.Format("{0}Id", controllingPropertyName);
            var isReadOnly = !property.CanWrite;
            mapper.Class<TControllingEntity>(map => map.Set<TInverseEntity>(property.Name,
                                                            cm =>
                                                            {
                                                                if (isReadOnly)
                                                                    cm.Access(Accessor.NoSetter);
                                                                cm.Cascade(Cascade.All | Cascade.DeleteOrphans);
                                                                cm.Inverse(false);
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

        }


        protected override void CallGenericRelationMethod(NHibernate.Mapping.ByCode.ConventionModelMapper mapper, System.Reflection.PropertyInfo property, System.Reflection.PropertyInfo inverseProperty)
        {
            var type = property.PropertyType;
            var argument = type.GetGenericArguments()[0];
            var method = typeof(UnidirectionalOneToManyConvention).GetMethod("MapOneToMany", BindingFlags.Public | BindingFlags.Static)
                                                   .MakeGenericMethod(new[] { property.DeclaringType, argument });
            method.Invoke(null, new object[] { mapper, property });
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
            var results = argument.GetProperties()
                .FirstOrDefault(x =>
                {
                    var hasPropertyOfSameType = x.PropertyType == property.DeclaringType && x.Name == property.DeclaringType.Name;
                    var hasCollectionOfSameType = false;
                    var collectionType = x.PropertyType.DetermineCollectionElementOrDictionaryValueType();
                    if (collectionType != null)
                    {
                        var argumentCollectionType = x.PropertyType.GetGenericArguments()[0];
                        hasCollectionOfSameType = argumentCollectionType == property.DeclaringType && x.Name == base.Service.Pluralize(property.DeclaringType.Name);
                    }
                    return hasCollectionOfSameType || hasPropertyOfSameType;
                });
            if (results == null)
                this.ShouldProcess = true;
            return results;
        }

        protected override System.Reflection.PropertyInfo GetInverseProperty(System.Reflection.MemberInfo member)
        {
            var type = member.GetPropertyOrFieldType();
            var to = type.DetermineCollectionElementOrDictionaryValueType();
            if (to == null)
            {
                // no generic collection or simple property
                throw new NotImplementedException();
            }
            else
                return null;

            //var argument = type.GetGenericArguments()[0];
            //return argument.GetProperties().FirstOrDefault(x => x.PropertyType == member.DeclaringType);
        }
    }
}
