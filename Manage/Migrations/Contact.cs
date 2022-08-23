using System;
using System.Collections.Generic;

namespace BioTech.Models
{
    public partial class Contact
    {
        public Contact()
        {
            Proposal = new HashSet<Proposal>();
            TouchLog = new HashSet<TouchLog>();
        }

        public int Id { get; set; }
        public int CompanyId { get; set; }   
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string MobilePhone { get; set; }
        public string BusinessPhone { get; set; }       
        public string EmailAddress { get; set; }
        public string EmailAddress2 { get; set; }
        public string JobTitle { get; set; }
        public string ContactType { get; set; }
    
        public string Suffix { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<Proposal> Proposal { get; set; }
        public virtual ICollection<TouchLog> TouchLog { get; set; }
        public bool Active { get; set; } = true;
    }
}
