using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.ViewModels.Reports
{
    public class JobProgressVsEstimate
    {
        public string ProjectFormattedTitle { get; set; }
        public List<InvoicesRelated> InvoicesRelated { get; set; }
    }

    public class InvoicesRelated
    {
        public string ContractNumber { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string Num { get; set; }
        public string Rep { get; set; }
        public bool EstimateActive { get; set; }
        public decimal EstimateTotal { get; set; }
        public decimal ProgressInvoice { get; set; }
        public decimal PercentageProgress { get; set; }
        public string CompanyName { get; set; }
    }
}
