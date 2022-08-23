using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using BioTech.Data;
using BioTech.Models;
using Microsoft.EntityFrameworkCore;

namespace BioTech.Services
{
    public class TouchLogService : ITouchLogService
    {
        BioTechContext _db;
        public TouchLogService(BioTechContext db)
        {
            _db = db;
        }
        public void Create(TouchLog touchlog)
        {
            _db.TouchLog.Add(touchlog);
            _db.SaveChanges();
        }

        public IEnumerable<TouchLog> GetAll()
        {
            return _db.TouchLog.ToList();
        }

        public IEnumerable<TouchLog> GetTouchLogsByContactId(int contactId)
        {
            return _db.TouchLog.Where(tl => tl.ContactId == contactId);
        }
        public IEnumerable<TouchLog> GetTouchLogsByUserId(string userId)
        {
            return _db.TouchLog.Where(tl => tl.UserId == userId).Include(t => t.Contact).ThenInclude(c => c.Company);
        }
    }
}
