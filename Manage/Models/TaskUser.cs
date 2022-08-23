using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Models
{
    public class TaskUser
    {
        public int Id { get; set; }
        public int TaskExtensionId { get; set; }
        public TaskExtension TaskExtension { get; set; }
        public string UserId { get; set; }
        public List<TimesheetEntry> TimesheetEntries { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool Active { get; set; }
        public bool DisplayInTimesheet { get; set; } = true;
        public DateTime? RemoveFromTimesheetsDate { get; set; }
    }
}
