using BioTech.Helpers;
using BioTech.Models;
using BioTech.Services;
using BioTech.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using IronPdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace BioTech.Controllers
{
    public class ProposalController : Controller
    {
        private readonly IContactService _contactService;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITaskService _taskService;
        private readonly IProposalService _proposalService;
        private readonly ITaskExtensionService _taskExtensionService;
        private readonly IProposalApplicationUserService _proposalApplicationUser;
        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IBillingRoleService _billingRoleService;
        private readonly ContractNumberService _contractNumberService;

        public ProposalController(IContactService contactService, IUserService userService, UserManager<ApplicationUser> userManager
            , ITaskService taskService, IProposalService proposalService, ITaskExtensionService taskExtensionService, IMapper mapper,
            IProposalApplicationUserService proposalApplicationUser, IHostingEnvironment hostingEnvironment, RoleManager<ApplicationRole> roleManager,
            IBillingRoleService billingRoleService, ContractNumberService contractNumberService)
        {
            _contactService = contactService;
            _userService = userService;
            _userManager = userManager;
            _taskService = taskService;
            _proposalService = proposalService;
            _taskExtensionService = taskExtensionService;
            _proposalApplicationUser = proposalApplicationUser;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
            _roleManager = roleManager;
            _billingRoleService = billingRoleService;
            _contractNumberService = contractNumberService;
        }

        public IActionResult Index()
        {
            bool canManage = false;
            ApplicationUser appUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var websiteRoles = _userManager.GetRolesAsync(appUser).Result;
            canManage = RolesUserHelper.CanManage(websiteRoles);
            return View(canManage);
        }

        public IActionResult Proposals_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<ProposalViewModel> proposalViewModels = new List<ProposalViewModel>();
            IEnumerable<Proposal> data = _proposalService.GetAllAndInclude().Where(p => String.IsNullOrEmpty(p.ProjectNumber))
                .OrderByDescending(p => p.ContractNumber);
            foreach (var proposal in data)
            {
                proposalViewModels.Add(new ProposalViewModel
                {
                    Id = proposal.Id,
                    Client = proposal.Contact.Company.Name,
                    ProjectName = proposal.ProjectName,
                    ProjectCounty = proposal.ProjectCounty,
                    BTCOffice = proposal.BTCOffice,
                    ContractNumber = proposal.ContractNumber
                });
            }
            var obj = proposalViewModels.ToDataSourceResult(request);
            if (request.Sorts.Any(s => String.Equals(s.Member, "ContractNumber", StringComparison.OrdinalIgnoreCase)))
            {
                CustomSortingHelper.SortByContractNumberProposal(request, obj);
            }
            return Json(obj);
        }

        public IActionResult EditProposal(int id)
        {
            var proposal = _proposalService.GetProposalById(id);
            var userIds = _proposalApplicationUser.GetUserIdsByProposalId(id, "Project Manager");
            ProposalViewModel proposalVM = new ProposalViewModel();
            proposalVM = _mapper.Map<ProposalViewModel>(proposal);
            var pmsAssigned = _userService.GetPmsByIds(userIds);
            var usersWithPMRole = _userService.GetAll().Where(u => u.IsActive && u.BillingRoleId == 4 || u.BillingRoleId == 2 || u.BillingRoleId == 3
          || u.BillingRoleId == 10).ToList();
            var pmsCheckBox = usersWithPMRole.Select(u => new ProjectManagerRoleHelper()
            {
                Id = u.Id,
                IsSelected = false,
                Name = u.FirstName,
                LastName = u.LastName
            }).ToList();
            foreach (var pm in pmsAssigned)
            {
                foreach (var pmCheckBox in pmsCheckBox)
                {
                    if (pmCheckBox.Id == pm.Id)
                    {
                        pmCheckBox.IsSelected = true;
                    }
                }
            }
            proposalVM.ProposalTeamMember = _proposalApplicationUser.GetByProposalId(id).Where(p => String.Equals(p.Type, "Team Member"
                , StringComparison.CurrentCultureIgnoreCase)).ToList();
            proposalVM.ProjectManagersCheckBox = pmsCheckBox;
            proposalVM.Contact = _contactService.GetById(proposalVM.ContactId);
            var author = _userService.GetAllApplicationUsers().Where(u => u.IsActive)
                   .FirstOrDefault(u => u.Id == proposal.AuthorId);
            proposalVM.AuthorFormattedName = $"{author.FirstName} {author.LastName}";
            return View(proposalVM);
        }
        [HttpPost]
        public IActionResult EditProposal(ProposalViewModel proposalVM, string returnUrl)
        {
            //Check if the Proposal/Project Name is already taken
            var allProposals = _proposalService.GetAll();
            if (allProposals.Any(p => p.ProjectName.ToLower() == proposalVM.ProjectName?.ToLower() && p.Id != proposalVM.Id))
            {
                ModelState.AddModelError("ProjectName", "There is already a project with that name");
            }

            if (String.IsNullOrEmpty(proposalVM.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Please select an Author");
            }

            if (proposalVM.ContactId == 0)
            {
                ModelState.AddModelError("ContactId", "Please select a valid client name from the autocomplete list");
            }

            if (ModelState.IsValid)
            {
                Proposal proposal = _mapper.Map<Proposal>(proposalVM);
                _proposalService.UpdateProposal(proposal);
                if (returnUrl == "Update")
                {
                    var existentProposalTeamMembers = _proposalApplicationUser.GetByProposalId(proposalVM.Id).Where(p => String.Equals(p.Type, "Team Member"
            , StringComparison.CurrentCultureIgnoreCase)).ToList();
                    var existentProposalProjectManagers = _proposalApplicationUser.GetByProposalId(proposalVM.Id).Where(p => String.Equals(p.Type, "Project Manager"
                   , StringComparison.CurrentCultureIgnoreCase)).ToList();
                    var teamMembersAdded = proposalVM.ProposalTeamMember?.Select(tm => tm.ApplicationUserId).Where(tm => !existentProposalTeamMembers
                    .Select(eptm => eptm.ApplicationUserId).Contains(tm));
                    var projectManagersAdded = proposalVM.ProjectManagersCheckBox.Where(pm => pm.IsSelected).Select(pm => pm.Id).Where(pm => !existentProposalProjectManagers
                    .Select(eppm => eppm.ApplicationUserId).Contains(pm));
                    var currentDomain = HttpContext.Request;
                    var logoUrl = new Uri($"{currentDomain.Scheme}://{currentDomain.Host}/images/BioTech-logo-f-rgb.jpg").AbsoluteUri;
                    var newTeamMembersAssigned = (teamMembersAdded != null || teamMembersAdded?.Count() > 0) ?
                        _userService.GetPmsByIds(teamMembersAdded.Select(tma => tma).ToList()) : null;
                    var newProjectManagersAssigned = projectManagersAdded != null || projectManagersAdded?.Count() > 0 ?
                        _userService.GetPmsByIds(projectManagersAdded.Select(pma => pma).ToList()) : null;
                    List<EmailNotification> emailsAssigned = new List<EmailNotification>();
                    if (newTeamMembersAssigned != null && newTeamMembersAssigned.Count > 0)
                    {
                        foreach (var teamMember in newTeamMembersAssigned)
                        {
                            emailsAssigned.Add(new EmailNotification
                            {
                                Type = "tm",
                                Email = teamMember.Email
                            });
                        }

                    }

                    if (newProjectManagersAssigned != null && newProjectManagersAssigned.Count > 0)
                    {
                        foreach (var projectManager in newProjectManagersAssigned)
                        {
                            emailsAssigned.Add(new EmailNotification
                            {
                                Type = "pm",
                                Email = projectManager.Email
                            });
                        }
                    }

                    if (emailsAssigned.Count() > 0)
                    {
                        foreach (var emailAssigned in emailsAssigned)
                        {
                            SMTPService.SendEmail(logoUrl, emailAssigned, proposal);
                        }
                    }

                    ViewBag.UpdateMessage = "The proposal was updated successfully";
                    if (proposalVM.ContactId != 0)
                    {
                        proposalVM.Contact = _contactService.GetById(proposalVM.ContactId);
                    }
                    // Cleaning proposalTeamMembers related to proposal being edited.
                    _proposalApplicationUser.DestroyByProposalId(proposalVM.Id);
                    _proposalApplicationUser.SaveProjectManagersToProposal(proposalVM.ProjectManagersCheckBox, proposalVM.Id);
                    var author = _userService.GetAllApplicationUsers().Where(u => u.IsActive)
              .FirstOrDefault(u => u.Id == proposal.AuthorId);
                    proposalVM.AuthorFormattedName = $"{author.FirstName} {author.LastName}";
                    //Adding Team Members to proposal 
                    if (proposalVM.ProposalTeamMember != null)
                    {
                        foreach (var proposalTeamMember in proposalVM.ProposalTeamMember)
                        {
                            proposalTeamMember.ProposalId = proposal.Id;
                            proposalTeamMember.Type = "Team Member";
                            _proposalApplicationUser.CreateTeamMember(proposalTeamMember);
                        }
                    }

                    return View(proposalVM);
                }
                if (returnUrl == "Edit Tasks")
                {
                    return RedirectToAction("EditRemoveTasksFromProposal", new { proposalId = proposal.Id });
                }
                else
                {
                    return RedirectToAction("TasksByCategory", new { proposalId = proposal.Id });
                }
            }
            if (proposalVM.ContactId != 0)
            {
                proposalVM.Contact = _contactService.GetById(proposalVM.ContactId);
            }
            return View("EditProposal", proposalVM);
        }

        [AcceptVerbs("Post")]
        public IActionResult Proposal_Destroy([DataSourceRequest] DataSourceRequest request, Proposal proposal)
        {
            var proposalToDelete = _proposalService.GetProposalById(proposal.Id);
            if (proposalToDelete != null)
            {
                if (!String.IsNullOrEmpty(proposalToDelete.ImageUrl))
                {
                    var root = _hostingEnvironment.ContentRootPath;
                    var PdfImagePath = $"{root}\\Pdfs\\Images\\{proposal.Id}\\";
                    System.IO.DirectoryInfo di = new DirectoryInfo(PdfImagePath);
                    foreach (FileInfo content in di.GetFiles())
                    {
                        content.Delete();
                    }
                    di.Delete();
                }
                _proposalService.Destroy(proposalToDelete);
                _proposalApplicationUser.DestroyByProposalId(proposal.Id);
            }

            return Json(new[] { proposal }.ToDataSourceResult(request, null));
        }

        public IActionResult Create()
        {
            ProposalViewModel model = new ProposalViewModel();
            //Getting users with BillingRole equals Project Manager, Vice President/Director, President, Expert Witness.
            var usersWithPMRole = _userService.GetAllApplicationUsers().Where(u => u.IsActive && (u.BillingRoleId == 4 || u.BillingRoleId == 3
         || u.BillingRoleId == 2 || u.BillingRoleId == 10)).ToList();
            model.ProjectManagersCheckBox = usersWithPMRole.Select(u => new ProjectManagerRoleHelper()
            {
                Id = u.Id,
                IsSelected = false,
                Name = u.FirstName,
                LastName = u.LastName
            }).ToList();
            var newCNSuffix = _proposalService.GetLastContractNumberSuffix() + 1;
            var newCNSuffixFormatted = newCNSuffix.ToString("D3");
            var currentYear = DateTime.Now.ToString("yyyy");
            var contractNumber = $"{currentYear.Substring(currentYear.Length - 2)}-{newCNSuffixFormatted}";
            ContractNumbers contractNbr = new ContractNumbers
            {
                ContractNumber = contractNumber,
                DateCreated = DateTime.Now
            };
            _contractNumberService.Create(contractNbr);
            model.ContractNumber = contractNumber;
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateProposal(ProposalViewModel proposalVM, string requestType)
        {
            //Check if the Proposal/Project Name is already taken
            var allProposals = _proposalService.GetAll();
            if (allProposals.Any(p => p.ProjectName.ToLower() == proposalVM.ProjectName?.ToLower()))
            {
                ModelState.AddModelError("ProjectName", "There is already a project with that name");
            }
            //Assigning Contact to proposalVM to have it loaded when returning the same Create View
            var contact = _contactService.GetById(proposalVM.ContactId);
            proposalVM.Contact = contact;

            //Loading Project Managers in case it's necessary to return the same Create View
            var pmAssignedIds = proposalVM.ProjectManagersCheckBox.Where(pmchb => pmchb.IsSelected).Select(pmchb => pmchb.Id).ToList();
            var tmAssignedIds = proposalVM.ProposalTeamMember != null ? proposalVM.ProposalTeamMember.Select(ptm => ptm.ApplicationUserId)?.ToList()
                : new List<string>();
            var pmsAssigned = _userService.GetPmsByIds(pmAssignedIds);
            var usersWithPMRole = _userService.GetAllApplicationUsers().Where(u => u.IsActive && u.BillingRoleId == 4).ToList();
            var pmsCheckBox = usersWithPMRole.Select(u => new ProjectManagerRoleHelper()
            {
                Id = u.Id,
                IsSelected = false,
                Name = u.FirstName,
                LastName = u.LastName,
                Email = u.Email
            }).ToList();
            foreach (var pm in pmsAssigned)
            {
                foreach (var pmCheckBox in pmsCheckBox)
                {
                    if (pmCheckBox.Id == pm.Id)
                    {
                        pmCheckBox.IsSelected = true;
                    }
                }
            }
            proposalVM.ProjectManagersCheckBox = pmsCheckBox;

            if (String.IsNullOrEmpty(proposalVM.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Please select an Author");
            }
            if (proposalVM.ContactId == 0)
            {
                ModelState.AddModelError("ContactId", "Please select a valid contact name from the autocomplete list");
            }
            var proposal = _mapper.Map<Proposal>(proposalVM);
            if (ModelState.IsValid)
            {
                proposal.DateCreated = DateTime.Now;
                _proposalService.Create(proposal);
                _proposalApplicationUser.SaveProjectManagersToProposal(proposalVM.ProjectManagersCheckBox, proposal.Id);
                if (proposalVM.ProposalTeamMember?.Count > 0)
                {
                    foreach (var proposalTeamMember in proposalVM.ProposalTeamMember)
                    {
                        proposalTeamMember.ProposalId = proposal.Id;
                        proposalTeamMember.Type = "Team Member";
                        _proposalApplicationUser.CreateTeamMember(proposalTeamMember);
                    }
                }

                //Building email receivers list for PMs and Team Members
                List<EmailNotification> emailsAssigned = new List<EmailNotification>();
                foreach (var pm in proposalVM.ProjectManagersCheckBox)
                {
                    if (pm.IsSelected)
                    {
                        emailsAssigned.Add(new EmailNotification
                        {
                            Type = "pm",
                            Email = pm.Email
                        });
                    }
                }
                var teamMembersAssigned = _userService.GetPmsByIds(tmAssignedIds);
                foreach (var teamMember in teamMembersAssigned)
                {
                    emailsAssigned.Add(new EmailNotification
                    {
                        Type = "tm",
                        Email = teamMember.Email
                    });
                }

                var currentDomain = HttpContext.Request;
                var logoUrl = new Uri($"{currentDomain.Scheme}://{currentDomain.Host}/images/BioTech-logo-f-rgb.jpg").AbsoluteUri;

                if (emailsAssigned.Count > 0)
                {
                    foreach (var emailassigned in emailsAssigned)
                    {
                        SMTPService.SendEmail(logoUrl, emailassigned, proposal);
                    }
                }

                return RedirectToAction("TasksByCategory", new { proposalId = proposal.Id });
            }
            //Setting Team Members FirstName and Lastname to Display in case ModelState is not valid.
            if (proposalVM.ProposalTeamMember?.Count > 0)
            {
                foreach (var proposalTeamMember in proposalVM.ProposalTeamMember)
                {
                    var currentTeamMember = _userService.GetAllApplicationUsers().Where(u => u.IsActive)
                       .FirstOrDefault(u => u.Id == proposalTeamMember.ApplicationUserId);
                    proposalTeamMember.FormattedName = $"{currentTeamMember.FirstName} {currentTeamMember.LastName}";
                }
            }
            //Setting Author Formatted name and AuthorId to display in case ModelState is not valid 
            if (proposalVM.AuthorId != null)
            {
                var author = _userService.GetAllApplicationUsers().Where(u => u.IsActive)
                      .FirstOrDefault(u => u.Id == proposal.AuthorId);
                proposalVM.AuthorFormattedName = $"{author.FirstName} {author.LastName}";
            }
            return View("Create", proposalVM);
        }

        public IActionResult ViewProposal(int id)
        {
            var proposal = _proposalService.GetProposalById(id);
            if (!String.IsNullOrEmpty(proposal.ProjectNumber))
            {
                return RedirectToAction("Index", "Project");
            }
            ApplicationUser appUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var author = _userService.GetById(proposal.AuthorId);
            var proposalVM = new ProposalViewModel();
            proposalVM = _mapper.Map<ProposalViewModel>(proposal);
            var userIds = _proposalApplicationUser.GetUserIdsByProposalId(id, "Project Manager");
            var pmsAssigned = _userService.GetPmsByIds(userIds);
            proposalVM.ProjectManagers = pmsAssigned;
            proposalVM.CurrentUserBillingRoleId = appUser.BillingRoleId;
            var websiteRoles = _userManager.GetRolesAsync(appUser).Result;
            proposalVM.CanManage = RolesUserHelper.CanManage(websiteRoles);
            proposalVM.CanActive = RolesUserHelper.CanActive(websiteRoles);
            proposalVM.AuthorFormattedName = $"{author.FirstName} {author.LastName}";
            proposalVM.ProposalTeamMember = _proposalApplicationUser.GetByProposalId(id).Where(p => String.Equals(p.Type, "Team Member"
               , StringComparison.CurrentCultureIgnoreCase)).ToList();
            return View(proposalVM);
        }

        [HttpPost]
        public JsonResult AutoCompleteContact(string prefix)
        {
            var contactsByPrefix = _contactService.GetAll().OrderBy(c => c.FirstName).Where(c => String.Format(c.FirstName + " " + c.LastName)
            .Contains(prefix, StringComparison.InvariantCultureIgnoreCase) && c.Active)
      .Select(obj => new
      {
          label = $"{obj.FirstName} {obj.LastName}",
          val = obj.Id,
          address1 = obj.Company.AddressLine1,
          address2 = obj.Company.AddressLine2,
          city = obj.Company.City,
          state = obj.Company.State,
          zipcode = obj.Company.ZipCode,
          companyName = obj.Company.Name
      });
            return Json(contactsByPrefix);
        }

        [HttpPost]
        public JsonResult AutoCompleteTask(string prefix, string searchType)
        {
            ProposalHelper ph = new ProposalHelper(_proposalService, _mapper);
            if (searchType == "category")
            {
                var tasksByCatPrefix = _taskService.GetAll().Where(t => t.Category.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
              .GroupBy(t => t.Category).Select(obj => new { label = obj.Key });
                return Json(tasksByCatPrefix);
            }
            if (searchType == "taskNumber")
            {
                var tasksByTNPrefix = _taskService.GetAll().Where(t => String.Format(t.TaskCodeParent + "-" + ph.FormatLeadingZeroSearch(t.TaskCodeSub))
             .StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
             .Select(obj => new { label = String.Format("(" + obj.TaskCodeParent + "-" + ph.FormatLeadingZeroSearch(obj.TaskCodeSub) + ") " + obj.TaskTitle), id = obj.Id });

                return Json(tasksByTNPrefix);
            }
            else
            {
                var tasksByTitlePrefix = _taskService.GetAll().Where(t => t.TaskTitle.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
                    .Select(obj => new { label = obj.TaskTitle, id = obj.Id });
                return Json(tasksByTitlePrefix);
            }
        }

        [HttpPost]
        public JsonResult AutocompleteProjectName(string prefix)
        {
            var projectByPrefix = _proposalService.GetAll().Where(p => p.ProjectName.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
                .Select(obj => new { label = obj.ProjectName, val = obj.Id });
            return Json(projectByPrefix);
        }


        [HttpPost]
        public JsonResult AutoCompleteTeamMember(string prefix)
        {
            var teamMemberByPrefix = _userService.GetAllApplicationUsers().Where(u => u.IsActive).Where(c => String.Format(c.FirstName + " " + c.LastName)
            .Contains(prefix, StringComparison.InvariantCultureIgnoreCase))
                .Select(obj => new
                {
                    label = $"{obj.FirstName} {obj.LastName}",
                    val = obj.Id
                });
            return Json(teamMemberByPrefix);
        }

        [HttpPost]
        public JsonResult AutocompleteAuthor(string prefix)
        {
            var usersWithPMRoleByPrefix = _userService.GetAll().Where(u => u.IsActive && (u.BillingRoleId == 4 || u.BillingRoleId == 3
            || u.BillingRoleId == 2 || u.BillingRoleId == 1 || u.BillingRoleId == 10) &&
            String.Format(u.FirstName + " " + u.LastName).Contains(prefix, StringComparison.InvariantCultureIgnoreCase))
                .Select(obj => new
                {
                    label = $"{obj.FirstName} {obj.LastName}",
                    val = obj.Id
                });
            return Json(usersWithPMRoleByPrefix);
        }

        public IActionResult PopulateTasksByProposal(int proposalId)
        {
            var taskExtensions = _proposalService.GetProposalById(proposalId).TaskExtension;
            List<TaskExtensionViewModel> taskExtensionsVM = new List<TaskExtensionViewModel>();
            TaskExtensionViewModel taskExtensionVM = new TaskExtensionViewModel();
            foreach (var taskExtension in taskExtensions)
            {
                taskExtensionVM = _mapper.Map<TaskExtensionViewModel>(taskExtension);
                taskExtensionVM.FormattedTaskName = $"{taskExtensionVM.TaskTitle} ({taskExtensionVM.TaskCodeParent.ToString("00")}-{taskExtensionVM.TaskCodeSub.ToString("00")})";
                taskExtensionsVM.Add(taskExtensionVM);
            }
            return View(taskExtensionsVM);
        }

        public IActionResult PopulateTaskByCategory(string category, int proposalId)
        {
            var tasksAssignedToProposal = _proposalService.GetProposalById(proposalId).TaskExtension.ToList();
            var tasks = _taskService.GetTasksByCategory(category);
            var tasksVM = _taskService.TransformTaskBioTechToTaskBioTechViewModel(tasks, tasksAssignedToProposal, proposalId);
            return View("_PopulateTaskByCategory", tasksVM);
        }

        public IActionResult OpenTask(int taskId)
        {
            var task = _taskService.GetTaskById(taskId);
            TaskExtensionViewModel taskExtensionVM = new TaskExtensionViewModel();
            taskExtensionVM = _mapper.Map<TaskExtensionViewModel>(task);
            taskExtensionVM.TaskBioTechId = task.Id;
            taskExtensionVM.TaskDescription = task.TaskDescription;
            taskExtensionVM.FormattedTaskName = $"{task.TaskTitle} ({task.TaskCodeParent.ToString("00")}-{task.TaskCodeSub.ToString("00")})";
            return View(taskExtensionVM);
        }

        [HttpPost]
        public IActionResult AddTaskToProposal(TaskExtensionViewModel taskExtensionVM)
        {
            if (String.IsNullOrEmpty(taskExtensionVM.FeeStructure))
            {
                ModelState.AddModelError("FeeStructure", "Please select a Fee Structure");
            }
            if (String.Equals(taskExtensionVM.FeeStructure, "recurring", StringComparison.CurrentCultureIgnoreCase))
            {
                if (taskExtensionVM.NumberOfInstances == 0)
                {
                    ModelState.AddModelError("NumberOfInstances", "When the task is recurring you need to set a number of instances.");
                }
                if (String.IsNullOrEmpty(taskExtensionVM.IntervalType))
                {
                    ModelState.AddModelError("IntervalType", "When the task is recurring you need to select an Interval Type.");
                }
            }
            if (ModelState.IsValid)
            {
                TaskExtension taskExtension = new TaskExtension();
                taskExtension = _mapper.Map<TaskExtension>(taskExtensionVM);
                _taskExtensionService.Create(taskExtension);
                return Json(new { success = "success", taskName = taskExtensionVM.TaskTitle, taskId = taskExtension.Id });
            }
            else
            {
                var errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { errors = errorMessages });
            }

        }

        public IActionResult TasksByCategory(int proposalId)
        {
            var proposal = _proposalService.GetProposalById(proposalId);
            var projectName = proposal.ProjectName;
            TaskByCategoryVM taskCatVM = new TaskByCategoryVM
            {
                ProposalId = proposalId,
                ProjectName = projectName,
                Proposal = proposal
            };
            return View(taskCatVM);
        }

        public IActionResult EditRemoveTasksFromProposal(int proposalId)
        {
            ProposalHelper ph = new ProposalHelper(_proposalService, _mapper);
            var taskExtensionVMList = ph.GetTaskExtensionViewModelByProposalId(proposalId);
            if (HttpContext.Session.GetString("message") != null)
            {
                ViewBag.TaskUpdated = HttpContext.Session.GetString("message");
                HttpContext.Session.Remove("message");
            }
            return View(taskExtensionVMList);
        }

        [HttpPost]
        public IActionResult EditRemoveTasksFromProposal(TaskExtensionViewModel taskExtVM, string submitType)
        {
            if (String.IsNullOrEmpty(taskExtVM.FeeStructure))
            {
                ModelState.AddModelError("FeeStructure", "Please select a Fee Structure");
            }
            if (String.Equals(taskExtVM.FeeStructure, "recurring", StringComparison.CurrentCultureIgnoreCase))
            {
                if (taskExtVM.NumberOfInstances == 0)
                {
                    ModelState.AddModelError("NumberOfInstances", "When the task is recurring you need to set a number of instances.");
                }
                if (String.IsNullOrEmpty(taskExtVM.IntervalType))
                {
                    ModelState.AddModelError("IntervalType", "When the task is recurring you need to select an Interval Type.");
                }
            }
            if (ModelState.IsValid)
            {
                var taskExtension = new TaskExtension();
                var message = "";
                taskExtension = _mapper.Map<TaskExtension>(taskExtVM);
                if (submitType.Equals("update", StringComparison.InvariantCultureIgnoreCase))
                {
                    _taskExtensionService.Update(taskExtension);
                    message = "Task have been updated";
                }
                if (submitType.Equals("remove", StringComparison.InvariantCultureIgnoreCase))
                {
                    _taskExtensionService.Destroy(taskExtension);
                }
                HttpContext.Session.SetString("message", message);
                return RedirectToAction("EditRemoveTasksFromProposal", new { proposalId = taskExtension.ProposalId });
            }
            ProposalHelper ph = new ProposalHelper(_proposalService, _mapper);
            var taskExtensionVMList = ph.GetTaskExtensionViewModelByProposalId(taskExtVM.ProposalId);
            return View(taskExtensionVMList);
        }


        [HttpPost]
        public IActionResult AddMultipleTasksToProposal(List<TaskExtensionViewModel> tasksExtensionVM, int proposalId)
        {
            foreach (var taskExtensionVM in tasksExtensionVM)
            {
                //Setting Id = 0 to avoid conflict with EF inserting on entity column.
                taskExtensionVM.Id = 0;
                taskExtensionVM.ProposalId = proposalId;
            }
            List<TaskExtension> taskExtensions = new List<TaskExtension>();
            taskExtensions = _mapper.Map<List<TaskExtension>>(tasksExtensionVM);
            _taskExtensionService.AddTasksToProposal(taskExtensions);
            return Json(new { success = "success" });
        }

        public async Task<IActionResult> PreviewPdf(int id)
        {
            var root = _hostingEnvironment.ContentRootPath;
            var proposal = _proposalService.GetProposalById(id);
            var proposalVM = new ProposalViewModel();
            proposalVM = _mapper.Map<ProposalViewModel>(proposal);
            var userIds = _proposalApplicationUser.GetUserIdsByProposalId(id, "Project Manager");
            var pmsAssigned = _userService.GetPmsByIds(userIds);
            proposalVM.ProjectManagers = pmsAssigned;
            proposalVM.PdfDateCreated = DateTime.Now;
            var author = _userService.GetAllApplicationUsers().Where(u => u.IsActive)
                   .FirstOrDefault(u => u.Id == proposal.AuthorId);
            proposalVM.AuthorBillingRole = _billingRoleService.GetBillingRoleById(author.BillingRoleId).Name;
            proposalVM.AuthorFormattedName = $"{author.FirstName} {author.LastName}";
            var rendererFirstFormat = new HtmlToPdf();
            rendererFirstFormat.PrintOptions.SetCustomPaperSizeInInches(8.50, 11);
            rendererFirstFormat.PrintOptions.MarginLeft = 0;
            rendererFirstFormat.PrintOptions.MarginRight = 0;
            rendererFirstFormat.PrintOptions.MarginTop = 0;
            rendererFirstFormat.PrintOptions.MarginBottom = 0;

            var rendererSecondFormat = new IronPdf.HtmlToPdf();
            rendererSecondFormat.PrintOptions.SetCustomPaperSizeInInches(8.50, 11);
            rendererSecondFormat.PrintOptions.FirstPageNumber = 1;
            var rendererThirdFormat = new IronPdf.HtmlToPdf();
            rendererThirdFormat.PrintOptions.SetCustomPaperSizeInInches(8.50, 11);

            // Build a header using an image asset
            // Note the use of BaseUrl to set a relative path to the assets
            rendererSecondFormat.PrintOptions.Header = new HtmlHeaderFooter()
            {
                Height = 20,
                HtmlFragment = $"<span style='font-style:Times New Roman;font-size:10pt;'>{proposalVM.Contact.FirstName} {proposalVM.Contact.LastName}; " +
                $"{proposalVM.Contact.Company.Name}</span><br/>" +
                $"<span style='font-style:Times New Roman;font-size:10pt;'>{proposalVM.ProjectName} (BTC Proposal # {proposalVM.ContractNumber})</span><br/>",
                //"<span style='font-style:italic'><i>{page} of {total-pages}<i></span>"
            };
            // Build a footer using html to style the text
            // mergable fields are:
            // {page} {total-pages} {url} {date} {time} {html-title} & {pdf-title}
            var currentDomain = HttpContext.Request;
            var imageFooterUri = new Uri($"{currentDomain.Scheme}://{currentDomain.Host}/images/BioTech-logo-2021-rgb-01.jpg").AbsoluteUri;
            var initialsSignatureImage = new Uri($"{currentDomain.Scheme}://{currentDomain.Host}/images/Initials-line.png").AbsoluteUri;

            rendererSecondFormat.PrintOptions.Footer = new HtmlHeaderFooter()
            {
                Height = 15,
                HtmlFragment = $"<img style='display:inline-block;width:60px;height:40px;margin-left:60px;margin-bottom:-13px;' src={initialsSignatureImage}>" +
        $"<span style='display:inline-block;font-family:Times New Roman;font-size:10pt;margin-left:-120px;'>INITIAL:   ___________(BTC)   ___________(Client)" +
        $"</span>" +
        $"<div style='display:inline-block'><img style='width:240px;margin-left:250px;' src={imageFooterUri}></div>"
            };
            rendererThirdFormat.PrintOptions.Footer = new HtmlHeaderFooter()
            {
                Height = 15,
                HtmlFragment = $"<img style='width:240px;margin-left:525px;' src={imageFooterUri}>"
            };
            var rendererImageFormat = new IronPdf.HtmlToPdf();
            rendererImageFormat.PrintOptions.SetCustomPaperSizeInInches(8.50, 11);
            //return View("PdfPages/FirstPage", proposalVM);
            var firstPage = rendererFirstFormat.RenderHtmlAsPdf(await this.RenderViewAsync("PdfPages/FirstPage", proposalVM));
            var counterDescriptionCharacters = 0;
            var taskExtensionCounter = 0;
            bool firstPageCompleted = false;
            List<string> taskPages = new List<string>();
            var counterTaskLooper = 0;
            var lastPosition = 0;
            bool extraPage = false;
            bool wholeExtraPage = false;
            var proposalHelper = new ProposalHelper(_proposalService, _mapper);
            for (int i = 0; i < proposalVM.TaskExtension.Count; i++)
            {
                counterTaskLooper++;
                taskExtensionCounter++;
                counterDescriptionCharacters += proposalHelper.GetTaskCharactersLenth(proposalVM.TaskExtension.ToArray()[i]);

                //Adding a blank line per task and price section.            
                ProposalViewModel proposalVMTemp = new ProposalViewModel();
                proposalVMTemp.ContractNumber = proposalVM.ContractNumber;
                proposalVMTemp.ProjectName = proposalVM.ProjectName;


                //Do not separate tasks by pages since there are tasks descriptions that are bigger than a page.
                var taskDescriptionLength = !String.IsNullOrEmpty(proposalVM.TaskExtension.ToArray()[i].TaskDescription) ?
                    proposalVM.TaskExtension.ToArray()[i].TaskDescription.Length : 0;
                var taskNoteLength = !String.IsNullOrEmpty(proposalVM.TaskExtension.ToArray()[i].Note) ? proposalVM.TaskExtension.ToArray()[i].Note.Length : 0;
                if (taskDescriptionLength + taskNoteLength > 1600)
                {
                    taskPages = new List<string>();
                    taskPages.Add(await this.RenderViewAsync("PdfPages/SecondPage", proposalVM));
                    break;
                }

                if (counterDescriptionCharacters > 3700)
                {
                    if (!firstPageCompleted)
                    {
                        proposalVMTemp.TaskExtension = proposalVM.TaskExtension.Take(i).ToList();
                        proposalVMTemp.ExtraPage = true;
                        taskPages.Add(await this.RenderViewAsync("PdfPages/SecondPage", proposalVMTemp));
                        lastPosition = i;
                        counterTaskLooper = 1;
                        counterDescriptionCharacters = proposalHelper.GetTaskCharactersLenth(proposalVM.TaskExtension.ToList()[i]);
                    }
                    else
                    {
                        if (!wholeExtraPage)
                        {
                            proposalVMTemp.TaskCounter = lastPosition;
                            proposalVMTemp.TaskExtension = proposalVM.TaskExtension.ToList().GetRange(lastPosition, counterTaskLooper - 1);
                            lastPosition = lastPosition + counterTaskLooper - 1;
                            wholeExtraPage = true;
                            counterTaskLooper = 1;
                        }

                        else
                        {
                            proposalVMTemp.TaskCounter = lastPosition;
                            proposalVMTemp.TaskExtension = proposalVM.TaskExtension.ToList().GetRange(lastPosition, counterTaskLooper - 1);
                            lastPosition = lastPosition + counterTaskLooper - 1;
                            counterTaskLooper = 1;
                        }

                        proposalVMTemp.ExtraPage = true;
                        taskPages.Add(await this.RenderViewAsync("PdfPages/ExtraTasks", proposalVMTemp));
                        counterDescriptionCharacters = 0;
                        if (i < proposalVM.TaskExtension.Count)
                        {
                            for (int c = lastPosition; c < i + 1; c++)
                            {
                                counterDescriptionCharacters += proposalHelper.GetTaskCharactersLenth(proposalVM.TaskExtension.ToList()[c]); /* taskDescriptionTemp + taskNoteTemp;*/
                            }
                        }


                    }

                    firstPageCompleted = true;
                    extraPage = true;
                }

                if (counterDescriptionCharacters <= 3700 && i == proposalVM.TaskExtension.Count - 1)
                {
                    if (!firstPageCompleted)
                    {
                        proposalVMTemp.TaskExtension = proposalVM.TaskExtension.Take(i + 1).ToList();
                        taskPages.Add(await this.RenderViewAsync("PdfPages/SecondPage", proposalVMTemp));
                    }
                    else
                    {
                        var taskDescriptionTemp = 0;
                        var taskNoteTemp = 0;
                        var remainingPositionCharsLength = 0;
                        var remainingPositions = proposalVM.TaskExtension.Count - lastPosition;
                        var tempProposal = proposalVM.TaskExtension.ToList().GetRange(lastPosition, remainingPositions);
                        foreach (var taskExt in tempProposal)
                        {
                            taskDescriptionTemp = taskExt.TaskDescription != null ? taskExt.TaskDescription.Length : 0;
                            taskNoteTemp = taskExt.Note != null ? taskExt.Note.Length : 0;
                            remainingPositionCharsLength += taskDescriptionTemp + taskNoteTemp;
                        }
                        if (lastPosition < proposalVM.TaskExtension.Count)
                        {
                            proposalVMTemp.TaskExtension = proposalVM.TaskExtension.ToList().GetRange(lastPosition, remainingPositions);
                            proposalVMTemp.TaskCounter = lastPosition;
                            taskPages.Add(await this.RenderViewAsync("PdfPages/ExtraTasks", proposalVMTemp));
                        }
                    }

                }
            }           
            var thirdPage = rendererSecondFormat.RenderHtmlAsPdf(await this.RenderViewAsyncNoModel("PdfPages/ThirdPage"));
            var fourthPage = rendererSecondFormat.RenderHtmlAsPdf(await this.RenderViewAsyncNoModel("PdfPages/FourthPage"));
            var imagePage = rendererImageFormat.RenderHtmlAsPdf(await this.RenderViewAsync("PdfPages/ImagePage", proposal.ImageUrl));
            var fifthPage = rendererThirdFormat.RenderHtmlAsPdf(await this.RenderViewAsyncNoModel("PdfPages/FifthPage"));
            var sixthPage = rendererThirdFormat.RenderHtmlAsPdf(await this.RenderViewAsyncNoModel("PdfPages/SixthPage"));
            var seventhPage = rendererThirdFormat.RenderHtmlAsPdf(await this.RenderViewAsyncNoModel("PdfPages/SeventhPage"));
            var eighthPage = rendererThirdFormat.RenderHtmlAsPdf(await this.RenderViewAsyncNoModel("PdfPages/EighthPage"));
            List<PdfDocument> allPages = new List<PdfDocument>();
            allPages.Add(firstPage);
            foreach (var taskPage in taskPages)
            {
                allPages.Add(rendererSecondFormat.RenderHtmlAsPdf(taskPage));
            }
            allPages.Add(thirdPage);
            allPages.Add(fourthPage);
            if (!String.IsNullOrEmpty(proposal.ImageUrl))
            {
                allPages.Add(imagePage);
            }
            allPages.Add(fifthPage);
            allPages.Add(sixthPage);
            allPages.Add(seventhPage);
            allPages.Add(eighthPage);
            PdfDocument PDF = PdfDocument.Merge(allPages);
            var pdfName = $"/Pdfs/Proposals/{proposalVM.ContractNumber} {proposalVM.ProjectName}.pdf";
            var pdfRoot = $"{root}{pdfName}";
            PDF.SaveAs(pdfRoot);
            proposal.PdfCreated = true;
            _proposalService.UpdateProposal(proposal);
            var stream = new FileStream(pdfRoot, FileMode.Open);
            return new FileStreamResult(stream, "application/pdf") { FileDownloadName = $"{proposalVM.ContractNumber} {proposalVM.ProjectName}.pdf" };
        }

        [HttpPost]
        public IActionResult ActivateProject(int proposalId)
        {
            var proposal = _proposalService.GetProposalById(proposalId);
            List<string> errorMessages = new List<string>();
            bool isValid = true;
            if (!proposal.PdfCreated)
            {
                errorMessages.Add("- Project pdf needs to be generated before proposal can be activated");
                isValid = false;
            }
            if (isValid)
            {
                proposal.IsActive = true;
                proposal.PdfDateCreated = DateTime.Now;
                _proposalService.UpdateProposal(proposal);
                return Json(new { message = "Project Activated" });
            }
            else
            {
                return Json(new { error = String.Join("\n", errorMessages) });
            }
        }

        [HttpPost]
        public IActionResult SetProjectNumber(int proposalId, string projectNumber)
        {
            var proposal = _proposalService.GetProposalById(proposalId);
            proposal.ProjectNumber = projectNumber;
            proposal.ContractDate = DateTime.Now;
            proposal.ProjectStatus = "Active";
            _proposalService.UpdateProposal(proposal);
            return Json(new { success = "success" });
        }

        public ActionResult GetAllTasks()
        {
            var allTasks = _taskService.GetAll();
            return View(allTasks);
        }

        public ActionResult EditTask(int taskId, int proposalId)
        {
            var proposal = _proposalService.GetProposalById(proposalId);
            var taskAssignedToProposal = _proposalService.GetProposalById(proposalId).TaskExtension.FirstOrDefault(te => te.Id == taskId);
            TaskExtensionViewModel taskExtensionVM = new TaskExtensionViewModel();
            taskExtensionVM = _mapper.Map<TaskExtensionViewModel>(taskAssignedToProposal);
            taskExtensionVM.TaskBioTechId = taskAssignedToProposal.Id;
            taskExtensionVM.TaskDescription = taskAssignedToProposal.TaskDescription;
            taskExtensionVM.FormattedTaskName = $"{taskAssignedToProposal.TaskTitle} ({taskAssignedToProposal.TaskCodeParent.ToString("00")}-{taskAssignedToProposal.TaskCodeSub.ToString("00")})";
            return View(taskExtensionVM);
        }

        [HttpPost]
        public ActionResult EditTask(TaskExtensionViewModel taskExtVM)
        {
            if (String.Equals(taskExtVM.FeeStructure, "recurring", StringComparison.CurrentCultureIgnoreCase))
            {
                if (taskExtVM.NumberOfInstances == 0)
                {
                    ModelState.AddModelError("NumberOfInstances", "When the task is recurring you need to set a number of instances.");
                }
                if (String.IsNullOrEmpty(taskExtVM.IntervalType))
                {
                    ModelState.AddModelError("IntervalType", "When the task is recurring you need to select an Interval Type.");
                }
            }
            if (ModelState.IsValid)
            {
                var taskExtension = new TaskExtension();
                taskExtension = _mapper.Map<TaskExtension>(taskExtVM);
                _taskExtensionService.Update(taskExtension);
                return Json(new { success = "The task was successfully updated" });
            }
            else
            {
                var errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { errors = errorMessages });
            }
        }

        [HttpPost]
        public ActionResult RemoveTaskFromSearchPage(int taskId, int proposalId)
        {
            var proposal = _proposalService.GetProposalById(proposalId);
            var allTasksAssignedToProposal = _proposalService.GetProposalById(proposalId).TaskExtension;
            var taskAssignedToProposal = _proposalService.GetProposalById(proposalId).TaskExtension.FirstOrDefault(te => te.Id == taskId);
            _taskExtensionService.Destroy(taskAssignedToProposal);
            return Json(new { success = "Task has been successfully removed" });
        }

        [HttpPost]
        public ActionResult UploadImage(IFormFile file, int proposalId)
        {
            try
            {
                if (file != null && proposalId != 0)
                {
                    var root = _hostingEnvironment.ContentRootPath;
                    var PdfImagePath = $"{root}\\Pdfs\\Images\\{proposalId}\\";
                    System.IO.Directory.CreateDirectory(PdfImagePath);
                    System.IO.DirectoryInfo di = new DirectoryInfo(PdfImagePath);
                    foreach (FileInfo content in di.GetFiles())
                    {
                        content.Delete();
                    }

                    var filePath = Path.Combine(PdfImagePath, file.FileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    var proposal = _proposalService.GetProposalById(proposalId);
                    proposal.ImageUrl = filePath;
                    _proposalService.UpdateProposal(proposal);
                    return Json(new { message = "Image has been uploaded successfully" });

                }
                return Json(new { error = "Either file is null or proposalId is 0, currently proposalId is " + proposalId });
            }
            catch (Exception ex)
            {
                return Json(new { error = "An exception took place trying to upload image to the pdf: " + ex.Message });
            }

        }

        [HttpPost]
        public ActionResult RemoveImageFromPdf(int proposalId)
        {
            if (proposalId != 0)
            {
                var proposal = _proposalService.GetProposalById(proposalId);
                var root = _hostingEnvironment.ContentRootPath;
                var PdfImagePath = $"{root}\\Pdfs\\Images\\{proposalId}\\";
                System.IO.DirectoryInfo di = new DirectoryInfo(PdfImagePath);
                foreach (FileInfo content in di.GetFiles())
                {
                    content.Delete();
                }
                di.Delete();
                proposal.ImageUrl = "";
                _proposalService.UpdateProposal(proposal);
                TempData["ImageRemoved"] = "Image has been deleted";
                return Json(new { message = "Image has been deleted" });
            }
            else
            {
                return Json(new { message = "An error occurred trying to delete the image" });
            }

        }

    }
}
