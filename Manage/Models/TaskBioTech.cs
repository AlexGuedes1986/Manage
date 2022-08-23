using System;
using System.Collections.Generic;

namespace BioTech.Models
{
    public partial class TaskBioTech
    {
        public int Id { get; set; }
        public int TaskCodeParent { get; set; }
        public int TaskCodeSub { get; set; }
        public string TaskTitle { get; set; }
        public string TaskDescription { get; set; }
        public string Category { get; set; }     
    }
}
