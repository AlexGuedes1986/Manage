using System;
using System.Collections.Generic;

namespace BioTech.Models
{
    public partial class BillingRole
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int HourlyRate { get; set; }
    }
}
