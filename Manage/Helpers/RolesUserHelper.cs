using BioTech.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Helpers
{
    public static class RolesUserHelper
    {
        //setting checkboxes as selected for the roles current user is using
        public static void SetAndReturnSelectedUserWebsiteRoles(UserRegistrationViewModel model)
        {
            var currentUserRolesSeparated = model.Role.Split(',');
            foreach (var currentUserRole in currentUserRolesSeparated)
            {
                foreach (var websiteRole in model.WebsiteRoles)
                {
                    if (currentUserRole == websiteRole.Name)
                    {
                        websiteRole.IsSelected = true;
                    }
                }
            }            
        }

        public static void SetAndReturnSelectedUserBillingRoles(UserRegistrationViewModel model)
        {
            for (int i = 0; i < model.BillingRoles.Count(); i++)
            {
                if (model.BillingRoles[i].Value == model.BillingRoleId.ToString())
                {
                    model.BillingRoles[i].Selected = true;
                }
            }
        }

        public static bool CanManage(IList<string> websiteRoles)
        {
            foreach (var websiteRole in websiteRoles)
            {
                if (String.Equals(websiteRole, "Billing") || String.Equals(websiteRole, "Project Manager") || String.Equals(websiteRole, "Admin")
                    || String.Equals(websiteRole, "Executive Team"))
                {
                    return true;                   
                }
            }
            return false;
        }

        public static bool CanActive(IList<string> websiteRoles)
        {
            foreach (var websiteRole in websiteRoles)
            {
                if (String.Equals(websiteRole, "Billing"))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
