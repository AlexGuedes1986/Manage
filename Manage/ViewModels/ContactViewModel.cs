using BioTech.Constants;
using BioTech.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.ViewModels
{
    public class ContactViewModel
    {
        public ContactViewModel()
        {
            AvailableCompanies = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
            AvailableContactTypes = new List<SelectListItem>();
        }
        public int Id { get; set; }
        [Required(ErrorMessage = "Please select a Company")]
        [Display(Name ="Company")]
        public int CompanyId { get; set; }      
        public Company Company { get; set; }
        public List<SelectListItem> AvailableCompanies { get; set; }
        public List<SelectListItem> AvailableStates { get; set; }
        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; }
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
        [Display(Name="Mobile Phone")]
        public string MobilePhone { get; set; }
        [Display(Name="Business Phone")]
        public string BusinessPhone { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Required]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }
        [Display(Name = "Email Address 2")]
        public string EmailAddress2 { get; set; }
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }
        [Display(Name = "Contact Type")]
        [Required]
        public string ContactType { get; set; }
        [Display(Name = "Suffix")]
        public string Suffix { get; set; }
        public List<SelectListItem> AvailableContactTypes { get; set; }
        public bool Active { get; set; } = true;
    }
}
