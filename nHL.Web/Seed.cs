using nHL.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nHL.Web
{
    public static class Seed
    {
        public static void PopulateDatabase(object sender, NHibernate.ISessionFactory sessionFactory)
        {
            using (var session = sessionFactory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                if (session.Query<Culture>().Any())
                    return;
                var enCulture = new Culture { DisplayName = "English", Name = "en-US", FlagFilename = "flag_En.png" };
                var roCulture = new Culture { DisplayName = "Romana", Name = "ro-RO", FlagFilename = "flag_Ro.png" };
                var deCulture = new Culture { DisplayName = "Deustch", Name = "de-DE", FlagFilename = "flag_De.png" };
                session.Save(enCulture);
                session.Save(roCulture);
                session.Save(deCulture);
                var USACountry = new Country { Culture = enCulture, Name = "USA" };
                var RomaniaCountry = new Country { Culture = roCulture, Name = "Romania" };
                var GermanyCountry = new Country { Culture = deCulture, Name = "Germany" };
                session.Save(USACountry);
                session.Save(RomaniaCountry);
                session.Save(GermanyCountry);
                tx.Commit();
            }
        }
    }
}
