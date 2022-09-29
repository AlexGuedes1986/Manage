using BioTech.Models;
using Manage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Services
{
    public class LeagueService : ILeagueService
    {
        BioTechContext _db;
        public LeagueService(BioTechContext db)
        {
            _db = db;   
        }
        public IEnumerable<League> GetLeaguesByCountryCode(string countryCode)
        {
            return _db.League.Where(l => l.CountryCode == countryCode);
        }
    }
}
