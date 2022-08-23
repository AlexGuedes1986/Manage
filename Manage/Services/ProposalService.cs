using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using BioTech.Data;
using BioTech.Helpers;
using BioTech.Models;
using BioTech.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BioTech.Services
{
    public class ProposalService : IProposalService
    {
        BioTechContext _db;
        public ProposalService(BioTechContext db)
        {
            _db = db;
        }

        public void UpdateProposal(Proposal proposal)
        {
            var local = _db.Set<Proposal>().Local.FirstOrDefault(entry => entry.Id.Equals(proposal.Id));
            if (local != null)
            {
                // detach
                _db.Entry(local).State = EntityState.Detached;
            }

            _db.Entry(proposal).State = EntityState.Modified;

            _db.Proposal.Update(proposal);
            _db.SaveChanges();
        }

        public void Create(Proposal proposal)
        {
            _db.Proposal.Add(proposal);
            _db.SaveChanges();
        }

        public Proposal GetProposalById(int proposalId)
        {
            return _db.Proposal.Where(p => p.Id == proposalId).Include(p => p.TaskExtension).Include(p => p.Contact)
                .ThenInclude(c => c.Company).FirstOrDefault();
        }

        public Proposal TransformFromViewModel(ProposalViewModel model)
        {
            Proposal proposal = new Proposal()
            {
                ContactId = model.ContactId,
                ProjectName = model.ProjectName,
                ProjectCounty = model.ProjectCounty,
                BTCOffice = model.BTCOffice,
                AuthorId = model.AuthorId,
                SecondParagraphCoverLetter = model.SecondParagraphCoverLetter
            };
            return proposal;
        }

        public int GetLastContractNumberSuffix()
        {
            var lastSuffix = 0;
            var currentNumber = 0;
            List<string> proposalContractNumbers = new List<string>();
            var allContractNumbers = _db.ContractNumbers.ToList();
            if (allContractNumbers.Count == 0)
            {
                var allProposals = _db.Proposal.ToList();
                foreach (var proposal in allProposals)
                {
                    if (!String.IsNullOrEmpty(proposal.ContractNumber))
                    {
                        proposalContractNumbers.Add(proposal.ContractNumber);
                    }
                }
            }
            else
            {
                foreach (var contractNumber in allContractNumbers)
                {
                    if (!String.IsNullOrEmpty(contractNumber.ContractNumber))
                    {
                        proposalContractNumbers.Add(contractNumber.ContractNumber);
                    }
                }
            }

            var currentYearNumbers = proposalContractNumbers.Where(p => p.StartsWith($"{DateTime.Now.ToString("yy")}"));
            foreach (var contractNumber in currentYearNumbers)
            {
                currentNumber = Convert.ToInt32(contractNumber.Split('-')[1]);
                lastSuffix = currentNumber > lastSuffix ? currentNumber : lastSuffix;
            }           
            return lastSuffix;          
        }

        public IEnumerable<Proposal> GetAll()
        {
            return _db.Proposal.ToList();
        }

        //Because KendoGrid issue related to display only flat data we're adding an extra GetAllAndInclude() method with the corresponding Includes
        public IEnumerable<Proposal> GetAllAndInclude()
        {
            return _db.Proposal.Include(p => p.TaskExtension).Include(p => p.Contact).ThenInclude(c => c.Company).ToList();
        }

        public void Destroy(Proposal proposal)
        {
            _db.Proposal.Remove(proposal);
            _db.SaveChanges();
        }

        public List<Proposal> GetProposalsByIds(List<int> proposalIds)
        {
            List<Proposal> proposals = new List<Proposal>();
            foreach (var proposalId in proposalIds)
            {
                proposals.Add(_db.Proposal.Include(p => p.TaskExtension).Include(p => p.Contact).ThenInclude(p => p.Company).FirstOrDefault(p => p.Id == proposalId));
            }
            return proposals;
        }

        public Proposal GetProposalByProjectNumber(string projectNumber)
        {
            return _db.Proposal.Include(p => p.TaskExtension).Include(p => p.Contact).ThenInclude(p => p.Company)
                .Where(p => p.IsActive).FirstOrDefault(p => p.ProjectNumber == projectNumber);
        }

        public List<TaskExtension> GetTaskExtensionsByProjectNumber(string projectNumber)
        {
            return _db.Proposal.Where(p => p.IsActive && String.Equals(p.ProjectNumber, projectNumber, StringComparison.OrdinalIgnoreCase))
                   .Select(p => p.TaskExtension).SelectMany(t => t).ToList();
        }
    }
}
