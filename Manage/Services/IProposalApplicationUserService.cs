using BioTech.Helpers;
using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public interface IProposalApplicationUserService
    {
        List<string> GetUserIdsByProposalId(int proposalId, string type);
        void DestroyByProposalId(int proposalId);
        void DestroyByUserId(string userId);
        List<int> GetProposalIdsByUserId(string userId);
        void SaveProjectManagersToProposal(List<ProjectManagerRoleHelper> PMs, int proposalId);  
        IEnumerable<ProposalApplicationUser> GetByProposalId(int id);
        void CreateTeamMember(ProposalApplicationUser teamMember);
    }
}
