using NHibernate.Mapping.ByCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace nHL.Persistence.Conventions
{
    public class BidirectionalManyToManyRelationsConvention : AbstractRelationConvention
    {
        /*
        public static void ManyToMany<TControllingEntity, TInverseEntity>(
         ModelMapper mapper,
         Expression<Func<TControllingEntity, IEnumerable<TInverseEntity>>> controllingProperty,
         Expression<Func<TInverseEntity, IEnumerable<TControllingEntity>>> inverseProperty
         )
            where TControllingEntity : class
            where TInverseEntity : class
        {
            var controllingPropertyName = ((MemberExpression)controllingProperty.Body).Member.Name;
            var controllingColumnName = string.Format("{0}Id", controllingPropertyName);
            var inverseColumnName = string.Format("{0}Id", typeof(TControllingEntity).Name);
            var tableName = string.Format("{0}To{1}", typeof(TControllingEntity).Name, controllingPropertyName);
            mapper.Class<TControllingEntity>(map => map.Set(controllingProperty,
                                                            cm =>
                                                            {
                                                                cm.Cascade(Cascade.All | Cascade.DeleteOrphans);
                                                                cm.Table(tableName);
                                                                cm.Key(km => km.Column(controllingColumnName));
                                                            },
                                                            em => em.ManyToMany(m => m.Column(inverseColumnName))));
            mapper.Class<TInverseEntity>(map => map.Set(inverseProperty,
                                                        cm =>
                                                        {
                                                            cm.Table(tableName);
                                                            cm.Inverse(true);
                                                            cm.Key(km => km.Column(inverseColumnName));
                                                        },
                                                        em => em.ManyToMany(m => m.Column(controllingColumnName))));
        }

        public static void ManyToMany<TControllingEntity, TInverseEntity>(
            ModelMapper mapper,
            Expression<Func<TControllingEntity, IEnumerable<TInverseEntity>>> controllingProperty
            )
            where TControllingEntity : class
            where TInverseEntity : class
        {
            var controllingPropertyName = ((MemberExpression)controllingProperty.Body).Member.Name;
            var controllingColumnName = string.Format("{0}Id", controllingPropertyName);
            var inverseColumnName = string.Format("{0}Id", typeof(TControllingEntity).Name);
            var tableName = string.Format("{0}To{1}", typeof(TControllingEntity).Name, controllingPropertyName);
            mapper.Class<TControllingEntity>(map => map.Bag(controllingProperty,
                                                            cm =>
                                                            {
                                                                cm.Cascade(Cascade.All | Cascade.DeleteOrphans);
                                                                cm.Table(tableName);
                                                                cm.Key(km => km.Column(controllingColumnName));
                                                            },
                                                            em => em.ManyToMany(m => m.Column(inverseColumnName))));
        }*/

        public static void MapManyToMany<TControllingEntity, TInverseEntity>(
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
            var isInverseReadOnly = !inverseProperty.CanWrite;
            if (isSelfReferencingObject(property, inverseProperty))
            {
                MapSelfReferencingObject<TControllingEntity, TInverseEntity>(mapper, property);
            }
            else
            {
                mapper.Class<TControllingEntity>(map => map.Set<TInverseEntity>(property.Name,
                                                                cm =>
                                                                {
                                                                    if (isReadOnly)
                                                                        cm.Access(Accessor.NoSetter);
                                                                    cm.Cascade(Cascade.All | Cascade.DeleteOrphans);
                                                                    cm.Table(tableName);
                                                                    cm.Key(km => km.Column(controllingColumnName));
                                                                    cm.Lazy(CollectionLazy.Lazy);
                                                                },
                                                                em => em.ManyToMany(m => m.Column(inverseColumnName))));
                mapper.Class<TInverseEntity>(map => map.Set<TControllingEntity>(inverseProperty.Name,
                                                            cm =>
                                                            {
                                                                if (isInverseReadOnly)
                                                                    cm.Access(Accessor.ReadOnly);
                                                                cm.Table(tableName);
                                                                cm.Inverse(true);
                                                                cm.Key(km => km.Column(inverseColumnName));
                                                                cm.Lazy(CollectionLazy.Lazy);
                                                            },
                                                            em => em.ManyToMany(m => m.Column(controllingColumnName))));
            }
        }

        public static void MapSelfReferencingObject<TControllingEntity, TInverseEntity>(
            ModelMapper mapper, PropertyInfo property)
            where TControllingEntity : class
            where TInverseEntity : class
        {
            var controllingPropertyName = property.DeclaringType.Name;
            var controllingColumnName = string.Format("{0}Id", controllingPropertyName);
            var inverseColumnName = string.Format("{0}Id", property.Name);
            var tableName = string.Format("{0}To{1}", controllingPropertyName, property.Name);
            var isReadOnly = !property.CanWrite;
            mapper.Class<TControllingEntity>(map => map.Set<TInverseEntity>(property.Name,
                                            cm =>
                                            {
                                                if (isReadOnly)
                                                    cm.Access(Accessor.NoSetter);
                                                cm.Cascade(Cascade.All | Cascade.DeleteOrphans);
                                                cm.Table(tableName);
                                                cm.Key(km =>
                                                {
                                                    km.ForeignKey(string.Format("fk_{0}_{1}",
                                                                                    property.Name,
                                                                                    controllingColumnName));
                                                    km.Column(controllingColumnName);
                                                });
                                                cm.Lazy(CollectionLazy.Lazy);
                                            },
                                            em => em.ManyToMany(m => m.Column(inverseColumnName))));
        }


        public static void CallGenericMapManytoManyMethod(ModelMapper mapper, PropertyInfo property, PropertyInfo inverseProperty)
        {
            var method = typeof(BidirectionalManyToManyRelationsConvention).GetMethod("MapManyToMany", BindingFlags.Public | BindingFlags.Static)
                                                      .MakeGenericMethod(new[] { property.DeclaringType, inverseProperty.DeclaringType });
            method.Invoke(null, new object[] { mapper, property, inverseProperty });
        }

        protected override void CallGenericRelationMethod(ConventionModelMapper mapper, PropertyInfo property, PropertyInfo inverseProperty)
        {
           CallGenericMapManytoManyMethod(mapper, property, inverseProperty);
        }

        protected override PropertyInfo GetInverseProperty(PropertyInfo property)
        {
            var type = property.PropertyType;
            var to = type.DetermineCollectionElementOrDictionaryValueType();
            if (to == null)
            {
                // no generic collection or simple property
                return null;
            }

            var expectedInversePropertyType = type.GetGenericTypeDefinition()
                .MakeGenericType(property.DeclaringType);

            var argument = type.GetGenericArguments()[0];
            var pluralizedName = service.Pluralize(property.DeclaringType.Name);
            var result = argument.GetProperties().FirstOrDefault(x => x.PropertyType == expectedInversePropertyType && x.Name == pluralizedName);
            if (result != null)
                this.ShouldProcess = true;
            return result;
        }

        protected override PropertyInfo GetInverseProperty(MemberInfo member)
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
