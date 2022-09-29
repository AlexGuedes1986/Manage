﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Models
{
    public class Country
    {
        [Key]
        public int Id { get; set; }   
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
