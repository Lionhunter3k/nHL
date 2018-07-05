using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Mapping.ByCode;

namespace nHL.Persistence.Conventions
{
    public class ReadOnlyConvention : IAmConvention
    {
        public static void MapReadOnly<TControllingEntity, TReadOnlyProperty>(
         ModelMapper mapper, PropertyInfo property
        )
            where TControllingEntity : class
        {
            var controllingPropertyName = property.PropertyType.Name;
            var controllingColumnName = string.Format("{0}Id", controllingPropertyName);
            mapper.Class<TControllingEntity>(map => map.Property(property.Name, m => m.Access(Accessor.ReadOnly)));

        }

        public void ProcessMapper(NHibernate.Mapping.ByCode.ConventionModelMapper mapper, IEnumerable<Type> entities)
        {
            foreach (var entity in entities)
            {
                var properties = entity.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in properties)
                {
                    var propertyTypeToCheck = property.PropertyType;
                    if (propertyTypeToCheck.DetermineCollectionElementOrDictionaryValueType() != null)
                        continue;
                    if (entities.Contains(propertyTypeToCheck))
                        continue;
                    if (property.CanWrite == false)
                    {
                        var method = typeof(ReadOnlyConvention).GetMethod("MapReadOnly", BindingFlags.Public | BindingFlags.Static)
                                                 .MakeGenericMethod(new[] { property.DeclaringType, property.PropertyType });
                        method.Invoke(null, new object[] { mapper, property });
                    }
                }
            }


        }
    }
}
