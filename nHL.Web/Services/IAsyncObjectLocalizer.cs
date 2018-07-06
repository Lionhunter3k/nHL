using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace nHL.Web.Services
{
    public interface IAsyncObjectLocalizer<T>
    {
        ValueTask<T> GetLocalizedObjectAsync();
    }
}
