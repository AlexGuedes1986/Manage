using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Models
{
    public class ContactClub
    {
        public int Id { get; set; }
        public string Name { get; set; }     
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Team { get; set; }
        public string League { get; set; }
        public int LeagueId { get; set; }
        public string Position { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Comments { get; set; }      
        public string Status { get; set; }
        public bool Confirmed { get; set; }
        public string Stadium { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public IEnumerable<Call> Calls { get; set; }
    }

    public class Call
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
        public int ContactClubId { get; set; }
        public ContactClub Contact { get; set; }
    }
}
