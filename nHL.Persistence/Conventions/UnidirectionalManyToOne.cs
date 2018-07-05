using NHibernate.Mapping.ByCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace nHL.Persistence.Conventions
{
    public class UnidirectionalManyToOne : AbstractRelationConvention
    {
        public static void MapManyToOne<TControllingEntity, TInverseEntity>(
         ModelMapper mapper, PropertyInfo property
        )
            where TControllingEntity : class
            where TInverseEntity : class
        {
            var controllingPropertyName = property.PropertyType.Name;
            var controllingColumnName = string.Format("{0}Id", controllingPropertyName);
            mapper.Class<TControllingEntity>(map => map.ManyToOne<TInverseEntity>(property.Name,
                                                        cm =>
                                                        {
                                                            cm.Cascade(Cascade.Persist);
                                                            cm.Column(controllingColumnName);
                                                            cm.Index("IDX_" + controllingColumnName);
                                                            cm.NotNullable(false);
                                                        }));

        }


        protected override void CallGenericRelationMethod(NHibernate.Mapping.ByCode.ConventionModelMapper mapper, System.Reflection.PropertyInfo property, System.Reflection.PropertyInfo inverseProperty)
        {

            var method = typeof(UnidirectionalManyToOne).GetMethod("MapManyToOne", BindingFlags.Public | BindingFlags.Static)
                                                   .MakeGenericMethod(new[] { property.DeclaringType, property.PropertyType });

            method.Invoke(null, new object[] { mapper, property });

        }

        protected override System.Reflection.PropertyInfo GetInverseProperty(System.Reflection.PropertyInfo property)
        {
            var type = property.PropertyType;
            var to = type.DetermineCollectionElementOrDictionaryValueType();
            if (to == null)
            {
                // no generic collection or simple property
                var result = type.GetProperties().FirstOrDefault(x =>
                    {
                        var hasPropertyOfSameType = x.PropertyType == property.DeclaringType;
                        var hasCollectionOfSameType = false;
                        var collectionType = x.PropertyType.DetermineCollectionElementOrDictionaryValueType();
                        if (collectionType != null)
                        {
                            var argumentCollectionType = x.PropertyType.GetGenericArguments()[0];
                            hasCollectionOfSameType = argumentCollectionType == property.DeclaringType;
                        }
                        return hasCollectionOfSameType || hasPropertyOfSameType;
                    });
                if (result == null)
                    this.ShouldProcess = true;
                return result;
            }
            return null;
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
