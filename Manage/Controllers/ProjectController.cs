using AutoMapper;
using BioTech.Helpers;
using BioTech.Models;
using BioTech.Services;
using BioTech.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IProposalService _proposalService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProposalApplicationUserService _proposalApplicationUser;
        private readonly IUserService _userService;
        private readonly ITaskExtensionService _taskExtensionService;
        private readonly ITaskUserService _taskUserService;
        private readonly IProjectTaskUserAssignedService _projectTaskUserAssignedService;
        public ProjectController(IProposalService proposalService, IMapper mapper, UserManager<ApplicationUser> userManager
            , IProposalApplicationUserService proposalApplicationUser, IUserService userService
            , ITaskExtensionService taskExtensionService, ITaskUserService taskUserService, IProjectTaskUserAssignedService projectTaskUserAssignedService)
        {
            _mapper = mapper;
            _proposalService = proposalService;
            _userManager = userManager;
            _proposalApplicationUser = proposalApplicationUser;
            _userService = userService;
            _taskExtensionService = taskExtensionService;
            _taskUserService = taskUserService;
            _projectTaskUserAssignedService = projectTaskUserAssignedService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Projects_Read([DataSourceRequest] DataSourceRequest request)
        {
            ApplicationUser appUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var websiteRoles = _userManager.GetRolesAsync(appUser).Result;
            List<Proposal> projects = new List<Proposal>();
            var currentUserId = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;
            var projectIdsByUser = _proposalApplicationUser.GetProposalIdsByUserId(currentUserId);
            projectIdsByUser.AddRange(_proposalService.GetAll().Where(p => p.AuthorId == currentUserId).Select(p => p.Id).Except(projectIdsByUser));
            if (websiteRoles.Contains("Admin"))
            {
                projects = _proposalService.GetAllAndInclude().Where(p => p != null && p.IsActive && !String.IsNullOrEmpty(p.ProjectNumber)).ToList();
            }
            else
            {
                projects = _proposalService.GetProposalsByIds(projectIdsByUser).Where(p => p != null && p.IsActive
                && !String.IsNullOrEmpty(p.ProjectNumber)).ToList();
            }
            List<ProjectFlatKendoGrid> projectsKendo = new List<ProjectFlatKendoGrid>();
            ProjectFlatKendoGrid project = new ProjectFlatKendoGrid();
            foreach (var proposal in projects)
            {
                project = _mapper.Map<ProjectFlatKendoGrid>(proposal);
                project.CompanyName = $"{proposal.Contact.Company?.Name}";
                projectsKendo.Add(project);
            }                
            var obj = projectsKendo.ToDataSourceResult(request);
            if (request.Sorts.Any(s => String.Equals(s.Member, "ContractNumber", StringComparison.OrdinalIgnoreCase)))
            {
                CustomSortingHelper.SortByContractNumberProject(request, obj);
            }
            if (request.Sorts.Any(s => String.Equals(s.Member, "ProjectNumber", StringComparison.OrdinalIgnoreCase)))
            {
                CustomSortingHelper.SortByProjectNumber(request, obj);
            }           
            return Json(obj);
        }

        public IActionResult ProjectDetails(int id)
        {          
            var project = _proposalService.GetProposalById(id);
            //Assigning for now TaskExtensions by Project Number
            project.TaskExtension = _taskExtensionService.GetTaskExtensionsByProjectNumber(project.ProjectNumber);
            ApplicationUser appUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var projectVM = new ProposalViewModel();
            projectVM = _mapper.Map<ProposalViewModel>(project);
            for (int i = 0; i < projectVM.TaskExtensionVM.Count; i++)
            {
                projectVM.TaskExtensionVM[i].UsersAssignedToTask = _projectTaskUserAssignedService.GetUserByProposalIdAndTaskExtensionId(project.Id, projectVM.TaskExtensionVM[i].Id);
                if (projectVM.TaskExtensionVM[i].UsersAssignedToTask.Count() > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    var counter = 0;
                    foreach (var userAssigned in projectVM.TaskExtensionVM[i].UsersAssignedToTask)
                    {
                        if (counter % 2 != 0)
                        {
                            sb.Append($"<b>{userAssigned.FormattedName}</b><br>");
                        }
                        else
                        {
                            sb.Append($"{userAssigned.FormattedName}<br>");
                        }

                        counter++;
                    }
                    var usersFormattedName = sb.ToString();
                    projectVM.TaskExtensionVM[i].UsersAssignedFormattedName = usersFormattedName.Substring(0, usersFormattedName.Length - 4);
                }
                else
                {
                    projectVM.TaskExtensionVM[i].UsersAssignedFormattedName = "";
                }
            }

            var userIds = _proposalApplicationUser.GetUserIdsByProposalId(project.Id, "Project Manager");
            var pmsAssigned = _userService.GetPmsByIds(userIds);
            projectVM.ProjectManagers = pmsAssigned;
            projectVM.CurrentUserBillingRoleId = appUser.BillingRoleId;
            projectVM.ProjectNumber = project.ProjectNumber;
            var websiteRoles = _userManager.GetRolesAsync(appUser).Result;
            projectVM.CanManage = RolesUserHelper.CanManage(websiteRoles);
            projectVM.CanActive = RolesUserHelper.CanActive(websiteRoles);
            return View(projectVM);
        }

        public IActionResult TaskEditor(int taskExtId)
        {
            var taskExtension = _taskExtensionService.GetById(taskExtId);
            ApplicationUser appUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            TaskEditorViewModel taskEditorVM = new TaskEditorViewModel()
            {
                TaskExtensionId = taskExtension.Id,
                TaskNumber = $"{taskExtension.TaskCodeParent.ToString("00")}-{taskExtension.TaskCodeSub.ToString("00")}",
                TaskName = taskExtension.TaskTitle,
                DueDate = DateTime.Now,
                Description = taskExtension.TaskDescription,
                TaskUsersAssigned = new List<ProjectTaskUserAssigned> { new ProjectTaskUserAssigned {
                    ApplicationUserId = appUser.Id,
                    FormattedName = $"{appUser.FirstName} {appUser.LastName}",
                    ProposalId = taskExtension.ProposalId,
                    TaskExtensionId = taskExtension.Id
                } },           
            };

            return View(taskEditorVM);
        }

        [HttpPost]
        public IActionResult TaskEditor(TaskEditorViewModel taskEditorViewModel)
        {
            var currentTaskExtension = _taskExtensionService.GetById(taskEditorViewModel.TaskExtensionId);
            currentTaskExtension.RequireComment = taskEditorViewModel.RequireComment;
            currentTaskExtension.DueDate = taskEditorViewModel.DueDate;
            currentTaskExtension.Note = taskEditorViewModel.Note;
            currentTaskExtension.Status = taskEditorViewModel.Status;
            _taskExtensionService.Update(currentTaskExtension);
            TaskUser taskUser = new TaskUser()
            {
                TaskExtensionId = taskEditorViewModel.TaskExtensionId,
                UserId = _userManager.FindByNameAsync(User.Identity.Name).Result.Id
            };
            _taskUserService.Create(taskUser);
            var proposalId = _taskExtensionService.GetById(taskEditorViewModel.TaskExtensionId).ProposalId;
            var projectNumber = _proposalService.GetProposalById(proposalId).ProjectNumber;
            return RedirectToAction("ProjectDetails", new { id = proposalId });
        }

        public IActionResult ManageTask(int taskExtensionId, int projectId, string editOrCreate
            , string assignedEmployeeId, string projectNumber)
        {
            var manageTask = new ManageTaskViewModel();
            var allProjects = _proposalService.GetAllAndInclude().Where(p => p.IsActive).ToList();
            var currentUserId = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;
            if (taskExtensionId != 0)
            {
                manageTask.TaskDefaultId = taskExtensionId;
                var taskExtension = _taskExtensionService.GetById(taskExtensionId);
                manageTask.ProjectDefaultId = taskExtension == null ? 0 : taskExtension.ProposalId;
                var currentTaskUser = _taskUserService.GetAll().Where(tu => tu.TaskExtensionId == taskExtensionId).FirstOrDefault();
                //Building model to Edit on view
                if (String.Equals(editOrCreate, "edit"))
                {
                    var usersAssignedToTask = _projectTaskUserAssignedService.GetUserByProposalIdAndTaskExtensionId(taskExtension.ProposalId, taskExtensionId);
                    if (usersAssignedToTask.Count() == 1 && usersAssignedToTask.FirstOrDefault().ApplicationUserId == currentUserId)
                    {
                        manageTask.AssignToMySelf = true;                      
                    }                   
                    manageTask.Id = currentTaskUser.Id;
                    manageTask.Status = taskExtension.Status;
                    manageTask.DueDate = taskExtension.DueDate;
                    manageTask.Note = taskExtension.Note;
                    manageTask.RequireComment = taskExtension.RequireComment;
                    manageTask.TypeOfView = "Edit";
                    manageTask.EffectiveDate = currentTaskUser.EffectiveDate;                     
                }
                else
                {
                    manageTask.TypeOfView = "Assign";
                }     
                manageTask.UsersAssignedToTask = _projectTaskUserAssignedService.GetUserByProposalIdAndTaskExtensionId(taskExtension.ProposalId, taskExtensionId).ToList();
                manageTask.DisplayInTimesheet = currentTaskUser != null ? currentTaskUser.DisplayInTimesheet : false;
            }
            if (projectId != 0)
            {
                manageTask.ProjectDefaultId = projectId;
            }

            manageTask.Projects = allProjects;
            manageTask.ProjectNumber = projectNumber;
            return View(manageTask);
        }

        public JsonResult GetProjectTasks(int proposalId)
        {
            var taskExtensions = _taskExtensionService.GetByProposalId(proposalId);
            return Json(new { taskExtensions = taskExtensions });
        }

        [HttpPost]
        public JsonResult AssignTaskToUser(TaskEditorViewModel taskEditorVM, string createOrUpdate)
        {
            var message = "";
            List<string> validationErrors = new List<string>();
            bool isValid = true;
            var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            if (taskEditorVM.AssignToMyself)
            {
                taskEditorVM.TaskUsersAssigned.Add(new ProjectTaskUserAssigned
                {
                    ApplicationUserId = currentUser.Id,
                    FormattedName = $"{currentUser.FirstName} {currentUser.LastName}",
                    ProposalId = taskEditorVM.ProposalId,
                    TaskExtensionId = taskEditorVM.TaskExtensionId
                });         
            }
            if (taskEditorVM.TaskUsersAssigned.Count == 0 && !taskEditorVM.AssignToMyself)
            {
                isValid = false;
                validationErrors.Add("- Task should be assigned to a valid user");
            }
            if (String.IsNullOrEmpty(taskEditorVM.Status))
            {
                isValid = false;
                validationErrors.Add("- Please select a valid status");
            }
            if (isValid)
            {               
                var currentTaskExtension = _taskExtensionService.GetById(taskEditorVM.TaskExtensionId);
                currentTaskExtension.DueDate = DateTime.Today.AddDays(30);
                currentTaskExtension.Status = taskEditorVM.Status;
                currentTaskExtension.Note = taskEditorVM.Note;
                currentTaskExtension.RequireComment = taskEditorVM.RequireComment;
                currentTaskExtension.AlreadyAddedToTimesheet = true; 
                _taskExtensionService.Update(currentTaskExtension);
                _projectTaskUserAssignedService.DestroyByProposalIdAndTaskExtensionId(taskEditorVM.ProposalId, taskEditorVM.TaskExtensionId);
                if (taskEditorVM.TaskUsersAssigned?.Count > 0)
                {
                    foreach (var assignedUser in taskEditorVM.TaskUsersAssigned)
                    {
                        assignedUser.ProposalId = taskEditorVM.ProposalId;
                        assignedUser.TaskExtensionId = taskEditorVM.TaskExtensionId;
                        _projectTaskUserAssignedService.AssignUserToTask(assignedUser);
                    }
                }
                if (String.Equals(taskEditorVM.Status, "complete", StringComparison.OrdinalIgnoreCase))
                {
                    currentTaskExtension.TaskCompleted = true;
                }
             
                    foreach (var tua in taskEditorVM.TaskUsersAssigned)
                    {
                        TaskUser taskUser = new TaskUser();
                        taskUser.EffectiveDate = taskEditorVM.EffectiveDate;
                        taskUser.Active = true;
                        taskUser.DisplayInTimesheet = true;                       
                        var oldTaskUser = _taskUserService.GetByTaskExtensionIdAndUserId(tua.TaskExtensionId, tua.ApplicationUserId);                      
                        taskUser.TaskExtensionId = tua.TaskExtensionId;
                        taskUser.UserId = tua.ApplicationUserId;
                        taskUser.DisplayInTimesheet = taskEditorVM.DisplayInTimesheet;
                        if (oldTaskUser == null)
                        {                           
                            _taskUserService.Create(taskUser);
                            message = "Changes have been saved";
                        }
                        else
                        {
                            taskUser.Id = taskEditorVM.Id;
                            _taskUserService.Update(taskUser);
                            message = "Changes have been saved";
                        }
                    }
               
                return Json(new { message });
            }
            return Json(new { error = String.Join("\n", validationErrors) });
        }

        [HttpPost]
        public JsonResult AutoCompleteAssignTaskToUser(string prefix)
        {
            var teamMemberByPrefix = _userService.GetAllApplicationUsers().Where(u => u.IsActive).Where(u => u.FirstName.Contains(prefix
               , StringComparison.InvariantCultureIgnoreCase) || u.LastName.Contains(prefix, StringComparison.InvariantCultureIgnoreCase))
               .Select(obj => new
               {
                   label = $"{obj.FirstName} {obj.LastName}",
                   val = obj.Id
               });
            return Json(teamMemberByPrefix);
        }

        [HttpPost]
        public IActionResult AddTaskToTimesheetRegularUser(TaskUser taskUser)
        {
            var errorMessage = "";
            bool isValid = true;
            var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var currentUserId = currentUser.Id;
            taskUser.UserId = currentUserId;
            var allTaskUsers = _taskUserService.GetAll();
            if (allTaskUsers.Any(tu => tu.TaskExtensionId == taskUser.TaskExtensionId))
            {
                errorMessage = "This task was assigned already";
                isValid = false;
            }
            if (isValid)
            {
                var currentTaskExtension = _taskExtensionService.GetById(taskUser.TaskExtensionId);
                currentTaskExtension.DueDate = DateTime.Today.AddDays(30);
                currentTaskExtension.Status = "Active";
                currentTaskExtension.FormattedNameAssignedTo = $"{currentUser.FirstName} {currentUser.LastName}";
                currentTaskExtension.AlreadyAddedToTimesheet = true;
                currentTaskExtension.AssignedEmployeeId = currentUser.Id;
                taskUser.Active = true;
                taskUser.EffectiveDate = DateTime.Now;
                ProjectTaskUserAssigned projectTaskUserAssigned = new ProjectTaskUserAssigned {
                    ProposalId = currentTaskExtension.ProposalId,
                    ApplicationUserId = taskUser.UserId,
                    FormattedName = $"{currentUser.FirstName} {currentUser.LastName}",
                    TaskExtensionId = taskUser.TaskExtensionId
                };              
                _projectTaskUserAssignedService.AssignUserToTask(projectTaskUserAssigned);
                _taskUserService.Create(taskUser);
                return Json(new { message = "Task added to Timesheet" });
            }
            else
            {
                return Json(new { errorMessage = errorMessage });
            }
        }

        [HttpPost]
        public IActionResult Unassign(int taskExtensionId)
        {
            var currentTaskUser = _taskUserService.GetAll().FirstOrDefault(tu => tu.TaskExtensionId == taskExtensionId);
            var currentTaskExtension = _taskExtensionService.GetById(taskExtensionId);
            currentTaskExtension.FormattedNameAssignedTo = "";
            currentTaskExtension.AlreadyAddedToTimesheet = false;
            currentTaskExtension.Status = "";
            currentTaskExtension.DueDate = null;
            currentTaskExtension.AssignedEmployeeId = null;
            _taskUserService.Remove(currentTaskUser);
            _projectTaskUserAssignedService.UnassignUsersToTask(taskExtensionId);
            return Json(new { message = "Task was successfully unassigned" });
        }

        [HttpPost]
        public IActionResult UpdateProjectNumber(int id, string projNumber)
        {
            if (String.IsNullOrEmpty(projNumber))
            {
                return Json(new { error = "You need to enter a valid Project Number." });
            }
            var allProjectNumbers = _proposalService.GetAll().Where(p => !String.IsNullOrEmpty(p.ProjectNumber)).Select(p => p.ProjectNumber);
            foreach (var pn in allProjectNumbers)
            {
                if (String.Equals(pn, projNumber))
                {
                    return Json(new { error = "The Project Number you entered has been assigned already." });
                }
            }
            var proposal = _proposalService.GetProposalById(id);
            proposal.ProjectNumber = projNumber;
            _proposalService.UpdateProposal(proposal);
            return Json(new { message = "Project Number have been updated." });
        }

        [HttpPost]
        public IActionResult UpdateProjectStatus(int projectId, string projectStatus)
        {
            var project = _proposalService.GetProposalById(projectId);
            project.ProjectStatus = projectStatus;
            _proposalService.UpdateProposal(project);
            return Json(new { message = "Project Status has been updated" });
        }

        [HttpPost]
        public IActionResult UpdateContractStatus(int projectId, string contractStatus)
        {
            var project = _proposalService.GetProposalById(projectId);
            project.ContractStatus = contractStatus;
            _proposalService.UpdateProposal(project);
            TempData["ContractStatusUpdated"] = "Contract Status has been updated";
            return Json(new { message = "success" });
        }

        [HttpPost]
        public IActionResult UpdateContractStatusUnderSameProjectNumber(int projectId, string contractStatus)
        {
            var project = _proposalService.GetProposalById(projectId);
            var projectsWithSameContractNumber = _proposalService.GetAll().Where(p => p.ProjectNumber == project.ProjectNumber);
            foreach (var projectToUpdate in projectsWithSameContractNumber)
            {
                projectToUpdate.ContractStatus = contractStatus;
                _proposalService.UpdateProposal(project);
            }
            TempData["ContractStatusUpdated"] = "Contract Statuses under this Project Number has been updated";
            return Json(new { message = "success" });
        }

        [HttpPost]
        public JsonResult AutoCompleteTaskUserAssigned(string prefix)
        {
            var userByPrefix = _userService.GetAllApplicationUsers().Where(u => u.IsActive).Where(c => String.Format(c.FirstName + " " + c.LastName)
            .Contains(prefix, StringComparison.InvariantCultureIgnoreCase))
                .Select(obj => new
                {
                    label = $"{obj.FirstName} {obj.LastName}",
                    val = obj.Id
                });
            return Json(userByPrefix);
        }
    }
}
