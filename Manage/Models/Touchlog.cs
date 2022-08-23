using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BioTech.Models
{
    public partial class TouchLog
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Employee { get; set; }
        [Required]
        public string Log { get; set; }
        public int ContactId { get; set; }
        [Required]
        public string Type { get; set; }
        public virtual Contact Contact { get; set; }
        public string UserId { get; set; }
    }
}
