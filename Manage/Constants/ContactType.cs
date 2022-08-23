using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Constants
{
    public static class ContactType
    {
        public static List<SelectListItem> ContactTypes { get; set; } = new List<SelectListItem> { new SelectListItem() { Text = "Prospect"
            , Value = "Prospect" }, new SelectListItem() { Text = "Client", Value="Client"}, new SelectListItem{ Text = "Employee"
            , Value = "Employee" }, new SelectListItem {Text = "Media", Value = "Media" }, new SelectListItem {Text="Associate"
            , Value="Associate" }, new SelectListItem { Text="Partner", Value="Partner"}, new SelectListItem { Text="Sub-consultant"
            , Value="Sub-consultant"} };
    }
}
