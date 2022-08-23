using System;
using System.Collections.Generic;

namespace BioTech.Models
{
    public partial class ProposalApplicationUser
    {
        public int ProposalId { get; set; }
        public string ApplicationUserId { get; set; }
        public int Id { get; set; }
        public string FormattedName { get; set; }
        public string Type { get; set; }
    }
}
