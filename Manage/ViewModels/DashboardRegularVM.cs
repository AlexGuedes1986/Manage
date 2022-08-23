using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.ViewModels
{
    public class DashboardRegularVM
    {
        public int Id { get; set; }
        [Display(Name ="Project #")]
        public string ProjectNumber { get; set; }
        [Display(Name="Client")]
        public string ClientName { get; set; }
        [Display(Name="Project Name")]
        public string ProjectName { get; set; }
        [Display(Name="Contract #")]
        public string ContractNumber { get; set; }
        public string TaskNumber { get; set; }
        public string TaskName { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; }
    }
}
