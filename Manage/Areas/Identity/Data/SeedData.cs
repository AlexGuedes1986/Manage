//using BioTech.Helpers;
//using BioTech.Models;
//using Microsoft.AspNetCore.Identity;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using BioTech.Constants;
//using BioTech.Areas.Identity.Data;

//namespace BioTech.Data
//{
//    //The purpose of this class is just to add new Identity roles.
//    //For some reason Authentication is not working.
//    public class SeedData
//    {
//        public static async Task Initialize(
//        BioTechIdentityContext context,
//        UserManager<ApplicationUser> userManager,
//        RoleManager<ApplicationRole> roleManager
//    )
//        {
//            context.Database.EnsureCreated();

//            // create the admin role in the database
//            if (await roleManager.FindByNameAsync(RolesHelper.AdminRole) == null)
//                await roleManager.CreateAsync(new ApplicationRole(RolesHelper.AdminRole, "Administers the website users", DateTime.Now));
//            // create the billing role in the database
//            if (await roleManager.FindByNameAsync(RolesHelper.BillingRole) == null)
//                await roleManager.CreateAsync(new ApplicationRole(RolesHelper.BillingRole, "Clients using the website", DateTime.Now));
//            // create the Project Manager role in the database
//            if (await roleManager.FindByNameAsync(RolesHelper.ProjectManagerRole) == null)
//                await roleManager.CreateAsync(new ApplicationRole(RolesHelper.ProjectManagerRole, "Project Managers using the website", DateTime.Now));
//            // create the Field Operator role in the database
//            if (await roleManager.FindByNameAsync(RolesHelper.FieldOperatorRole) == null)
//                await roleManager.CreateAsync(new ApplicationRole(RolesHelper.FieldOperatorRole, "Field Operators using the website", DateTime.Now));
//            // create the Executive Team role in the database
//            if (await roleManager.FindByNameAsync(RolesHelper.ExecutiveTeamRole) == null)
//                await roleManager.CreateAsync(new ApplicationRole(RolesHelper.ExecutiveTeamRole, "Executive Team using the website", DateTime.Now));
                        
//            // create the default admin user
//            if (await userManager.FindByNameAsync("aguedes@bayshoresolutions.com") == null)
//            {
//                // create the model
//                var user = new ApplicationUser
//                {
//                    UserName = "aguedes",
//                    Email = "aguedes@bayshoresolutions.com",
//                    FirstName = "Alexander",
//                    LastName = "Guedes",
//                    PhoneNumber = "813.902.0141 x 262",
//                    IsActive = true
//                };

//                // add the default user
//                var result = await userManager.CreateAsync(user, "Password10");
//                if (result.Succeeded)
//                {
//                    // await userManager.AddPasswordAsync(user, "Password10");
//                    await userManager.AddToRoleAsync(user, RolesHelper.AdminRole);
//                }
//            }

//        }
//    }
//}
