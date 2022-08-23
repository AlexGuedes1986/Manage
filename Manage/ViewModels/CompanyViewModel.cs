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
    public class CompanyViewModel
    {
        public CompanyViewModel()
        {
            AvailableStates = new List<SelectListItem>();
        }
        public int Id { get; set; }
        [Display(Name = "Company Name")]
        [Required(ErrorMessage = "Company Name is required")]
        public string Name { get; set; }
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }
        [Display(Name = "Company Phone")]
        public string Phone { get; set; }
        [Display(Name = "Company Fax")]
        public string CompanyFax { get; set; }
        public IEnumerable<Contact> Contacts { get; set; }
        public List<SelectListItem> AvailableStates { get; set; }
    }
}
