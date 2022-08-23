using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Models
{
    public class ProposalTeamMember
    {
        public int Id { get; set; }
        public int ProposalId { get; set; }
        public string  TeamMemberId { get; set; }
        public string FormattedName { get; set; }
    }
}
