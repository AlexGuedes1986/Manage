using BioTech.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public class TaskUserService : ITaskUserService
    {
        BioTechContext _db;
        public TaskUserService(BioTechContext db)
        {
            _db = db;
        }
        public void Create(TaskUser taskUser)
        {
            _db.TaskUser.Add(taskUser);
            _db.SaveChanges();
        }
        public List<TaskUser> GetAll()
        {
            return _db.TaskUser.Include(tu => tu.TimesheetEntries).Include(tu => tu.TaskExtension).ThenInclude(te => te.Proposal)
                .ThenInclude(p => p.Contact).ThenInclude(c => c.Company).ToList();
        }
        public void Update(TaskUser taskUser)
        {
            try
            {
                var local = _db.Set<TaskUser>().Local.FirstOrDefault(entry => entry.Id.Equals(taskUser.Id));
                if (local != null)
                {
                    // detach
                    _db.Entry(local).State = EntityState.Detached;
                }

                _db.Entry(taskUser).State = EntityState.Modified;
                _db.TaskUser.Update(taskUser);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                var t = ex;
                throw;
            }        
        }
        public TaskUser GetById(int id)
        {
            return _db.TaskUser.Include(tu => tu.TaskExtension).FirstOrDefault(tu => tu.Id == id);
        }
        public void Remove(TaskUser taskUser)
        {
            _db.TaskUser.Remove(taskUser);
            _db.SaveChanges();
        }
        public void HideFromTimesheet(int taskUserId)
        {
            var taskUserToHide = _db.TaskUser.FirstOrDefault(tu => tu.Id == taskUserId);
            taskUserToHide.DisplayInTimesheet = false;
            taskUserToHide.RemoveFromTimesheetsDate = DateTime.Now;
            _db.SaveChanges();
        }
        public IEnumerable<TaskUser> GetTaskUsersByTaskExtensionId(int taskExtensionId)
        {
            return _db.TaskUser.Where(tu => tu.TaskExtensionId == taskExtensionId);
        }

        public TaskUser GetByTaskExtensionIdAndUserId(int taskExtensionId, string userId)
        {
            return _db.TaskUser.Include(tu => tu.TimesheetEntries).Include(tu => tu.TaskExtension).ThenInclude(te => te.Proposal)
                .ThenInclude(p => p.Contact).ThenInclude(c => c.Company).FirstOrDefault(tu => tu.TaskExtensionId == taskExtensionId && tu.UserId == userId);
        }
    }
}
