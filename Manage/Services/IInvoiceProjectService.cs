using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public interface IInvoiceProjectService
    {
        void Create(InvoiceProject invoiceProject);
        IEnumerable<InvoiceProject> GetAll();
        IEnumerable<InvoiceProject> GetByProjectNumber(string projectNumber);
        InvoiceProject GetById(int id);
        void Update(InvoiceProject invoice);
        int GetLastInvoiceNumber();
        void Destroy(InvoiceProject invoice);
        void RemovePartialPayment(PartialPayment partialPayment);
        PartialPayment GetPartialPaymentById(int partialPaymentId);
    }
}
