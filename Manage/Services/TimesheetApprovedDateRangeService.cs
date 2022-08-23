using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public class TimesheetApprovedDateRangeService : ITimesheetApprovedDateRangeService
    {
        BioTechContext _db;
        public TimesheetApprovedDateRangeService(BioTechContext db)
        {
            _db = db;
        }

        public void Create(TimesheetApprovedDateRange tsApproved)
        {           
            _db.TimesheetApprovedDateRange.Add(tsApproved);
            _db.SaveChanges();
        }

        public TimesheetApprovedDateRange GetById(int id)
        {
            return _db.TimesheetApprovedDateRange.FirstOrDefault(t => t.Id == id);
        }
        public IEnumerable<TimesheetApprovedDateRange> GetByUserId(string userId)
        {
            return _db.TimesheetApprovedDateRange.Where(ta => ta.UserId == userId);
        }

        public void Remove(int timesheetApprovedRangeId)
        {
            var entryToRemove = _db.TimesheetApprovedDateRange.FirstOrDefault(a => a.Id == timesheetApprovedRangeId);
            _db.TimesheetApprovedDateRange.Remove(entryToRemove);
            _db.SaveChanges();
        }

    }
}
