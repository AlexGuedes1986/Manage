using BioTech.Services;
using BioTech.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BioTech.Models;

namespace BioTech.Helpers
{
    public class ProposalHelper
    {
        private readonly IProposalService _proposalService;
        private readonly IMapper _mapper;
        public ProposalHelper(IProposalService proposalService, IMapper mapper)
        {
            _proposalService = proposalService;
            _mapper = mapper;
        }
        public List<TaskExtensionViewModel> GetTaskExtensionViewModelByProposalId(int proposalId)
        {
            var proposal = _proposalService.GetProposalById(proposalId);
            List<TaskExtensionViewModel> taskExtensionVMList = new List<TaskExtensionViewModel>();
            var taskExtensionList = proposal.TaskExtension.ToList();
            foreach (var taskExtension in taskExtensionList)
            {
                TaskExtensionViewModel taskExtensionVM = new TaskExtensionViewModel();
                taskExtensionVM = _mapper.Map<TaskExtensionViewModel>(taskExtension);
                taskExtensionVM.FormattedTaskName = $"{taskExtensionVM.TaskTitle} ({taskExtensionVM.TaskCodeParent.ToString("00")}" +
                    $"-{taskExtensionVM.TaskCodeSub.ToString("00")})";
                taskExtensionVMList.Add(taskExtensionVM);
            }
            return taskExtensionVMList;
        }

        public string FormatLeadingZeroSearch(int taskCodeSub)
        {
            if (taskCodeSub.ToString().Length == 1)
            {
                return $"0{taskCodeSub}";
            }

            return taskCodeSub.ToString();
        }

        public int GetTaskCharactersLenth(TaskExtension taskExtension)
        {
            var counterDescriptionCharacters = 0;
            counterDescriptionCharacters += !String.IsNullOrEmpty(taskExtension.TaskDescription) ? taskExtension.TaskDescription.Length : 0;
            counterDescriptionCharacters += !String.IsNullOrEmpty(taskExtension.Note) ? taskExtension.Note.Length + 7 : 0;
            counterDescriptionCharacters += !String.IsNullOrEmpty(taskExtension.TaskTitle) ? taskExtension.TaskTitle.Length + 7 : 7;
            if (String.Equals(taskExtension.FeeStructure, "recurring", StringComparison.CurrentCultureIgnoreCase))
            {
                counterDescriptionCharacters += 200;
            }
            //Adding a blank line per task and price section.
            counterDescriptionCharacters += 200;
            return counterDescriptionCharacters;
        }
    }
}
