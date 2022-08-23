using BioTech.Models;
using BioTech.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Helpers
{
    public static class TimesheetHelper
    {
        public static float CalculateLiveRemainingTimeForTask(TaskExtension currentTaskExtension, float hoursEntered, int hourlyRateSelected, float totalSpentOnTask)
        {
            float remainingBudgetOnTask = 0;
            float remainingTimeOnTask = 0;
            remainingBudgetOnTask = (float)currentTaskExtension.NotToExceedTotalPrice - totalSpentOnTask;
            remainingTimeOnTask = remainingBudgetOnTask / hourlyRateSelected;
            var remainingTimeHourlyNotToExceed = (float)Math.Round(remainingTimeOnTask - hoursEntered, 1);
            return remainingTimeHourlyNotToExceed;
        }
        public static decimal CalculateTotalAmountBilledThisWeek(int hourlyRate, IEnumerable<TimesheetEntry> taskUserViewModels)
        {
            decimal total = 0;    
            foreach (var taskUser in taskUserViewModels)
            {
                if (String.Equals(taskUser.TaskUser.TaskExtension.FeeStructure, "Per Event", StringComparison.InvariantCultureIgnoreCase)
                    || String.Equals(taskUser.TaskUser.TaskExtension.FeeStructure, "Recurring", StringComparison.InvariantCultureIgnoreCase))
                {
                    total += taskUser.TaskUser.TaskExtension.InstancePrice;
                    continue;
                }
                if (String.Equals(taskUser.TaskUser.TaskExtension.FeeStructure, "Flat Fee", StringComparison.InvariantCultureIgnoreCase) 
                    && taskUser.TaskUser.TaskExtension.TaskCompleted)
                {
                    total += taskUser.TaskUser.TaskExtension.TotalPrice;
                    continue;
                }               
                if (!String.Equals(taskUser.TaskUser.TaskExtension.FeeStructure, "Per Event", StringComparison.InvariantCultureIgnoreCase)
                    && !String.Equals(taskUser.TaskUser.TaskExtension.FeeStructure, "Recurring", StringComparison.InvariantCultureIgnoreCase)
                    && !String.Equals(taskUser.TaskUser.TaskExtension.FeeStructure, "Flat Fee", StringComparison.InvariantCultureIgnoreCase))
                {
                    total += (decimal)taskUser.HoursWorked * hourlyRate;
                }               
            }       
            return total;
        }
    }
}
