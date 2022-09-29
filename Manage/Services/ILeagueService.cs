using Manage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Services
{
    public interface ILeagueService
    {
        IEnumerable<League> GetLeaguesByCountryCode(string countryCode);
    }
}
