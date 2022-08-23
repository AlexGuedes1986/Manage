using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Models
{
    public class InvoiceProject
    {
        public InvoiceProject()
        {
            InvoiceTimesheetEntries = new List<InvoiceTimesheetEntry>();
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }       
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int RecordCount { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectTitle { get; set; }    
        public List<InvoiceTimesheetEntry> InvoiceTimesheetEntries { get; set; }
        public string Status { get; set; } = "Pending";
        public bool Emailed { get; set; }
        public bool Finalized { get; set; }
        public DateTime? LastPaymentReceivedDate { get; set; }
        public decimal TotalInvoiced { get; set; }
        public string Note { get; set; }     
        public Contact Contact { get; set; }
        public int InvoiceNumber { get; set; }
        public string ProjectManager { get; set; }
        public string ContractNumber { get; set; }
        public string InvoiceType { get; set; }
        public string Filter { get; set; }
        public decimal CurrentCharges { get; set; }
        public List<PartialPayment> PartialPayments { get; set; } = new List<PartialPayment>();
    }
}
