using AutoMapper;
using BioTech.Models;
using BioTech.ViewModels;
using Manage.Models;
using Manage.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Contact, ContactViewModel>();
            CreateMap<TaskExtensionViewModel, TaskExtension>();
            CreateMap<TaskBioTech, TaskExtensionViewModel>();
            CreateMap<TaskExtension, TaskExtensionVM>();
            CreateMap<Proposal, ProposalViewModel>().ForMember(pvm => pvm.TaskExtensionVM, opt => opt.MapFrom(src => src.TaskExtension));
            CreateMap<ProposalViewModel, Proposal>();
            CreateMap<TaskExtension, TaskExtensionViewModel>();
            CreateMap<Proposal, ProjectFlatKendoGrid>();
            CreateMap<TimesheetEntryViewModel, TimesheetEntry>();
            CreateMap<TaskEditorViewModel, TaskUser>();
            CreateMap<TouchLog, TouchLogKendoVM>();
            CreateMap<Proposal, ExecutiveDashboardKendoVM>();
            CreateMap<Proposal,ContractStatusVM>();
            CreateMap<TaskBioTech, TaskBioTechVM>();
            CreateMap<TaskBioTechVM, TaskBioTech>();
            CreateMap<Company, CompanyViewModel>();
            CreateMap<InvoiceProject, InvoiceProjectVM>();
            CreateMap<List<InvoiceProject>, List<InvoiceCsv>>();
            CreateMap<ContactClubVM, ContactClub>();
            CreateMap<ContactClub, ContactClubVM>();
        }
    }
}
