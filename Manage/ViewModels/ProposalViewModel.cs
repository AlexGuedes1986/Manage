                                                                                               using BioTech.Helpers;
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
    public class ProposalViewModel
    {
        public ProposalViewModel()
        {
            ProjectStatuses = new List<SelectListItem> {
            new SelectListItem { Text = "Select Status", Value="" },
            new SelectListItem { Text = "Active", Value="Active" },
            new SelectListItem { Text = "Inactive", Value="Inactive" },
            new SelectListItem { Text = "On Hold", Value="On Hold" },
            };
            ContractStatuses = new List<SelectListItem> {
            new SelectListItem { Text = "Select Status", Value="" },
            new SelectListItem { Text = "Active", Value="Active" },
            new SelectListItem { Text = "Inactive", Value="Inactive" },
            new SelectListItem { Text = "On Hold", Value="On Hold" },
            };
        }
        public int Id { get; set; }
        public Contact Contact { get; set; }
        public int ContactId { get; set; }
        [Display(Name = "Project Name")]
        [Required]
        public string ProjectName { get; set; }
        [Display(Name = "Project County")]
        [Required]
        public string ProjectCounty { get; set; }
        [Display(Name = "BTC Office")]
        [Required]
        public string BTCOffice { get; set; }
        public virtual ICollection<TaskExtension> TaskExtension { get; set; }
        public virtual List<TaskExtensionVM> TaskExtensionVM { get; set; }
        public List<SelectListItem> BTCOfficesList { get; set; } = new List<SelectListItem>() {  new SelectListItem() { Value = "", Text = "- Select Office -"},
            new SelectListItem() { Value = "Jacksonville", Text = "Jacksonville"}, new SelectListItem(){ Value = "Key West", Text = "Key West"}
            , new SelectListItem() { Value = "Orlando", Text = "Orlando" }, new SelectListItem() { Value = "Tampa", Text = "Tampa"}
            , new SelectListItem() { Value = "Vero Beach", Text = "Vero Beach"} };
        public List<ApplicationUser> ProjectManagers { get; set; }
        public List<ProjectManagerRoleHelper> ProjectManagersCheckBox { get; set; }
        public List<ProposalApplicationUser> ProposalTeamMember { get; set; } 
        public IEnumerable<TaskBioTech> TasksSelected { get; set; }
        public DateTime? PdfDateCreated { get; set; }
        public DateTime? DateActive { get; set; }
        public bool IsActive { get; set; }
        public bool PdfCreated { get; set; }
        public string ProjectNumber { get; set; }
        public string ContractNumber { get; set; }
        public int CurrentUserBillingRoleId { get; set; }
        [Display(Name="Assigned to")]
        public string AssignedTo { get; set; }
        public bool CanManage { get; set; }
        [Display(Name ="Author")]
        public string AuthorId { get; set; }
        public string AuthorFormattedName { get; set; }
        public bool CanActive { get; set; }
        public string Client { get; set; }
        //Date Proposal have been assigned with a Project Number
        public DateTime? ContractDate { get; set; }
        public string SecondParagraphCoverLetter { get; set; }
        public DateTime DateCreated { get; set; }
        public int TaskCounter { get; set; }
        public bool ExtraPage { get; set; }
        public string AuthorBillingRole { get; set; }
        public string ImageUrl { get; set; }
        public List<SelectListItem> ProjectStatuses { get; set; } = new List<SelectListItem> { };
        public List<SelectListItem> ContractStatuses { get; set; } = new List<SelectListItem> { };
        public string ProjectStatus { get; set; }
        public string ContractStatus { get; set; }
    }
}
