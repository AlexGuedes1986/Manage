using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.ViewModels
{
    public class TaskBioTechViewModel
    {
        public int Id { get; set; }
        public int TaskCodeParent { get; set; }
        public int TaskCodeSub { get; set; }
        public string TaskTitle { get; set; }
        public string TaskDescription { get; set; }
        public string Category { get; set; }
        public bool IsAddedToProject { get; set; }
        public int TaskIdAssignedToProposal { get; set; }
    }
}
