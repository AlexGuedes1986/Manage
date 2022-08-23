using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Constants
{
    public static class States
    {
        public static List<string> StatesList { get; set; } = new List<string>() { "Alabama","Alaska","Arizona","Arkansas","California"
        ,"Colorado","Connecticut","Delaware","Florida","Georgia","Hawaii","Idaho","Illinois","Indiana","Iowa","Kansas","Kentucky"
        ,"Louisiana","Maine","Maryland","Massachusetts","Michigan","Minnesota","Mississippi","Missouri","Montana","Nebraska"
        ,"Nevada","New Hampshire","New Jersey","New Mexico","New York","North Carolina","North Dakota","Ohio","Oklahoma","Oregon"
        ,"Pennsylvania","Rhode Island","South Carolina","South Dakota","Tennessee","Texas","Utah","Vermont","Virginia","Washington"
        ,"West Virginia","Wisconsin","Wyoming"};

        public static List<SelectListItem> PopulateAllStates()
        {
             List<SelectListItem> allStates = new List<SelectListItem>();
            foreach (var state in StatesList)
            {
                allStates.Add(new SelectListItem() { Text = state, Value = state });
            }
            return allStates;
        }
    }
}
