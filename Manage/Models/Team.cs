using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Models
{
    public class Team
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Code { get; set; }
        public int Founded { get; set; }    
    }
}
