using BioTech.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.ViewModels
{
    public class TaskExtensionViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Fee Structure")]
        [NotMapped]
        public List<SelectListItem> AvailableFeeStructures { get; set; } = new List<SelectListItem>() { new SelectListItem() { Value = ""
            , Text = "- Select Fee Structure -"}, new SelectListItem() { Value = "Flat Fee", Text = "Flat Fee"}
            , new SelectListItem() { Value = "Hourly", Text = "Hourly" }
            , new SelectListItem() { Value = "Hourly Not To Exceed", Text = "Hourly Not To Exceed" } 
            , new SelectListItem() { Value = "Per Event", Text = "Per Event" } 
            , new SelectListItem() { Value = "Recurring", Text = "Recurring" }};
        [Display(Name = "Event Price")]
        [Range(0.0, Double.MaxValue, ErrorMessage = "The value for Event Price must be equal or greater than zero")]
        public decimal InstancePrice { get; set; }
        [Display(Name = "Frequency of events")]
        [NotMapped]
        public List<SelectListItem> AvailableIntervalTypes { get; set; } = new List<SelectListItem>() { new SelectListItem() { Value = ""
            , Text = "- Select an Interval -"},new SelectListItem() { Value = "Daily", Text = "Daily"}
            , new SelectListItem() { Value = "Weekly", Text = "Weekly" }
            , new SelectListItem() { Value = "Monthly", Text = "Monthly" }
            ,new SelectListItem() { Value = "Bimonthly", Text = "Bimonthly"}               
            , new SelectListItem() { Value = "Quarterly", Text = "Quarterly" }
            , new SelectListItem() { Value = "Bi-anual", Text = "Bi-anual"}
            , new SelectListItem() { Value = "Yearly", Text = "Yearly" }};
        [Display(Name = "Total Number of Events")]
        [Range(0.0, Double.MaxValue, ErrorMessage = "The value for Total Number of Events must be equal or greater than zero")]
        public int NumberOfInstances { get; set; }
        [Display(Name = "Total Price")]
        [Range(0.0, Double.MaxValue, ErrorMessage = "The value for Total Price must be equal or greater than zero")]
        public decimal TotalPrice { get; set; }
        [Display(Name = "Not to Exceed Total Price")]
        [Range(0.0, Double.MaxValue, ErrorMessage = "The value for Not to Exceed Total Price must be equal or greater than zero")]
        public decimal NotToExceedTotalPrice { get; set; }
        public int TaskBioTechId { get; set; }
        public int ProposalId { get; set; }
        [Display(Name = "Fee Structure")]
        public string FeeStructure { get; set; }
        [Display(Name="Frequency")]
        public string IntervalType { get; set; }
        public string FormattedTaskName { get; set; }
        public string TaskDescription { get; set; }
        public int TaskCodeParent { get; set; }
        public int TaskCodeSub { get; set; }
        public string TaskTitle { get; set; }
        public string Category { get; set; }
        public Proposal Proposal { get; set; }
        [MaxLength(500, ErrorMessage = "Note field allows only 500 characters")]
        public string Note { get; set; }
    }
}
