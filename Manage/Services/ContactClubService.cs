using BioTech.Models;
using Manage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Services
{
    public class ContactClubService : IContactClubService
    {
        BioTechContext _db;
        public ContactClubService(BioTechContext db)
        {
            _db = db;
        }
        public IEnumerable<ContactClub> GetAll()
        {
            return _db.ContactClub;          
        }
        public ContactClub GetById(int contactClubId)
        { 
            return _db.ContactClub.FirstOrDefault(c => c.Id == contactClubId);
        }
        public void Create(ContactClub contactClub)
        {
            _db.ContactClub.Add(contactClub);
            _db.SaveChanges();
        }
        public void Update(ContactClub contactClub)
        {
            _db.ContactClub.Update(contactClub);
            _db.SaveChanges();
        }

    }
}
