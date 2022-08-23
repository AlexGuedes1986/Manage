using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Models
{
    public class TaskEditorViewModel
    {
        public int Id { get; set; }
        public int TaskExtensionId { get; set; }
        public string TaskNumber { get; set; }
        public string TaskName { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public bool RequireComment { get; set; }    
        public string PercentageCompleted { get; set; }
        public bool AssignToMyself { get; set; }
        public DateTime EffectiveDate { get; set; }
        public List<ProjectTaskUserAssigned> TaskUsersAssigned { get; set; } = new List<ProjectTaskUserAssigned>();
        public int ProposalId { get; set; }
        public bool DisplayInTimesheet { get; set; }
    }
}
