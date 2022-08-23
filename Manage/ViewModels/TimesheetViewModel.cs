using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.ViewModels
{
    public class TimesheetViewModel
    {
        public DateTime? EffectiveDate { get; set; }
        public DateTime StartWeek { get; set; }
        public DateTime EndWeek { get; set; }
        public List<TaskUserViewModel> TaskUserModels { get; set; }
        public bool HasAdminPermission { get; set; }
        public string UserFilteredFormattedName { get; set; }
        public string UserFilteredId { get; set; }
        public string BillingRole { get; set; }
        public bool IsApproved { get; set; }
        public int TimeSheetApprovedRangeId { get; set; }
        public List<UserTimesheetVM> UsersTimesheet { get; set; }
        public float HoursEnteredThisWeek { get; set; }
        public decimal TotalAmountBilledThisWeek { get; set; }

    }
}
