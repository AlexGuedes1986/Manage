using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.ViewModels
{
    public class InvoiceProjectVM
    {
        public int Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int RecordCount { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectTitle { get; set; }
        public string Status { get; set; } = "Pending";
        public bool Emailed { get; set; }
        public bool Finalized { get; set; }
        public DateTime? LastPaymentReceivedDate { get; set; }
        public decimal TotalInvoiced { get; set; }
        public string Note { get; set; }    
        public int InvoiceNumber { get; set; }
        public string ProjectManager { get; set; }
        public string ContractNumber { get; set; }
        public string InvoiceType { get; set; }
        public string Filter { get; set; }
        public decimal PartialPaymentsTotal { get; set; }
        public decimal RemainingAmountToBePaid { get; set; }
        public decimal CurrentCharges { get; set; }
    }
}
