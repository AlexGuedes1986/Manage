using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.ViewModels
{
    public class TaskExtensionVM
    {
        public int Id { get; set; }
        public decimal InstancePrice { get; set; }
        public int NumberOfInstances { get; set; }
        public decimal TotalPrice { get; set; }
        public int TaskCodeSub { get; set; }
        public int ProposalId { get; set; }
        public string FeeStructure { get; set; }
        public string IntervalType { get; set; }
        public string Category { get; set; }
        public int TaskCodeParent { get; set; }
        public string TaskDescription { get; set; }
        public string TaskTitle { get; set; }
        public virtual Proposal Proposal { get; set; }
        public bool RequireComment { get; set; }
        public DateTime? DueDate { get; set; }
        public string Note { get; set; }
        public string PercentageCompleted { get; set; }
        public bool TaskCompleted { get; set; }
        public string Status { get; set; }
        public bool AlreadyAddedToTimesheet { get; set; }
        public string FormattedNameAssignedTo { get; set; }
        public string AssignedEmployeeId { get; set; }
        public decimal NotToExceedTotalPrice { get; set; }
        public IEnumerable<ProjectTaskUserAssigned> UsersAssignedToTask { get; set; }
        public string UsersAssignedFormattedName { get; set; }

    }
}
