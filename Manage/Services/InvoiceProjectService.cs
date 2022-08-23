using BioTech.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public class InvoiceProjectService : IInvoiceProjectService
    {
        BioTechContext _db;
        public InvoiceProjectService(BioTechContext db)
        {
            _db = db;
        }

        public void Create(InvoiceProject invoiceProject)
        {
            _db.InvoiceProject.Add(invoiceProject);
            _db.SaveChanges();         
        }

        public IEnumerable<InvoiceProject> GetAll()
        {
            return _db.InvoiceProject.Include(i => i.InvoiceTimesheetEntries).Include(i => i.PartialPayments).Include(i => i.Contact).ThenInclude(i => i.Company);
        }

        public IEnumerable<InvoiceProject> GetByProjectNumber(string projectNumber)
        {
            return _db.InvoiceProject.Include(i => i.InvoiceTimesheetEntries).Where(i => i.ProjectNumber == projectNumber);
        }

        public InvoiceProject GetById(int id)
        {
            return _db.InvoiceProject.Include(i => i.PartialPayments).Include(i => i.InvoiceTimesheetEntries).Include(i => i.Contact)
                .ThenInclude(c => c.Company).FirstOrDefault(i => i.Id == id);
        }
        public void Update(InvoiceProject invoice)
        {
            _db.InvoiceProject.Update(invoice);
            _db.SaveChanges();
        }
        public int GetLastInvoiceNumber()
        {
            var invoiceProjects = _db.InvoiceProject.ToList();
            if (invoiceProjects.Count == 0)
            {
                return 0;
            }
            else
            {
                return invoiceProjects.Max(i => i.InvoiceNumber);
            }            
        }
        public void Destroy(InvoiceProject invoice)
        {
            foreach (var invoiceTimesheetEntry in invoice.InvoiceTimesheetEntries.ToList())
            {
                var localInvoiceTE = _db.Set<InvoiceTimesheetEntry>().Local.FirstOrDefault(entry => entry.Id.Equals(invoiceTimesheetEntry.Id));
                if (localInvoiceTE != null)
                {
                    _db.Entry(localInvoiceTE).State = EntityState.Deleted;

                    _db.SaveChanges();
                }
            }
            var local = _db.Set<InvoiceProject>().Local.FirstOrDefault(entry => entry.Id.Equals(invoice.Id));
            // check if local is not null 
            if (local != null)
            {
                // detach
                _db.Entry(local).State = EntityState.Deleted;
            }
         
            _db.SaveChanges();
        }
        public void RemovePartialPayment(PartialPayment partialPayment)
        {
            _db.PartialPayment.Remove(partialPayment);
            _db.SaveChanges();
        }
        public PartialPayment GetPartialPaymentById(int partialPaymentId)
        {
            return _db.PartialPayment.FirstOrDefault(p => p.Id == partialPaymentId);
        }
    }
}
