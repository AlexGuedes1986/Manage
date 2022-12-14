using Manage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Services
{
    public interface IContactClubService
    {
        IEnumerable<ContactClub> GetAll();
        ContactClub GetById(int contactClubId);
        void Create(ContactClub contactClub);
        void Update(ContactClub contact);
    }
}
