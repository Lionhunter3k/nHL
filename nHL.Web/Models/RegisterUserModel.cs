using nHL.Web.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace nHL.Web.Models
{
    public class RegisterUserModel
    {
        [Required(ErrorMessage = "Username is required")]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "Username can't exceed 50 characters")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "Password can't exceed 50 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmation password is required")]
        [Compare("Password", ErrorMessage = "Confirmation password must be the same as the password")]
        [DataType(DataType.Password)]
        [StringLength(50)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [Email(ErrorMessage = "Email is invalid")]
        [DataType(DataType.EmailAddress)]
        [StringLength(128)]
        public string Email { get; set; }

        public AddressModel Address { get; set; } = new AddressModel();
    }

    public class AddressModel
    {
        [Required]
        public int? CountryId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50)]
        public string FirstName { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(200)]
        public string Street { get; set; }

        [Required]
        [RegularExpression(@"([A-Za-zäöüÄÖÜßăîâșțĂÎÂȘȚ\.\\/ ])+", ErrorMessage = "Town cannot contain numbers")]
        [DataType(DataType.Text)]
        [StringLength(100)]
        public string Town { get; set; }

        [Required]
        [RegularExpression(@"(\d)+", ErrorMessage = "Letters are not allowed")]
        [DataType(DataType.Text)]
        [StringLength(25)]
        public string ZipCode { get; set; }

        [RegularExpression(@"([0-9 \.\(\)/\\\-\+])+", ErrorMessage = "Letters are not allowed")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(25)]
        public string Phone { get; set; }

        [RegularExpression(@"([0-9 \.\(\)/\\\-\+])+", ErrorMessage = "Letters are not allowed")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(25)]
        public string Fax { get; set; }
    }
}
