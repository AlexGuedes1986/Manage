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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string Team { get; set; }
        public string League { get; set; }
        public string Position { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Comments { get; set; }      
        public string Status { get; set; }
        public IEnumerable<Call> Calls { get; set; }
    }

    public class Call
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
    }
}
