using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.ViewModels
{
    public class TimesheetEntryViewModel
    {
        public int Id { get; set; }
        public int TaskUserId { get; set; }
        public TaskUser TaskUser { get; set; }
        public DateTime DateModified { get; set; }
        public float HoursWorked { get; set; }
        public int HourlyRate { get; set; }
        public bool TaskCompleted { get; set; }
        public string Comment { get; set; }
        public bool RequireComment { get; set; }
        public float RemainingTimeHourlyNotToExceed { get; set; }
        public string TimesheetEntryType { get; set; }
    }
}
