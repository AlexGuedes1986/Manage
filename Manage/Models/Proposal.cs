using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioTech.Models
{
    public partial class Proposal
    {
        public Proposal()
        {
            TaskExtension = new HashSet<TaskExtension>();    
        }

        public int Id { get; set; }
        public int ContactId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectCounty { get; set; }
        public string BTCOffice { get; set; }
        public virtual Contact Contact { get; set; }
        public virtual ICollection<TaskExtension> TaskExtension { get; set; }
        public DateTime? PdfDateCreated { get; set; }
        public DateTime? DateActive { get; set; }
        public bool IsActive { get; set; }
        public bool PdfCreated { get; set; }
        public string ProjectNumber { get; set; }
        public string ContractNumber { get; set; }
        public DateTime DateCreated { get; set; }
        public string AuthorId { get; set; }
        //Date Proposal have been assigned with a Project Number
        public DateTime? ContractDate { get; set; }
        public string SecondParagraphCoverLetter { get; set; }
        public string ImageUrl { get; set; }      
        public string ProjectStatus { get; set; }
        public string ContractStatus { get; set; }
    }
 
}
