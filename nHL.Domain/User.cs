using System;
using System.Collections.Generic;
using System.Text;

namespace nHL.Domain
{
    public enum UserRoles : short
    {
        Administrator = 1,
        SimpleUser = 2,
    }

    public class User
    {
        public virtual int Id { get; set; }

        public virtual string Username { get; set; }

        public virtual string Password { get; set; }

        public virtual string Email { get; set; }

        public virtual UserRoles UserRole { get; set; }

        public virtual Address Address { get; set; }
    }
}
