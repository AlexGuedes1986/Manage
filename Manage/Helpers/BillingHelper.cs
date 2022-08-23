using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Helpers
{
    public static class BillingHelper
    {
        public static decimal GetContractAmt(string currentTaskFeeStructure, TimesheetEntry timesheetApprovedEntry)
        {
            if (String.Equals(currentTaskFeeStructure, "Hourly", StringComparison.InvariantCultureIgnoreCase))
            {
                return 0;
            }
            if (String.Equals(currentTaskFeeStructure, "Flat Fee", StringComparison.InvariantCultureIgnoreCase))
            {
               return timesheetApprovedEntry.TaskUser.TaskExtension.TotalPrice;
            }
            if (String.Equals(currentTaskFeeStructure, "Hourly Not To Exceed", StringComparison.InvariantCultureIgnoreCase))
            {
                return timesheetApprovedEntry.TaskUser.TaskExtension.NotToExceedTotalPrice;
            }
            if (String.Equals(currentTaskFeeStructure, "Recurring", StringComparison.InvariantCultureIgnoreCase))
            {
                return timesheetApprovedEntry.TaskUser.TaskExtension.InstancePrice * timesheetApprovedEntry.TaskUser.TaskExtension.NumberOfInstances;
            }
          
            return 0;
        }

        public static decimal GetRate(string currentTaskFeeStructure, TimesheetEntry timesheetApprovedEntry)
        {
            if (String.Equals(currentTaskFeeStructure, "Hourly", StringComparison.InvariantCultureIgnoreCase)
                || String.Equals(currentTaskFeeStructure, "Hourly Not To Exceed", StringComparison.InvariantCultureIgnoreCase))
            {
                return timesheetApprovedEntry.HourlyRate;
            }
            if (String.Equals(currentTaskFeeStructure, "Flat Fee", StringComparison.InvariantCultureIgnoreCase))
            {
                return timesheetApprovedEntry.TaskUser.TaskExtension.TotalPrice;
            }
            if (String.Equals(currentTaskFeeStructure, "Per Event", StringComparison.InvariantCultureIgnoreCase))
            {
                return timesheetApprovedEntry.TaskUser.TaskExtension.InstancePrice;
            }
            if (String.Equals(currentTaskFeeStructure, "Recurring", StringComparison.InvariantCultureIgnoreCase))
            {
                return timesheetApprovedEntry.TaskUser.TaskExtension.InstancePrice;
            }

            return 0;
        }

        public static decimal GetPercentage(string currentTaskFeeStructure, TimesheetEntry timesheetApprovedEntry)
        {
            if (String.Equals(currentTaskFeeStructure, "Hourly", StringComparison.InvariantCultureIgnoreCase))
            {
                return 0;
            }

            if (String.Equals(currentTaskFeeStructure, "Recurring", StringComparison.InvariantCultureIgnoreCase))
            {
                return timesheetApprovedEntry.TaskUser.TaskExtension.InstancePrice * 100/timesheetApprovedEntry.TaskUser.TaskExtension.TotalPrice;
            }
            return 100;
        }

        public static decimal GetPercentageHNTE(TimesheetEntry timesheetApprovedEntry, decimal HNTEAmountSpentCurrentInvoice)
        {
            var NTE = timesheetApprovedEntry.TaskUser.TaskExtension.NotToExceedTotalPrice;
            var totalPercentage = HNTEAmountSpentCurrentInvoice * 100 / NTE;
            return totalPercentage;
        }

        public static decimal GetAmount(string currentTaskFeeStructure, TimesheetEntry timesheetApprovedEntry)
        {
            if (String.Equals(currentTaskFeeStructure, "Hourly", StringComparison.InvariantCultureIgnoreCase)
                   || String.Equals(currentTaskFeeStructure, "Hourly Not To Exceed", StringComparison.InvariantCultureIgnoreCase))
            {
                return ((decimal)timesheetApprovedEntry.HoursWorked) * (timesheetApprovedEntry.HourlyRate);               
            }
            if (String.Equals(currentTaskFeeStructure, "Flat Fee", StringComparison.InvariantCultureIgnoreCase))
            {
                return timesheetApprovedEntry.TaskUser.TaskExtension.TotalPrice;
            }
            if (String.Equals(currentTaskFeeStructure, "Per Event", StringComparison.InvariantCultureIgnoreCase)
                || String.Equals(currentTaskFeeStructure, "Recurring", StringComparison.InvariantCultureIgnoreCase))
            {
                return timesheetApprovedEntry.TaskUser.TaskExtension.InstancePrice;
            }
            return 0;
        }

        public static int GetQty(string currentTaskFeeStructure, TimesheetEntry timesheetEntry)
        {
            if (String.Equals(currentTaskFeeStructure, "Flat Fee", StringComparison.InvariantCultureIgnoreCase)
                || String.Equals(currentTaskFeeStructure, "Per Event", StringComparison.InvariantCultureIgnoreCase)
                || String.Equals(currentTaskFeeStructure, "Recurring", StringComparison.InvariantCultureIgnoreCase))
            {
                return 1;
            }
            if (String.Equals(currentTaskFeeStructure, "Hourly", StringComparison.InvariantCultureIgnoreCase)
                || String.Equals(currentTaskFeeStructure, "Hourly Not To Exceed", StringComparison.InvariantCultureIgnoreCase))
            {
                //Need to make sure Qty will always be int.
                return Convert.ToInt32(timesheetEntry.HoursWorked);
            }
            return 0;
        }

        public static IEnumerable<InvoiceProject> FilterReport(IEnumerable<InvoiceProject> filteredReports, string filterType, string filterValue)
        {
            if (String.Equals(filterType, "client"))
            {
                filteredReports = filteredReports.Where(i => String.Equals(i.Contact.Company.Name, filterValue));
            }
            if (String.Equals(filterType, "employee"))
            {
                filteredReports = filteredReports.Where(i => String.Equals(i.ProjectManager, filterValue));
            }
            if (String.Equals(filterType, "projectNumber"))
            {
                filteredReports = filteredReports.Where(i => String.Equals(i.ProjectNumber, filterValue));
            }
            if (String.Equals(filterType, "contractNumber"))
            {
                filteredReports = filteredReports.Where(i => String.Equals(i.ContractNumber, filterValue));
            }
            return filteredReports;
        }
       
    }
}
