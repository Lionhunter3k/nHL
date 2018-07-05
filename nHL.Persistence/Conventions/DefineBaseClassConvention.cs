using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nHL.Persistence.Conventions
{
    public class DefineBaseClassConvention : IAmConvention
    {
        public Type BaseEntityToIgnore { set; get; }

        public void ProcessMapper(NHibernate.Mapping.ByCode.ConventionModelMapper mapper, IEnumerable<Type> entities)
        {
            if (BaseEntityToIgnore == null) return;
            mapper.IsEntity((type, declared) =>
                BaseEntityToIgnore.IsAssignableFrom(type) &&
                BaseEntityToIgnore != type &&
                !type.IsInterface);
            mapper.IsRootEntity((type, declared) => type.BaseType == BaseEntityToIgnore);
        }
    }
}
