using System;
using System.Collections.Generic;

namespace BioTech.Models
{
    public partial class Company
    {
        public Company()
        {
            Contact = new HashSet<Contact>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string CompanyFax { get; set; }

        public virtual ICollection<Contact> Contact { get; set; }
    }
}
