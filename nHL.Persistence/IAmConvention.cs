using NHibernate.Mapping.ByCode;
using System;
using System.Collections.Generic;
using System.Text;

namespace nHL.Persistence
{
    public interface IAmConvention
    {
        void ProcessMapper(ConventionModelMapper mapper, IEnumerable<Type> entities);
    }
}
