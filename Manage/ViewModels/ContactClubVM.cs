using Manage.Models;
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
        public IEnumerable<Country> AvailableCountries { get; set; } = new List<Country>();
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public List<Team> AvailableTeams { get; set; } = new List<Team>();
        public string Team { get; set; }
        public IEnumerable<League> AvailableLeagues { get; set; } = new List<League>();     
        public string League { get; set; }
        public int LeagueId { get; set; }
        public string Position { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public bool IsAgent { get; set; }
        public string Stadium { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
