using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public interface ITouchLogService
    {
        void Create(TouchLog touchlog);
        IEnumerable<TouchLog> GetAll();
        IEnumerable<TouchLog> GetTouchLogsByContactId(int contactId);
        IEnumerable<TouchLog> GetTouchLogsByUserId(string userId);
    }
}
