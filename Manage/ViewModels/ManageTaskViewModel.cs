using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.ViewModels
{
    public class ManageTaskViewModel
    {
        public int Id { get; set; }
        public int ProjectDefaultId { get; set; }
        public int TaskDefaultId { get; set; }
        public List<Proposal> Projects { get; set; }
        public bool AssignToMySelf { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public DateTime? DueDate { get; set; }
        public string Note { get; set; }
        public bool RequireComment { get; set; }
        public string TypeOfView { get; set; } = "Assign";
        public string AssignedFormattedName { get; set; }
        public string ProjectNumber { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public List<ProjectTaskUserAssigned> UsersAssignedToTask { get; set; } = new List<ProjectTaskUserAssigned>();
        public bool DisplayInTimesheet { get; set; }
    }
}
