using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Models
{
    public class InvoiceTimesheetEntry
    {     
        public int Id { get; set; }
        public string TaskNumber { get; set; }
        public string TaskName { get; set; }
        public string EmployeeId { get; set; }
        public DateTime DateModified { get; set; }
        public string Note { get; set; }
        public decimal Contract { get; set; }
        public decimal RateUsed { get; set; }
        public decimal Prev { get; set; }
        public int Qty { get; set; }
        public decimal TotalPercentage { get; set; } 
        public decimal Amount { get; set; }
        public int TimesheetEntryId { get; set; }
        public int TaskExtensionId { get; set; }
        public string FeeStructure { get; set; }
    }
}
