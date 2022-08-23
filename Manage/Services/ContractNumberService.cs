using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public class ContractNumberService
    {
        BioTechContext _db;
        public ContractNumberService(BioTechContext db)
        {
            _db = db;
        }
        public void Create(ContractNumbers contractNumber)
        {
            _db.ContractNumbers.Add(contractNumber);
            _db.SaveChanges();
        }
    }
}
