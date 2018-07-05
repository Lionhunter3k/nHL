using System;
using System.Collections.Generic;
using System.Text;

namespace nHL.Domain
{
    public class Address
    {
        public virtual int Id { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual string Street { get; set; }

        public virtual string Town { get; set; }

        public virtual string ZipCode { get; set; }

        public virtual string Phone { get; set; }

        public virtual string Fax { get; set; }

        public virtual Country Country { get; set; }
    }
}
