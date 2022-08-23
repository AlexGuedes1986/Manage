using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public class ProjectTaskUserAssignedService : IProjectTaskUserAssignedService
    {
        BioTechContext _db;
        public ProjectTaskUserAssignedService(BioTechContext db)
        {
            _db = db;
        }

        public void AssignUserToTask(ProjectTaskUserAssigned userTaskAssigned)
        {
            _db.ProjectTaskUserAssigned.Add(userTaskAssigned);
            _db.SaveChanges();
        }

        public void UnassignUsersToTask(int taskExtensionId)
        {
            var taskUserRelationsToRemove = _db.ProjectTaskUserAssigned.Where(ptua =>  ptua.TaskExtensionId == taskExtensionId);
            foreach (var taskUserRelationToRemove in taskUserRelationsToRemove)
            {
                _db.ProjectTaskUserAssigned.Remove(taskUserRelationToRemove);
            }
            _db.SaveChanges();
        }

        public IEnumerable<ProjectTaskUserAssigned> GetUserByProposalIdAndTaskExtensionId(int proposalId, int taskExtensionId)
        {
            return _db.ProjectTaskUserAssigned.Where(ptm => ptm.ProposalId == proposalId && ptm.TaskExtensionId == taskExtensionId);
        }

        public void DestroyByProposalIdAndTaskExtensionId(int proposalId, int taskExtensionId)
        {
            var projectTaskUsersAssigned = _db.ProjectTaskUserAssigned.Where(pau => pau.ProposalId == proposalId && pau.TaskExtensionId == taskExtensionId);
            foreach (var ptua in projectTaskUsersAssigned)
            {
                _db.ProjectTaskUserAssigned.Remove(ptua);
            }
            _db.SaveChanges();
        }
    }
}
