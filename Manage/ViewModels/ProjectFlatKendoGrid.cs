using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.ViewModels
{
    public class ProjectFlatKendoGrid
    {
        public int Id { get; set; }
        public int ContactId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectCounty { get; set; }
        public string BTCOffice { get; set; }
        public DateTime? PdfDateCreated { get; set; }
        public DateTime? DateActive { get; set; }
        public bool IsActive { get; set; }
        public bool PdfCreated { get; set; }
        public string ProjectNumber { get; set; }
        public string ContractNumber { get; set; }
        public string CompanyName { get; set; }
        public string ProjectStatus { get; set; }
    }
}
