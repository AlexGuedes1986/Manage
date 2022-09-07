using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.ViewModels
{
    public class ContactClubVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CountryVM> AvailableCountries { get; set; } = new List<CountryVM>();
        public string Country { get; set; }
        public List<LeagueVM> AvailableTeams { get; set; } = new List<LeagueVM>();
        public string Team { get; set; }
        public List<LeagueVM> AvailableLeagues { get; set; } = new List<LeagueVM>();     
        public string League { get; set; }
        public string Position { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }       
    }
}
