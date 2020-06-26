using IdentityAuth.Utilities.CustomValidators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAuth.Models
{
    public class LoginUser
    {   
        [Required]
        [EmailAddress]
        [ValidEmailDomain(allowedDomain: "gmail.com",
            ErrorMessage = "Please provide email with domain gmail.com")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
