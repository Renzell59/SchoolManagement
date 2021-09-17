using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Models;
using SchoolManagement.Utilities;

namespace SchoolManagement.ViewModels.Account
{ 
    public class RegisterViewModel
    {
        [Required]
        [Display(Name ="Username")]
        [Remote(action:"isEmailInUse","Account")]
        //[ValidateEmailDomain(requiredEmailDomain:"njit.edu", ErrorMessage ="Please register with using a njit.edu domain")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name ="Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password",ErrorMessage ="Password and Confirm password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
