using BioTech.Helpers;
using BioTech.Models;
using BioTech.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public class UserService : IDisposable, IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly BioTechIdentityContext _db;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager
            , BioTechIdentityContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }
        public IEnumerable<UserRegistrationViewModel> GetAll()
        {
            return TransformToViewModel(_userManager.Users);
        }

        public IEnumerable<ApplicationUser> GetAllApplicationUsers()
        {
            return _userManager.Users;
        }

        public void Create(UserRegistrationViewModel model)
        {
            ApplicationUser user = new ApplicationUser();
            user.Email = model.Email;
            user.UserName = model.UserName;
            user.EmailConfirmed = true;
            user.NormalizedEmail = model.Email.ToUpper();
            user.NormalizedUserName = model.Email.ToUpper();
            user.FirstName = model.FirstName;
            user.IsActive = model.IsActive;
            user.LastName = model.LastName;
            user.PhoneNumber = model.Phone;

            var result = _userManager.CreateAsync(user).Result;
            if (result.Succeeded)
            {
                _userManager.AddPasswordAsync(user, model.Password);
                _userManager.AddToRoleAsync(user, model.Role);
            }
        }

        public UserRegistrationViewModel GetById(string Id)
        {
            ApplicationUser user = _userManager.FindByIdAsync(Id).Result;
            return GetViewModel(user);
        }

        public UserRegistrationViewModel GetByName(string name)
        {
            ApplicationUser user = _userManager.FindByEmailAsync(name).Result;
            return GetViewModel(user);
        }

        public bool CheckEmail(string email)
        {
            ApplicationUser user = _userManager.FindByEmailAsync(email).Result;
            if (user != null)
            {
                return true;
            }
            return false;
        }

        public bool CheckUsername(string username)
        {
            ApplicationUser user = _userManager.FindByNameAsync(username).Result;
            if (user != null)
            {
                return true;
            }
            return false;
        }

        public string GetEmaiBylID(string ID)
        {
            ApplicationUser user = _userManager.FindByIdAsync(ID).Result;
            string email = user.Email;
            return email;
        }

        public void Update(UserRegistrationViewModel model)
        {
            // get a user object from the model
            ApplicationUser user = TransformFromViewModel(model);

            // update the user data
            _userManager.UpdateAsync(user).Wait();

            // remove from all roles (ignore the empty string role value from the default role selection)
            string[] allRoles = _roleManager.Roles.Select(r => r.Name).ToArray();
            _userManager.RemoveFromRolesAsync(user, allRoles).Wait();

            // add to the selected role
            _userManager.AddToRoleAsync(user, model.Role).Wait();
        }

        public void UpdatebyAdmin(UserRegistrationViewModel model)
        {
            // get a user object from the model
            ApplicationUser user = TransformFromViewModel(model);

            // update the user data
            _userManager.UpdateAsync(user).Wait();
            List<string> userRoles = new List<string>();

            // remove from all roles (ignore the empty string role value from the default role selection)
            List<string> allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            foreach (var role in allRoles)
            {
                _userManager.RemoveFromRoleAsync(user, role).Wait();
            }

            ////BSS Updating role one by one when multiple roles are selected
            foreach (var websiteRole in model.WebsiteRoles)
            {
                if (websiteRole.IsSelected == true)
                {
                    // add to the selected role
                    _userManager.AddToRoleAsync(user, websiteRole.Name).Wait();
                }
            }
        }

        public List<SelectListItem> GetRoleSelectListItems()
        {
            List<SelectListItem> roleList = new List<SelectListItem>();
            roleList.Add(new SelectListItem() { Text = "", Value = "" });
            roleList.AddRange(_roleManager.Roles.Select(i => new SelectListItem() { Text = i.Name, Value = i.Name }));
            return roleList;
        }

        public List<RoleUser> GetWebsiteRoleUsers(string userId = null)
        {
            List<RoleUser> roleList = new List<RoleUser>();
            if (!String.IsNullOrEmpty(userId))
            {
                roleList.AddRange(_roleManager.Roles.Where(r => r.Id == userId).Select(i => new RoleUser()
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description
                }));
            }
            else
            {
                roleList.AddRange(_roleManager.Roles.Select(i => new RoleUser()
                {
                    Id = i.Id
                    ,
                    Name = i.Name,
                    Description = i.Description
                }));
            }

            return roleList;
        }

        public List<ApplicationUser> GetPmsByIds(List<string> pmIds)
        {
            List<ApplicationUser> applicationUsers = new List<ApplicationUser>();
            foreach (var id in pmIds)
            {
                var user = _userManager.Users.Where(u => u.IsActive).FirstOrDefault(u => u.Id == id);
                if (user != null)
                {
                    applicationUsers.Add(user);
                }
               
            }
            return applicationUsers;
        }

        private List<UserRegistrationViewModel> TransformToViewModel(IEnumerable<ApplicationUser> users)
        {
            List<UserRegistrationViewModel> model = new List<UserRegistrationViewModel>();

            foreach (var user in users)
            {

                UserRegistrationViewModel vm = GetViewModel(user);
                model.Add(vm);
            }

            return model;
        }

        private UserRegistrationViewModel GetViewModel(ApplicationUser user)
        {
            // get the user's role
            List<string> userRoles = _userManager.GetRolesAsync(user).Result.OrderBy(x => x).ToList();

            UserRegistrationViewModel vm = new UserRegistrationViewModel()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                IsActive = user.IsActive,
                LastName = user.LastName,
                Phone = user.PhoneNumber,
                UserName = user.UserName,
                Id = user.Id,
                BillingRoleId = user.BillingRoleId,
                WebsiteRoles = GetWebsiteRoleUsers(user.Id),
                Role = String.Join(',', userRoles)
            };

            return vm;
        }

        private ApplicationUser TransformFromViewModel(UserRegistrationViewModel model)
        {
            // get a user from db if they exist (need to keep the password etc)
            ApplicationUser user = _userManager.FindByIdAsync(model.Id).Result;
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.IsActive = model.IsActive;
            user.LastName = model.LastName;
            user.PhoneNumber = model.Phone;
            user.UserName = model.UserName;
            user.BillingRoleId = model.BillingRoleId;
            return user;
        }

        public void Destroy(UserRegistrationViewModel model)
        {
            ApplicationUser user = TransformFromViewModel(model);
            user.IsActive = false;
            _userManager.UpdateAsync(user);
        }

        public void Dispose()
        {

        }
        public async Task<IdentityResult> UpdatePassword(UserRegistrationViewModel userPasswordChange)
        {
            var user = await _userManager.FindByIdAsync(userPasswordChange.Id);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            return await _userManager.ResetPasswordAsync(user, token, userPasswordChange.Password);
        }
    }
}
