using NHibernate.Mapping.ByCode;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace nHL.Persistence.Conventions
{
    public class EnumConvention : IAmConvention
    {
        public void ProcessMapper(NHibernate.Mapping.ByCode.ConventionModelMapper mapper, IEnumerable<Type> entities)
        {
            mapper.BeforeMapProperty += mapProperty;
        }

        public static bool IsEnum(PropertyInfo property)
        {
            return property.PropertyType.IsEnum || (property.PropertyType.IsGenericType &&
                         property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                         property.PropertyType.GetGenericArguments()[0].IsEnum);
        }

        private static void callGenericTypeMethod(IPropertyMapper map, PropertyInfo property)
        {
            var enumStringOfPropertyType = typeof(EnumStringType<>).MakeGenericType(property.PropertyType);
            var method = map.GetType().GetMethods().First(x => x.Name == "Type" && !x.GetParameters().Any());
            var genericMethod = method.MakeGenericMethod(new[] { enumStringOfPropertyType });
            genericMethod.Invoke(map, null);
        }

        private static void mapProperty(IModelInspector modelInspector, PropertyPath member, IPropertyMapper map)
        {
            var property = member.LocalMember as PropertyInfo;
            if (property == null) return;
            if (IsEnum(property))
            {
                callGenericTypeMethod(map, property);
            }
        }
    }
}
