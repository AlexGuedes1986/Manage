using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Models
{
    public class League
    {
        public int Id { get; set; }
        public int LeagueID { get; set; }
        public string Name { get; set; }  
        public string CountryCode { get; set; }     
    }
}
