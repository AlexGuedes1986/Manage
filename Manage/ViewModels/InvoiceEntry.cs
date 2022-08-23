using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.ViewModels
{
    public class InvoiceEntry
    {
        public string ProjectNumber { get; set; }
        public string ProjectName { get; set; }
        public string ContractNumber { get; set; }
        public string TaskNumber { get; set; }
        public string TaskName { get; set; }
        public string EmployeeFormattedName { get; set; }
        public int Hours { get; set; }
        public int Rate { get; set; }
    }
}
