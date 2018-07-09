using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace nHL.Web.Models
{
    public class LogOnModel
    {
        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [Required]
        public string Username { get; set; }
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
