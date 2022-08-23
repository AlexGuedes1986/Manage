using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public interface IContactService
    {
        Contact GetById(int Id);

        void Create(Contact contact);

        void Destroy(Contact contact);

        IEnumerable<Contact> GetAll();

        bool Exists(string contactName);

        void Update(Contact contact);

        //There is an issue to display Contacts on the Kendo table if table Company is included, GetAll() method is using
        //.Include(c => c.Company). That's why an specific method was created just for the Kendo Grid
        IEnumerable<Contact> GetAllContactsForKendoGrid();
    }
}
