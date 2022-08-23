using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.ViewModels
{
    public class InvoiceCsv
    {
        [CsvHelper.Configuration.Attributes.Name("Project Number")]
        public string ProjectNumber { get; set; }
        [CsvHelper.Configuration.Attributes.Name("Contract Number")]
        public string ContractNumber { get; set; }
        [CsvHelper.Configuration.Attributes.Name("Invoice Number")]
        public int InvoiceNumber { get; set; }
        [CsvHelper.Configuration.Attributes.Name("Invoice Date")]
        public DateTime InvoiceDate { get; set; }
        [CsvHelper.Configuration.Attributes.Name("Current Charges")]
        public decimal CurrentCharges { get; set; }
        [CsvHelper.Configuration.Attributes.Name("Terms")]
        public string Terms { get; set; }
    }
}
