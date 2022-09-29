using BioTech.Models;
using Manage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Services
{
    public class CountryService : ICountryService
    {
        BioTechContext _db;
        public CountryService(BioTechContext db)
        {
            _db = db;
        }
        public IEnumerable<Country> GetAll()
        {
            return _db.Country;
        }
    }
}
