using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;
using NHibernate;
using NHibernate.Linq;
using nHL.Domain;
using nHL.Web.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nHL.Web.Components
{
    public class NHMissingStringLocalizerLogger : IMissingStringLocalizerLogger
    {
        private readonly ISessionFactory sessionFactory;

        private readonly ConcurrentBag<ValueTuple<string, string, CultureInfo>> cachedResources = new ConcurrentBag<ValueTuple<string, string, CultureInfo>>();

        public NHMissingStringLocalizerLogger(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public async Task FlushMissingStringsAsync()
        {
            using (var session = sessionFactory.OpenSession())
            {
                var alreadyInsertedResources = new HashSet<ValueTuple<string, string, CultureInfo>>();
                while(cachedResources.TryTake(out var missingLocalization))
                {
                    if (alreadyInsertedResources.Add(missingLocalization))
                        await session.SaveAsync(new LocalizedStringResource { Culture = session.Load<Culture>(missingLocalization.Item3.Name), Key = missingLocalization.Item2, Resource = missingLocalization.Item1, Text = missingLocalization.Item2, ResourceNotFound = true});
                }
                await session.FlushAsync();
            }
        }

        public void LogMissingLocalization(string location, string name, CultureInfo culture)
        {
            cachedResources.Add(new ValueTuple<string, string, CultureInfo>(location, name, culture));
        }
    }
}
