using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.ViewModels
{
    public class InvoiceTimesheetEntryVM
    {
        public decimal Value { get; set; }
        public int InvoiceTimesheetEntryId { get; set; }
        public string Type { get; set; }
    }
}
