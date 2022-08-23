using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public interface ITaskExtensionService
    {
        void Create(TaskExtension taskExtension);
        TaskExtension GetById(int taskExtensionId);
        void Update(TaskExtension taskExtension);
        void Destroy(TaskExtension taskExtension);
        void AddTasksToProposal(List<TaskExtension> taskExtensions);
        List<TaskExtension> GetByIds(List<int> taskExtensionIds);
        List<TaskExtension> GetByProposalId(int proposalId);
        List<TaskExtension> GetTaskExtensionsByProjectNumber(string projectNumber);
    }
}
