using Manage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Services
{
    public interface IManageDataService
    {
        void UpdateCountries(List<Country> countries);
        void UpdateLeagues(List<League> leagues);
    }
}
