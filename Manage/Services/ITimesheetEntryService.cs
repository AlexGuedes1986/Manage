using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public interface ITimesheetEntryService
    {
        void Create(TimesheetEntry timesheetEntry);
        void Update(TimesheetEntry timesheetEntry);
        IEnumerable<TimesheetEntry> GetAllByDateRange(DateTime fromDate, DateTime toDate);
        void ApproveByDateRange(DateTime? fromDate, DateTime? toDate, string userId);
        float TotalSpentByTask(List<int> taskUsersId);
        void Remove(TimesheetEntry timesheetEntry);
        IEnumerable<TimesheetEntry> GetAll();
        TimesheetEntry GetById(int id);
    }
}
