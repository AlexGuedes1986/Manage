using BioTech.Helpers;
using BioTech.Models;
using BioTech.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public interface IProposalService
    {
        void Create(Proposal proposal);
        Proposal TransformFromViewModel(ProposalViewModel model);
        Proposal GetProposalById(int proposalId);
        void UpdateProposal(Proposal proposal);
        IEnumerable<Proposal> GetAll();
        void Destroy(Proposal proposal);        
        //Because KendoGrid issue related to display only flat data we're adding an extra GetAllAndInclude() method with the corresponding Includes
        IEnumerable<Proposal> GetAllAndInclude();
        List<Proposal> GetProposalsByIds(List<int> proposalIds);
        Proposal GetProposalByProjectNumber(string projectNumber);
        int GetLastContractNumberSuffix();
        List<TaskExtension> GetTaskExtensionsByProjectNumber(string projectNumber);
    }
}
