using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.ViewModels.Reports
{
    public class BaseReport
    {
        public string Type { get; set; } = "Invoice";
        public DateTime Date { get; set; }
        public int Num { get; set; }       
        public DateTime DueDate { get; set; }       
        public decimal Balance { get; set; }
        public string FilterType { get; set; }
        public string FilterValue { get; set; }
        public string CompanyName { get; set; }
    }
}
