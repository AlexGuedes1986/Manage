using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public interface IProjectTaskUserAssignedService
    {   
        IEnumerable<ProjectTaskUserAssigned> GetUserByProposalIdAndTaskExtensionId(int proposalId, int taskExtensionId);
        void AssignUserToTask(ProjectTaskUserAssigned userTaskAssigned);
        void DestroyByProposalIdAndTaskExtensionId(int proposalId, int taskExtensionId);
        void UnassignUsersToTask(int taskExtensionId);
    }
}
