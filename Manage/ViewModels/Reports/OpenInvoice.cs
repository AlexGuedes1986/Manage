using BioTech.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.ViewModels.Reports
{
    public class OpenInvoice : BaseReport
    {        
        public string PONumber { get; set; } = "";
        public string Terms { get; set; } = "Net 30";       
        public string ContractNumber { get; set; }
        public int Aging { get; set; }
    }
}
