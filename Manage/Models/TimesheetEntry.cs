using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Models
{
    public class TimesheetEntry
    {
        public int Id { get; set; }
        public int TaskUserId { get; set; }
        public TaskUser TaskUser { get; set; }
        public DateTime DateModified { get; set; }
        public float HoursWorked { get; set; }
        public int HourlyRate { get; set; }
        public string Comment { get; set; }
        public bool IsApproved { get; set; }
        public bool InvoiceGenerated { get; set; }
        public int InvoiceId { get; set; }
        public string TimesheetEntryType { get; set; }
    }
}
