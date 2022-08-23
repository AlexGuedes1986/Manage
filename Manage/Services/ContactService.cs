//using BioTech.Data;
using BioTech.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public class ContactService : IContactService
    {
        BioTechContext _db;
        public ContactService(BioTechContext db)
        {
            _db = db;
        }

        public void Create(Contact contact)
        {
            _db.Contact.Add(contact);
            _db.SaveChanges();
        }

        public void Destroy(Contact contact)
        {
            _db.Contact.Remove(contact);
            _db.SaveChanges();
        }

        public Contact GetById(int Id)
        {
            return _db.Contact.Include(c => c.Company).FirstOrDefault(c => c.Id == Id);
        }

        public IEnumerable<Contact> GetAll()
        {
            return _db.Contact.Include(c => c.Company).ToList();
        }

        public bool Exists(string contactName)
        {
            return _db.Contact.FirstOrDefault(c => c.FirstName == contactName) != null;
        }

        public void Update(Contact contact)
        {
            _db.Contact.Update(contact);
            _db.SaveChanges();
        }

        //There is an issue to display Contacts on the Kendo table if table Company is included, GetAll() method is using
        //.Include(c => c.Company). That's why an specific method was created just for the Kendo Grid. Kendo Grid aparently 
        //works good with flat data, but not with array of arrays
        public IEnumerable<Contact> GetAllContactsForKendoGrid()
        {         
                return _db.Contact.ToList();
        }
    }
}
