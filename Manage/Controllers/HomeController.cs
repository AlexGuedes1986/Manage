using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BioTech.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using BioTech.Services;
using Microsoft.AspNetCore.Identity;
using BioTech.ViewModels;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using AutoMapper;
using BioTech.Helpers;

namespace BioTech.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {        
        private readonly ILogger<HomeController> _log;
        private readonly ITaskUserService _taskUserService;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITouchLogService _touchLogService;
        private readonly IMapper _mapper;
        private readonly IProposalService _proposalService;
        private readonly IProposalApplicationUserService _proposalApplicationUserService;        

        public HomeController(ILogger<HomeController> log, ITaskUserService taskUserService, IUserService userService
            , UserManager<ApplicationUser> userManager, ITouchLogService touchLogService, IMapper mapper
            , IProposalService proposalService, IProposalApplicationUserService proposalApplicationUserService)
        {
            _log = log;
            _taskUserService = taskUserService;
            _userService = userService;
            _userManager = userManager;
            _touchLogService = touchLogService;
            _mapper = mapper;
            _proposalService = proposalService;
            _proposalApplicationUserService = proposalApplicationUserService;
        }

        public IActionResult Index()

        {
            var currentUserBillingRole = _userManager.FindByNameAsync(User.Identity.Name).Result.BillingRoleId;
            if (currentUserBillingRole <= 4 || currentUserBillingRole == 9 || currentUserBillingRole == 10)
            {
                return View("ExecutiveDashboard");
            }
            else
            {
                return View("RegularDashboard");
            }
        
        }

        public IActionResult DashboardRegular_Read([DataSourceRequest] DataSourceRequest request)
        {            
                List<DashboardRegularVM> dashboardRegularVMs = new List<DashboardRegularVM>();
                var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
                var taskUsersByUserId = _taskUserService.GetAll().Where(tu => tu.UserId == currentUser.Id 
                && !String.IsNullOrEmpty(tu.TaskExtension.Proposal.ProjectNumber)).OrderBy(tu => tu.TaskExtension.DueDate);
                foreach (var taskUser in taskUsersByUserId)
                {
                    dashboardRegularVMs.Add(new DashboardRegularVM
                    {
                        ProjectNumber = taskUser.TaskExtension.Proposal.ProjectNumber,
                        ClientName = taskUser.TaskExtension.Proposal.Contact.Company.Name,
                        ProjectName = taskUser.TaskExtension.Proposal.ProjectName,
                        ContractNumber = taskUser.TaskExtension.Proposal.ContractNumber,
                        TaskNumber = $"{taskUser.TaskExtension.TaskCodeParent}-{taskUser.TaskExtension.TaskCodeSub}",
                        TaskName = taskUser.TaskExtension.TaskTitle,
                        DueDate = taskUser.TaskExtension.DueDate,
                        Status = taskUser.TaskExtension.Status
                    }
                        );
                }
            var dashboardRegularVMsSorted = dashboardRegularVMs.OrderByDescending(p => Convert.ToInt32(p.ContractNumber.Split('-')[0]))
                .ThenByDescending(p => Convert.ToInt32(p.ContractNumber.Split('-')[1])); ;
                var obj = dashboardRegularVMsSorted.ToDataSourceResult(request);
            if (request.Sorts.Any(s => String.Equals(s.Member, "ContractNumber", StringComparison.OrdinalIgnoreCase)))
            {
                CustomSortingHelper.SortByContractNumberDashboardRegular(request, obj);
            }
            return Json(obj);
        }

        public IActionResult DashboardExecutive_Read([DataSourceRequest] DataSourceRequest request)
        {
            var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            List<ExecutiveDashboardKendoVM> executiveDashboardKendoVMs = new List<ExecutiveDashboardKendoVM>();
            var projects = _proposalService.GetAllAndInclude().Where(p => !String.IsNullOrEmpty(p.ProjectNumber));
            foreach (var project in projects)
            {
                var pmsAssigned = _proposalApplicationUserService.GetUserIdsByProposalId(project.Id, "Project Manager");
                var projectTeamMembers = _proposalApplicationUserService.GetByProposalId(project.Id).Where(p => String.Equals(p.Type, "Team Member"
                , StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (projectTeamMembers.Any(ptm => String.Equals(ptm.ApplicationUserId, currentUser.Id)) || String.Equals(project.AuthorId, currentUser.Id)
                    || pmsAssigned.Any(pma => String.Equals(pma, currentUser.Id)))
                {
                    var executiveDashboard = _mapper.Map<ExecutiveDashboardKendoVM>(project);
                    executiveDashboard.Author = $"{_userService.GetById(project.AuthorId).FirstName} {_userService.GetById(project.AuthorId).LastName}";
                    executiveDashboard.ClientCompanyName = project.Contact.Company.Name;
                    var projectPMs = _proposalApplicationUserService.GetByProposalId(project.Id)
                        .Where(pau => pau.Type == "Project Manager");
                    executiveDashboard.ProjectManagers = String.Join(", ", projectPMs.Select(pm => pm.FormattedName))
                        .TrimEnd(' ').TrimEnd(',');
                    executiveDashboardKendoVMs.Add(executiveDashboard);
                }
            }
            var executiveDashboardKendoVMsSorted = executiveDashboardKendoVMs.OrderByDescending(p => Convert.ToInt32(p.ContractNumber.Split('-')[0]))
                .ThenByDescending(p => Convert.ToInt32(p.ContractNumber.Split('-')[1]));
            var obj = executiveDashboardKendoVMsSorted.ToDataSourceResult(request);
            if (request.Sorts.Any(s => String.Equals(s.Member, "ContractNumber", StringComparison.OrdinalIgnoreCase)))
            {
                CustomSortingHelper.SortByContractNumberDashboardExecutive(request, obj);
            }
            return Json(obj);
        }

        public IActionResult DashboardContractStatus_Read([DataSourceRequest] DataSourceRequest request)
        {
            var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            List<ContractStatusVM> contractStatusVMs = new List<ContractStatusVM>();
            var proposals = _proposalService.GetAllAndInclude().Where(p => String.IsNullOrEmpty(p.ProjectNumber));           
            foreach (var proposal in proposals)
            {
                var pmsAssigned = _proposalApplicationUserService.GetUserIdsByProposalId(proposal.Id, "Project Manager");
                var proposalTeamMembers = _proposalApplicationUserService.GetByProposalId(proposal.Id).Where(p => String.Equals(p.Type, "Team Member"
                , StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (proposalTeamMembers.Any(ptm => String.Equals(ptm.ApplicationUserId, currentUser.Id)) || String.Equals(proposal.AuthorId, currentUser.Id)
                    || pmsAssigned.Any(pma => String.Equals(pma, currentUser.Id)))
                {
                    var contractStatus = _mapper.Map<ContractStatusVM>(proposal);
                    contractStatus.Author = $"{_userService.GetById(proposal.AuthorId).FirstName} {_userService.GetById(proposal.AuthorId).LastName}";
                    contractStatus.ClientCompanyName = proposal.Contact.Company.Name;
                    contractStatus.ProposalName = proposal.ProjectName;
                    var proposalPMs = _proposalApplicationUserService.GetByProposalId(proposal.Id)
                      .Where(pau => pau.Type == "Project Manager");
                    contractStatus.ProjectManagers = String.Join(", ", proposalPMs.Select(p => p.FormattedName));
                    contractStatusVMs.Add(contractStatus);
                }
               
            }
            var contractStatusVMSorted = contractStatusVMs.OrderByDescending(cs => cs.DateCreated);    
            var obj = contractStatusVMSorted.ToDataSourceResult(request);
            return Json(obj);
        }

        public IActionResult DashBoardTouchLog_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<TouchLogKendoVM> touchLogKendoVMs = new List<TouchLogKendoVM>();
            var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var touchLogsByCurrentUser = _touchLogService.GetTouchLogsByUserId(currentUser.Id);
            foreach (var touchLog in touchLogsByCurrentUser)
            {
                var touchLogKendoVMMapped = _mapper.Map<TouchLogKendoVM>(touchLog);
                touchLogKendoVMMapped.ContactCompanyName = touchLog.Contact.Company.Name;
                touchLogKendoVMMapped.ContactName = $"{touchLog.Contact.FirstName} {touchLog.Contact.LastName}";
                touchLogKendoVMs.Add(touchLogKendoVMMapped);
            }
            var obj = touchLogKendoVMs.ToDataSourceResult(request);
            return Json(obj);
        }
       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
