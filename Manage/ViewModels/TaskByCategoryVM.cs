using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.ViewModels
{
    public class TaskByCategoryVM
    {
        public int ProposalId { get; set; }
        public string ProjectName { get; set; }
        public Proposal Proposal { get; set; }
    }
}
