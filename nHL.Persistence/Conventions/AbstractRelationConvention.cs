using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Mapping.ByCode;
using System.Globalization;
using PluralizationService;

namespace nHL.Persistence.Conventions
{
    public abstract class AbstractRelationConvention : IAmConvention
    {
        protected IPluralizationApi service = new PluralizationApiBuilder().Build();

        public IPluralizationApi Service
        {
            get
            {
                return this.service;
            }
            set
            {
                if (value == null)
                    throw new NullReferenceException("service");
                this.service = value;
            }
        }

        public Func<Type, bool> FilterType { get; set; }

        protected static bool isSelfReferencingObject(PropertyInfo property, PropertyInfo inverseProperty)
        {
            return property.PropertyType.FullName == inverseProperty.PropertyType.FullName;
        }

        //this method check the property of an object to see if it's a collection type, then if it's a collection type, gets the generic collection type, then create a concrete impl of that generic collection
        //using the declaring class type, then checks the object used for the collection type to see if it has a property which implements the collection of declaring class type
        protected abstract PropertyInfo GetInverseProperty(PropertyInfo property);

        protected abstract PropertyInfo GetInverseProperty(MemberInfo member);

        public void ProcessMapper(NHibernate.Mapping.ByCode.ConventionModelMapper mapper, IEnumerable<Type> entities)
        {
            var mappedItemsCache = new List<string>();
            foreach (var entity in entities)
            {
                var properties = entity.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in properties)
                {
                    this.ShouldProcess = false;
                    var propertyTypeToCheck = property.PropertyType;
                    if (propertyTypeToCheck.DetermineCollectionElementOrDictionaryValueType() != null)
                    {
                        propertyTypeToCheck = propertyTypeToCheck.GetGenericArguments()[0];
                    }
                    if (!FilterType(entity))
                        continue;
                    if (!FilterType(propertyTypeToCheck))
                        continue;
                    var inverseProperty = GetInverseProperty(property);//should return inverse property if conditions are met or return null otherwise
                    if ((inverseProperty != null && mappedItemsCache.Contains(inverseProperty.PropertyType.FullName + inverseProperty.Name + inverseProperty.DeclaringType.FullName)) || mappedItemsCache.Contains(property.PropertyType.FullName + property.Name + property.DeclaringType.FullName))
                        continue;
                    if (ShouldProcess)
                    {
                        CallGenericRelationMethod(mapper, property, inverseProperty);
                        if (inverseProperty != null)
                            mappedItemsCache.Add(inverseProperty.PropertyType.FullName + inverseProperty.Name + inverseProperty.DeclaringType.FullName);
                        mappedItemsCache.Add(property.PropertyType.FullName + property.Name + property.DeclaringType.FullName);
                    }
                }
            }
        }

        protected abstract void CallGenericRelationMethod(NHibernate.Mapping.ByCode.ConventionModelMapper mapper, PropertyInfo property, PropertyInfo inverseProperty);

        protected bool ShouldProcess = false;
    }
}
