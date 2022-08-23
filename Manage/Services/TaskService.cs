using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using BioTech.Data;
using BioTech.Models;
using BioTech.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BioTech.Services
{
    public class TaskService : ITaskService
    {
        BioTechContext _db;
        public TaskService(BioTechContext db)
        {
            _db = db;
        }
        public void Create(TaskBioTech task)
        {
            _db.TaskBioTech.Add(task);
            _db.SaveChanges();
        }

        public IEnumerable<TaskBioTech> GetAll()
        {
            return _db.TaskBioTech;
        }

        public TaskBioTech GetTaskById(int taskId)
        {
            return _db.TaskBioTech.Where(t => t.Id == taskId).FirstOrDefault();
        }

        public IEnumerable<TaskBioTech> GetTasksByCategory(string category)
        {
            return _db.TaskBioTech.Where(t => t.Category == category);
        }

        public List<TaskBioTechViewModel> TransformTaskBioTechToTaskBioTechViewModel(IEnumerable<TaskBioTech> tasks
            , List<TaskExtension> tasksAssignedToProposal, int proposalId)
        {
            List<TaskBioTechViewModel> tasksVM = new List<TaskBioTechViewModel>();
            foreach (var task in tasks)
            {
                TaskBioTechViewModel taskBTVM = new TaskBioTechViewModel {
                    Id = task.Id,
                    IsAddedToProject = tasksAssignedToProposal.Select(ta => ta.TaskTitle).Contains(task.TaskTitle) ? true : false,
                    TaskTitle = task.TaskTitle,
                    TaskDescription = task.TaskDescription
                };           
                foreach (var taskAssigned in tasksAssignedToProposal)
                {
                    if (String.Equals(taskAssigned.TaskTitle, task.TaskTitle, StringComparison.OrdinalIgnoreCase))
                    {
                        taskBTVM.TaskIdAssignedToProposal = taskAssigned.Id;
                    }
                }
                tasksVM.Add(taskBTVM);
            }
            return tasksVM;
        }

        public TaskBioTech GetByName(string taskName)
        {
            return _db.TaskBioTech.FirstOrDefault(t => String.Equals(t.TaskTitle, taskName, StringComparison.OrdinalIgnoreCase));
        }

        public void UpdateTask(TaskBioTech taskBioTech)
        {
            var local = _db.Set<TaskBioTech>().Local.FirstOrDefault(entry => entry.Id.Equals(taskBioTech.Id));
            if (local != null)
            {
                // detach
                _db.Entry(local).State = EntityState.Detached;
            }

            _db.Entry(taskBioTech).State = EntityState.Modified;

            _db.TaskBioTech.Update(taskBioTech);
            _db.SaveChanges();
        }
        public void Destroy(TaskBioTech task)
        {
            _db.TaskBioTech.Remove(task);
            _db.SaveChanges();
        }
    }
}
