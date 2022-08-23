using BioTech.Constants;
using BioTech.Models;
using BioTech.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BioTech.Helpers
{
    public static class Helper
    {
        public static List<SelectListItem> CompaniesToSelectListItems(IEnumerable<Company> companies)
        {
            var companiesToListItemsList = new List<SelectListItem>();
            foreach (var company in companies)
            {
                companiesToListItemsList.Add(new SelectListItem() { Text = company.Name, Value = company.Id.ToString() });
            }
            return companiesToListItemsList;
        }

        public static void PopulateDropDownsWhenErrorAddingContact(ContactViewModel model, IEnumerable<Company> allCompanies)
        {
            model.AvailableContactTypes.Add(new SelectListItem() { Text = "Select Contact Type", Value = "" });
            model.AvailableStates.Add(new SelectListItem() { Text = "Select State", Value = "" });
            model.AvailableCompanies.Add(new SelectListItem() { Text = "Select Company", Value = "" });
            model.AvailableContactTypes.AddRange(ContactType.ContactTypes);
            model.AvailableStates.AddRange(States.PopulateAllStates());
            model.AvailableCompanies.AddRange(Helper.CompaniesToSelectListItems(allCompanies));
        }

        public static string FormatInitials(string name)
        {
            Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
            string init = initials.Replace(name, "$1");
            return init;
        }

    }
}
