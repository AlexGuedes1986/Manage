using BioTech.Helpers;
using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public class ProposalApplicationUserService : IProposalApplicationUserService
    {
        BioTechContext _db;
        public ProposalApplicationUserService(BioTechContext db)
        {
            _db = db;
        }

        public void DestroyByProposalId(int proposalId)
        {
            var proposalApplicationUser = _db.ProposalApplicationUser.Where(pau => pau.ProposalId == proposalId).ToList();
            foreach (var pau in proposalApplicationUser)
            {
                _db.ProposalApplicationUser.Remove(pau);
                _db.SaveChanges();
            }           
        }

        public List<string> GetUserIdsByProposalId(int proposalId, string type)
        {
            return _db.ProposalApplicationUser.Where(pau => pau.ProposalId == proposalId 
            && String.Equals(pau.Type, type, StringComparison.CurrentCultureIgnoreCase)).Select(pau => pau.ApplicationUserId).ToList();
        }

        public void DestroyByUserId(string userId)
        {
            var proposalApplicationUser = _db.ProposalApplicationUser.Where(pau => pau.ApplicationUserId == userId);
            foreach (var pau in proposalApplicationUser)
            {
                _db.ProposalApplicationUser.Remove(pau);
            }
            _db.SaveChanges();
        }

        public List<int> GetProposalIdsByUserId(string userId)
        {
            return  _db.ProposalApplicationUser.Where(pau => String.Equals(pau.ApplicationUserId, userId)).Select(pau => pau.ProposalId).ToList();
        }

        public void SaveProjectManagersToProposal(List<ProjectManagerRoleHelper> PMs, int proposalId)
        {
            List<ProposalApplicationUser> proposalApplicationUserList = new List<ProposalApplicationUser>();
            //Iterating over the list of Project Managers and picking the ones that are selected to save their Id directly on the
            //ProposalApplicationUser table
            foreach (var pm in PMs)
            {
                if (pm.IsSelected == true)
                {
                    _db.ProposalApplicationUser.Add(new ProposalApplicationUser()
                    {
                        ProposalId = proposalId,
                        ApplicationUserId = pm.Id,
                        Type = "Project Manager",
                        FormattedName = $"{pm.Name} {pm.LastName}"
                    });
                }
            }
            _db.SaveChanges();
        }    

       public IEnumerable<ProposalApplicationUser> GetByProposalId(int id)
        {
            return _db.ProposalApplicationUser.Where(ptm => ptm.ProposalId == id);
        }

        public void CreateTeamMember(ProposalApplicationUser teamMember)
        {
            _db.ProposalApplicationUser.Add(teamMember);
            _db.SaveChanges();
        }

    }
}
