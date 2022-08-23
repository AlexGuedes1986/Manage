//using BioTech.Data;
using BioTech.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public class TaskExtensionService : ITaskExtensionService
    {
        BioTechContext _db;
        public TaskExtensionService(BioTechContext db)
        {
            _db = db;
        }

        public void Create(TaskExtension taskExtension)
        {
            _db.TaskExtension.Add(taskExtension);
            _db.SaveChanges();
        }

        public TaskExtension GetById(int taskExtensionId)
        {
            return _db.TaskExtension.Where(te => te.Id == taskExtensionId).FirstOrDefault();
        }

        public void Update(TaskExtension taskExtension)
        {
            _db.TaskExtension.Update(taskExtension);
            _db.SaveChanges();
        }

        public void Destroy(TaskExtension taskExtension)
        {
            _db.TaskExtension.Remove(taskExtension);
            _db.SaveChanges();
        }

        public void AddTasksToProposal(List<TaskExtension> taskExtensions)
        {
            foreach (var taskExtension in taskExtensions)
            {
                _db.TaskExtension.Add(taskExtension);
                _db.SaveChanges();
            }
        }

        public List<TaskExtension> GetByIds(List<int> taskExtensionIds)
        {
            List<TaskExtension> taskExtensions = new List<TaskExtension>();
            foreach (var taskExtensionId in taskExtensionIds)
            {
                taskExtensions.Add(_db.TaskExtension.FirstOrDefault(te => te.Id == taskExtensionId));
            }
            return taskExtensions;
        }

        public List<TaskExtension> GetByProposalId(int proposalId)
        {
            return _db.TaskExtension.Where(te => te.ProposalId == proposalId).ToList();
        }

        public List<TaskExtension> GetTaskExtensionsByProjectNumber(string projectNumber)
        {
            return _db.TaskExtension.Include(t => t.Proposal).Where(p => String.Equals(p.Proposal.ProjectNumber, projectNumber, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
      
    }
}
