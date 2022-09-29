using BioTech.Models;
using Manage.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Services
{
    public class CallService : ICallService
    {
        BioTechContext _db;
        public CallService(BioTechContext db)
        {
            _db = db;
        }
        public void Create(Call call)
        {
            _db.Call.Add(call);
            _db.SaveChanges();
        }

        public IEnumerable<Call> GetAll()
        {
            return _db.Call.ToList();
        }

        public IEnumerable<Call> GetCallLogsByContactId(int contactId)
        {
            return _db.Call.Where(tl => tl.ContactClubId == contactId);
        }//public IEnumerable<Call> GetCallLogsByUserId(string userId)
       
    }
}
