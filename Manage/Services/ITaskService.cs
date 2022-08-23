using BioTech.Models;
using BioTech.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public interface ITaskService
    {
        IEnumerable<TaskBioTech> GetAll();
        void Create(TaskBioTech task);
        IEnumerable<TaskBioTech> GetTasksByCategory(string category);
        List<TaskBioTechViewModel> TransformTaskBioTechToTaskBioTechViewModel(IEnumerable<TaskBioTech> tasks
               , List<TaskExtension> tasksAssignedToProposal, int proposalId);
        TaskBioTech GetTaskById(int taskId);
        TaskBioTech GetByName(string taskName);
        void UpdateTask(TaskBioTech taskBioTech);
        void Destroy(TaskBioTech task);
    }
}
