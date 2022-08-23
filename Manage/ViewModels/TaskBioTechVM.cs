using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.ViewModels
{
    public partial class TaskBioTechVM
    {
        public int Id { get; set; }
        [Required]
        public int TaskCodeParent { get; set; }
        [Required]
        public int TaskCodeSub { get; set; }
        [Required]
        public string TaskTitle { get; set; }        
        public string TaskDescription { get; set; }
        [Required]
        public string Category { get; set; }
        public IEnumerable<string> CategoriesList { get; set; }
        public string FormattedTaskNumber { get; set; }      
    }
}
