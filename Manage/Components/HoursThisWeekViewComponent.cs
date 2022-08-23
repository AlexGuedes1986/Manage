using BioTech.Models;
using BioTech.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Components
{
    public class HoursThisWeekViewComponent: ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITimesheetEntryService _timesheetEntryService;
        public HoursThisWeekViewComponent(UserManager<ApplicationUser> userManager, ITimesheetEntryService timesheetEntryService)
        {
            _userManager = userManager;
            _timesheetEntryService = timesheetEntryService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {            
                var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
                var startOfWeek = DateTime.Now.AddDays(-1 * (int)(DateTime.Now.DayOfWeek));
                var endOfWeek = DateTime.Now.AddDays(-1 * (int)(DateTime.Now.DayOfWeek) + 6);
                var timeEntriesThisWeek = _timesheetEntryService.GetAllByDateRange(startOfWeek, endOfWeek)
                    .Where(te => te.TaskUser.UserId == currentUser.Id);
                var hoursWorkedThisWeek = Convert.ToInt32(timeEntriesThisWeek.Sum(te => te.HoursWorked));
                var percentageHoursCompletedThisWeek = hoursWorkedThisWeek * 100 / 40;
                return View("~/Views/Home/HoursThisWeek.cshtml", percentageHoursCompletedThisWeek);      
        }
    }
}
