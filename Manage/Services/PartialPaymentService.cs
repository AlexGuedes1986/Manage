using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public class PartialPaymentService : IPartialPaymentService
    {
        BioTechContext _db;
        public PartialPaymentService(BioTechContext db)
        {
            _db = db;
        }
        public void Destroy(PartialPayment partialPayment)
        {
            _db.PartialPayment.Remove(partialPayment);
            _db.SaveChanges();
        }
    }
}
