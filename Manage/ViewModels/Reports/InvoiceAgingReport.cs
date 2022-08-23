using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BioTech.ViewModels.Reports;

namespace BioTech.ViewModels.Reports
{
    public class InvoiceAgingReport : BaseReport
    {       
        public string Name { get; set; }            
        public string Rep { get; set; }
        public int Aging { get; set; }
        public string Sorting { get; set; }
    }
}
