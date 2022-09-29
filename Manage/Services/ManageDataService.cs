using BioTech.Models;
using Manage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Services
{
    public class ManageDataService : IManageDataService
    {
        BioTechContext _db;
        public ManageDataService(BioTechContext db)
        {
            _db = db;   
        }
        public void UpdateCountries(List<Country> countries)
        {
            var existentCountries = _db.Country;
            foreach (var existentCountry in existentCountries)
            {
                _db.Country.Remove(existentCountry);
            }     
            foreach (var country in countries)
            {
            
                _db.Country.Add(country);
            }
            _db.SaveChanges();            
        }

        public void UpdateLeagues(List<League> leagues)
        {
            try
            {
                var existentLeagues = _db.League;
                foreach (var existentLeague in existentLeagues)
                {
                    _db.League.Remove(existentLeague);
                }
                foreach (var league in leagues)
                {
                    league.LeagueID = league.Id;
                    league.Id = 0;
                    _db.League.Add(league);
                }
                _db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }
    }
}
