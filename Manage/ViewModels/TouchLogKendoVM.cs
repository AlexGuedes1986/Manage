using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.ViewModels
{
    public class TouchLogKendoVM
    {
        public DateTime Date { get; set; }
        public string Employee { get; set; }
        [Required]
        public string Log { get; set; }
        public int ContactId { get; set; }
        [Required]
        public string Type { get; set; }
        public string ContactName { get; set; }
        public string ContactCompanyName { get; set; }
    }
}
