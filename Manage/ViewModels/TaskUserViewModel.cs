using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.ViewModels
{
    public class TaskUserViewModel
    {
        public int TaskUserId { get; set; }
        public int ProposalId { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectName { get; set; }
        public string ContractNumber { get; set; }
        public string TaskNumber { get; set; }
        public string TaskTitle { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public List<TimesheetEntry> TimesheetEntries { get; set; }
        public bool RequireComment { get; set; }
        public string FeeStructure { get; set; }
        public string TaskCode { get; set; }
        public float RemainingTimeHourlyNotToExceed { get; set; }
        public bool IsHourlyRateNotExceed { get; set; }
        public DateTime? ProjectDate { get; set; }
    }
}
