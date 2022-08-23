using BioTech.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public class InvoiceTimesheetEntryService : IInvoiceTimesheetEntryService
    {
        BioTechContext _db;
        public InvoiceTimesheetEntryService(BioTechContext db)
        {
            _db = db;
        }
        public IEnumerable<InvoiceTimesheetEntry> GetAll()
        {
            return _db.InvoiceTimesheetEntry;
        }

        public InvoiceTimesheetEntry GetById(int id)
        {
            return _db.InvoiceTimesheetEntry.FirstOrDefault( i => i.Id == id );
        }

        public void Update(InvoiceTimesheetEntry invoiceTimesheetEntry)
        {
            var local = _db.Set<InvoiceTimesheetEntry>().Local.FirstOrDefault(entry => entry.Id.Equals(invoiceTimesheetEntry.Id));
            if (local != null)
            {
                // detach
                _db.Entry(local).State = EntityState.Detached;
            }

            _db.Entry(invoiceTimesheetEntry).State = EntityState.Modified;

            _db.InvoiceTimesheetEntry.Update(invoiceTimesheetEntry);
            _db.SaveChanges();
        }
    }
}
