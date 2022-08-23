using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public interface ITimesheetApprovedDateRangeService
    {
        TimesheetApprovedDateRange GetById(int id);
        void Create(TimesheetApprovedDateRange tsApproved);
        IEnumerable<TimesheetApprovedDateRange> GetByUserId(string userId);
        void Remove(int timesheetApprovedRangeId);
    }
}
