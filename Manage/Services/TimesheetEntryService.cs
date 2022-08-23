using BioTech.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public class TimesheetEntryService : ITimesheetEntryService
    {
        BioTechContext _db;
        public TimesheetEntryService(BioTechContext db)
        {
            _db = db;
        }

        public IEnumerable<TimesheetEntry> GetAll()
        {
            return _db.TimesheetEntry.Include(ts => ts.TaskUser).ThenInclude(tu => tu.TaskExtension).ThenInclude(te => te.Proposal)
                    .ThenInclude(p => p.Contact).ThenInclude(c => c.Company);
        }
        public void Create(TimesheetEntry timesheetEntry)
        {
            _db.TimesheetEntry.Add(timesheetEntry);
            _db.SaveChanges();
        }

        public void Update(TimesheetEntry timesheetEntry)
        {
            _db.TimesheetEntry.Update(timesheetEntry);
            _db.SaveChanges();
        }
        public void Remove(TimesheetEntry timesheetEntry)
        {
            _db.TimesheetEntry.Remove(timesheetEntry);
            _db.SaveChanges();
        }

        public IEnumerable<TimesheetEntry> GetAllByDateRange(DateTime fromDate, DateTime toDate)
        {            
                return _db.TimesheetEntry.Include(ts => ts.TaskUser).ThenInclude(tu => tu.TaskExtension).ThenInclude(te => te.Proposal)
                .ThenInclude(p => p.Contact).ThenInclude(c => c.Company)
                .Where(ts => ts.DateModified.Date >= fromDate.Date && ts.DateModified.Date <= toDate.Date);
        }

        public void ApproveByDateRange(DateTime? fromDate, DateTime? toDate, string userId)
        {            
                var timesheetEntriesByDateRange = _db.TimesheetEntry.Include(t => t.TaskUser)
            .Where(ts => ts.DateModified.Date >= fromDate.Value.Date && ts.DateModified.Date <= toDate.Value.Date && String.Equals(ts.TaskUser.UserId, userId)).ToList();
                foreach (var entry in timesheetEntriesByDateRange)
                {
                    entry.IsApproved = true;
                    _db.SaveChanges();
            }        
        }

        public float TotalSpentByTask(List<int> taskUsersId)
        {
            float totalSpent = 0;           
            foreach (var taskUserId in taskUsersId)
            {
                totalSpent += _db.TimesheetEntry.Where(ts => ts.TaskUserId == taskUserId).Sum(ts => ts.HoursWorked * ts.HourlyRate);
            }
            return totalSpent;
        }

        public TimesheetEntry GetById(int id)
        {
            return _db.TimesheetEntry.FirstOrDefault(t => t.Id == id);
        }
    }
}
