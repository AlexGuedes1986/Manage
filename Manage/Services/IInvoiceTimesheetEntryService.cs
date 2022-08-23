using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public interface IInvoiceTimesheetEntryService
    {
        IEnumerable<InvoiceTimesheetEntry> GetAll();
        InvoiceTimesheetEntry GetById(int id);
        void Update(InvoiceTimesheetEntry invoiceTimesheetEntry);
    }
}
