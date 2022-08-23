using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.ViewModels.Reports
{
    public class CustomerBalanceSummary
    {
        public string ProjectTitle { get; set; }
        public List<EntryDetail> EntryDetails { get; set; }
        public decimal Total { get; set; }
    }

    public class EntryDetail
    {
        public string ContractNumber { get; set; }
        public decimal SummaryAmount { get; set; }
    }
}
