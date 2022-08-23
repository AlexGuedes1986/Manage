using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Models
{
    public class ProjectTaskUserAssigned
    {
        public int Id { get; set; }
        public int ProposalId { get; set; }
        public int TaskExtensionId { get; set; }
        public string ApplicationUserId { get; set; }
        public string FormattedName { get; set; } = "";
    }
}
