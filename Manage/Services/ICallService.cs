using Manage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Services
{
    public interface ICallService
    {
        void Create(Call call);
        IEnumerable<Call> GetAll();
        IEnumerable<Call> GetCallLogsByContactId(int contactId);
    }
}
