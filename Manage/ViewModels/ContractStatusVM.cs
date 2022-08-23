using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.ViewModels
{
    public class ContractStatusVM
    {
        public int Id { get; set; }
        public string ProposalName { get; set; }
        public string ClientCompanyName { get; set; }
        public DateTime DateCreated { get; set; }         
        public string ProjectManagers { get; set; }
        public string Author { get; set; }
    }
}
