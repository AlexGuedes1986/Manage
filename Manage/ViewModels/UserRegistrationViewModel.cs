using BioTech.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.ViewModels
{
    public class UserRegistrationViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        /// <summary>
        /// The role selected from the dropdown
        /// </summary>     
        public string Role { get; set; }

        /// <summary>
        /// A list of dropdown items
        /// </summary>
        public List<RoleUser> WebsiteRoles { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9.]+$", ErrorMessage = "Username should be composed only of letters and numbers")]
        public string UserName { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public List<SelectListItem> BillingRoles { get; set; }
        public int BillingRoleId { get; set; }
        public string BillingRole { get; set; }

    }
}
