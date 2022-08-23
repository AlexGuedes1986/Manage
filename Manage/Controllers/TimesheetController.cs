using AutoMapper;
using BioTech.Helpers;
using BioTech.Models;
using BioTech.Services;
using BioTech.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Controllers
{
    public class TimesheetController : Controller
    {
        private readonly ITaskExtensionService _taskExtensionService;
        private readonly ITaskUserService _taskUserService;
        private readonly IProposalService _proposalService;
        private readonly ITimesheetEntryService _timesheetEntryService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly IBillingRoleService _billingRoleService;
        private readonly ITimesheetApprovedDateRangeService _timesheetApprovedDateRangeService;
        public TimesheetController(ITaskExtensionService taskExtensionService, ITaskUserService taskUserService, IProposalService proposalService
            , ITimesheetEntryService timesheetEntryService, IMapper mapper, UserManager<ApplicationUser> userManager, IUserService userService
           , IBillingRoleService billingRoleService, ITimesheetApprovedDateRangeService timesheetApprovedDateRangeService)
        {
            _taskExtensionService = taskExtensionService;
            _taskUserService = taskUserService;
            _proposalService = proposalService;
            _timesheetEntryService = timesheetEntryService;
            _mapper = mapper;
            _userManager = userManager;
            _userService = userService;
            _billingRoleService = billingRoleService;
            _timesheetApprovedDateRangeService = timesheetApprovedDateRangeService;
        }
        public IActionResult Index(DateTime? date, string userId)
        {
            if (date == null)
            {
                date = DateTime.Now;
            }
            List<TaskUserViewModel> taskUserViewModels = new List<TaskUserViewModel>();
            var effectiveDate = date ?? DateTime.Now;
            var startOfWeek = new DateTime();
            var endOfWeek = new DateTime();
            if (effectiveDate.DayOfWeek == DayOfWeek.Sunday)
            {
                startOfWeek = effectiveDate.AddDays(-6);
                endOfWeek = effectiveDate;
            }
            else
            {
                startOfWeek = effectiveDate.AddDays(-1 * (int)(effectiveDate.DayOfWeek) + 1);
                endOfWeek = effectiveDate.AddDays(-1 * (int)(effectiveDate.DayOfWeek) + 7);
            }

            var allUsersTS = _userService.GetAll().Where(u => u.IsActive);
            List<UserTimesheetVM> usersTimesheet = new List<UserTimesheetVM>();
            foreach (var userTS in allUsersTS)
            {
                bool currentWeekUserTSApproved = false;
                var approvedTimesheetsByUserTS = _timesheetApprovedDateRangeService.GetByUserId(userTS.Id);
                foreach (var timesheetApprovedTS in approvedTimesheetsByUserTS)
                {
                    if (timesheetApprovedTS.DateStart.Date <= date.Value.Date && timesheetApprovedTS.DateEnd.Date >= date.Value.Date)
                    {
                        currentWeekUserTSApproved = true;
                    }
                }
                usersTimesheet.Add(new UserTimesheetVM
                {
                    UserId = userTS.Id,
                    FirstName = userTS.FirstName,
                    LastName = userTS.LastName,
                    TimesheetsCurrentApproved = currentWeekUserTSApproved,
                    Selected = String.Equals(userTS.Id, userId)
                });
            }
            var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var currentUserId = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;
            TimesheetViewModel timesheetView = new TimesheetViewModel();
            var approvedTimesheetsByUser = _timesheetApprovedDateRangeService.GetByUserId(currentUserId);
            foreach (var timesheetApproved in approvedTimesheetsByUser)
            {
                if (timesheetApproved.DateStart.Date <= date.Value.Date && timesheetApproved.DateEnd.Date >= date.Value.Date)
                {
                    timesheetView.IsApproved = true;
                    timesheetView.TimeSheetApprovedRangeId = timesheetApproved.Id;
                }
            }
            var taskUsers = new List<TaskUser>();
            if (!String.IsNullOrEmpty(userId))
            {
                var userFiltered = _userService.GetById(userId);
                taskUsers = _taskUserService.GetAll().Where(tu => tu.EffectiveDate <= endOfWeek
                && String.Equals(tu.UserId, userId, StringComparison.CurrentCultureIgnoreCase) && tu.Active && 
                tu.RemoveFromTimesheetsDate == null || (tu.RemoveFromTimesheetsDate != null 
                && tu.RemoveFromTimesheetsDate < startOfWeek || tu.RemoveFromTimesheetsDate > endOfWeek)
                && tu.TaskExtension.Proposal.ProjectStatus == "Active").ToList();
                timesheetView.UserFilteredId = userId;
                timesheetView.UserFilteredFormattedName = $"{userFiltered.FirstName} {userFiltered.LastName}";
                timesheetView.BillingRole = _billingRoleService.GetBillingRoleById(userFiltered.BillingRoleId).Name;
            }
            else
            {
                taskUsers = _taskUserService.GetAll().Where(tu => tu.EffectiveDate <= endOfWeek
                && String.Equals(tu.UserId, currentUserId, StringComparison.CurrentCultureIgnoreCase) && tu.Active 
                && tu.RemoveFromTimesheetsDate == null || (tu.RemoveFromTimesheetsDate != null && 
                tu.RemoveFromTimesheetsDate < startOfWeek || tu.RemoveFromTimesheetsDate > endOfWeek)
                && tu.TaskExtension.Proposal.ProjectStatus == "Active").ToList();
                timesheetView.BillingRole = _billingRoleService.GetBillingRoleById(currentUser.BillingRoleId).Name;
                timesheetView.UserFilteredId = currentUserId;
            }
            var taskUserId = taskUsers.Select(tu => tu.UserId).FirstOrDefault();
            var taskUserHourlyRate = 0;
            if (taskUsers.Count > 0)
            {
                taskUserHourlyRate = _billingRoleService.GetBillingRoleById(_userService.GetById(taskUserId).BillingRoleId).HourlyRate;
            }
            foreach (var taskUActive in taskUsers)
            {
                var currentProposal = _proposalService.GetProposalById(_taskExtensionService.GetById(taskUActive.TaskExtensionId).ProposalId);
                var currentTaskExtension = _taskExtensionService.GetById(taskUActive.TaskExtensionId);
                float totalSpentOnTask = 0;
                float remainingBudgetOnTask = 0;
                float remainingTimeOnTask = 0;
                var currentUserHourlyRate = _billingRoleService.GetBillingRoleById(_userService.GetById(taskUActive.UserId).BillingRoleId).HourlyRate;
                if (String.Equals(currentTaskExtension.FeeStructure, "Hourly Not To Exceed"))
                {
                    var taskUsersRelatedToCurrentTaskExtension = _taskUserService.GetTaskUsersByTaskExtensionId(currentTaskExtension.Id).Select(tu => tu.Id);
                    totalSpentOnTask = _timesheetEntryService.TotalSpentByTask(taskUsersRelatedToCurrentTaskExtension.ToList());
                    remainingBudgetOnTask = (float)currentTaskExtension.NotToExceedTotalPrice - totalSpentOnTask;
                    remainingTimeOnTask = remainingBudgetOnTask / currentUserHourlyRate;
                }
                var approvedTimesheetsByTaskUActive = _timesheetApprovedDateRangeService.GetByUserId(taskUActive.UserId);
                var taskUActiveUser = _userService.GetById(taskUActive.UserId);                
                taskUserViewModels.Add(new TaskUserViewModel()
                {
                    TaskUserId = taskUActive.Id,
                    ProjectNumber = currentProposal.ProjectNumber,
                    ProjectName = currentProposal.ProjectName,
                    ContractNumber = currentProposal.ContractNumber,
                    TaskNumber = $"{currentTaskExtension.TaskCodeParent.ToString("00")}-{currentTaskExtension.TaskCodeSub.ToString("00")}",
                    Status = taskUActive.TaskExtension.Status,
                    TaskTitle = currentTaskExtension.TaskTitle,
                    Description = currentTaskExtension.TaskDescription,
                    Note = taskUActive.TaskExtension.Note,
                    TimesheetEntries = taskUActive.TimesheetEntries,
                    RequireComment = currentTaskExtension.RequireComment,
                    FeeStructure = currentTaskExtension.FeeStructure,
                    TaskCode = $"{currentTaskExtension.TaskCodeParent}-{currentTaskExtension.TaskCodeSub}--{currentTaskExtension.FeeStructure}",
                    //Truncating integer to lower 1 place decimal
                    RemainingTimeHourlyNotToExceed = (float)Math.Truncate(remainingTimeOnTask*10)/10,
                    IsHourlyRateNotExceed = String.Equals(currentTaskExtension.FeeStructure, "Hourly Not To Exceed"),
                    ProjectDate = currentProposal.DateCreated,
                    ProposalId = currentProposal.Id,
                });
            }
            var billableHoursEntered = taskUserViewModels.SelectMany(tum => tum.TimesheetEntries.Where(tse => tse.DateModified >= startOfWeek.Date
           && tse.DateModified <= endOfWeek.Date && String.Equals(tse.TimesheetEntryType, "billed"))).Sum(tse => tse.HoursWorked);          
            var taskUserViewModelsFiltered = taskUserViewModels.SelectMany(tum => tum.TimesheetEntries.Where(tse => tse.DateModified >= startOfWeek.Date
           && tse.DateModified <= endOfWeek.Date && String.Equals(tse.TimesheetEntryType, "billed")));
            timesheetView.EffectiveDate = effectiveDate;
            timesheetView.StartWeek = startOfWeek;
            timesheetView.EndWeek = endOfWeek;
            timesheetView.TaskUserModels = taskUserViewModels;
            timesheetView.UsersTimesheet = usersTimesheet;
            timesheetView.HoursEnteredThisWeek = timesheetView.TaskUserModels.SelectMany(tum => tum.TimesheetEntries.Where(tse => tse.DateModified >= startOfWeek.Date
            && tse.DateModified <= endOfWeek.Date)).Sum(tse => tse.HoursWorked);
            timesheetView.TotalAmountBilledThisWeek = TimesheetHelper.CalculateTotalAmountBilledThisWeek(taskUserHourlyRate, taskUserViewModelsFiltered);
            //Checking for User Roles to display Admin functions in view
            if (currentUser.BillingRoleId == 9 || currentUser.BillingRoleId == 4 || currentUser.BillingRoleId == 3 || currentUser.BillingRoleId == 2)
            {
                timesheetView.HasAdminPermission = true;
            }
            return View(timesheetView);
        }

        public IActionResult CalculateLiveRemainingTimeHourlyTaskAjax(int taskUserId, float hoursEntered, int hourlyRateSelected)
        {
            var currentTaskUser = _taskUserService.GetById(taskUserId);
            float totalSpentOnTask = 0;
            var currentTaskExtension = _taskExtensionService.GetById(currentTaskUser.TaskExtensionId);
            var taskUsersRelatedToCurrentTaskExtension = _taskUserService.GetTaskUsersByTaskExtensionId(currentTaskExtension.Id).Select(tu => tu.Id);
            totalSpentOnTask = _timesheetEntryService.TotalSpentByTask(taskUsersRelatedToCurrentTaskExtension.ToList());
            var remainingTimeHourlyNotToExceed = TimesheetHelper.CalculateLiveRemainingTimeForTask(currentTaskExtension, hoursEntered, hourlyRateSelected, totalSpentOnTask);
            return Json(new { remainingTime = remainingTimeHourlyNotToExceed });
        }

        [HttpPost]
        public JsonResult AddTimesheetEntry(TimesheetEntryViewModel timesheetEntryVM)
        {
            List<string> validationErrors = new List<string>();
            bool isValid = true;
            if (timesheetEntryVM.RequireComment)
            {
                if (String.IsNullOrEmpty(timesheetEntryVM.Comment))
                {
                    validationErrors.Add("- This task requires a comment.");
                    isValid = false;
                }
            }
            if (timesheetEntryVM.HoursWorked < 0)
            {
                validationErrors.Add("- Hours Worked can't be negative");
                isValid = false;
            }
            var currentTaskExtensionId = _taskUserService.GetById(timesheetEntryVM.TaskUserId).TaskExtensionId;
            var currentTaskExtension = _taskExtensionService.GetById(currentTaskExtensionId);

            if (isValid)
            {
                if (timesheetEntryVM.TaskCompleted)
                {

                    currentTaskExtension.Status = "Complete";
                    currentTaskExtension.TaskCompleted = true;
                    _taskExtensionService.Update(currentTaskExtension);
                }
                TimesheetEntry timesheetEntry = _mapper.Map<TimesheetEntry>(timesheetEntryVM);

                if (timesheetEntry.Id > 0)
                {
                    if (timesheetEntry.HoursWorked == 0)
                    {
                        _timesheetEntryService.Remove(timesheetEntry);
                    }
                    else
                    {
                        _timesheetEntryService.Update(timesheetEntry);
                    }
                }
                else
                {
                    _timesheetEntryService.Create(timesheetEntry);
                }

                return Json(new
                {
                    id = timesheetEntry.Id,
                    note = timesheetEntry.Comment,
                    isComplete = timesheetEntryVM.TaskCompleted,
                    entryType = timesheetEntryVM.TimesheetEntryType
                });
            }
            else
            {
                return Json(new { error = String.Join("\n", validationErrors) });
            }

        }

        public JsonResult ApproveTimesheet(TimesheetApprovedDateRange timesheetApprovedDateRange)
        {
            _timesheetApprovedDateRangeService.Create(timesheetApprovedDateRange);
            _timesheetEntryService.ApproveByDateRange(timesheetApprovedDateRange.DateStart, timesheetApprovedDateRange.DateEnd, timesheetApprovedDateRange.UserId);
            return Json(new { message = "success" });
        }

        public JsonResult UnapproveTimesheet(int approvedTimesheetId)
        {
            var timesheetApprovedDateRange = _timesheetApprovedDateRangeService.GetById(approvedTimesheetId);
            _timesheetApprovedDateRangeService.Remove(approvedTimesheetId);
            //Setting related Timesheet entries to Not Approved
            var timesheetEntriesToUpdate = _timesheetEntryService.GetAllByDateRange(timesheetApprovedDateRange.DateStart, timesheetApprovedDateRange.DateEnd)
                .Where(t => String.Equals(t.TaskUser.UserId, timesheetApprovedDateRange.UserId)).ToList();
            foreach (var timesheetEntry in timesheetEntriesToUpdate)
            {
                timesheetEntry.IsApproved = false;
                _timesheetEntryService.Update(timesheetEntry);
            }
            return Json(new { message = "success" });
        }

        [HttpPost]
        public JsonResult RemoveFromTimesheet(int taskUserId)
        {
            _taskUserService.HideFromTimesheet(taskUserId);
            return Json(new { message = "success" });
        }
    }
}
