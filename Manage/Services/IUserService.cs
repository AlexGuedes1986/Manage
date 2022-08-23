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
    public interface IUserService
    {
        IEnumerable<UserRegistrationViewModel> GetAll();

        void Create(UserRegistrationViewModel model);

        UserRegistrationViewModel GetById(string Id);

        UserRegistrationViewModel GetByName(string name);

        List<SelectListItem> GetRoleSelectListItems();

        void Update(UserRegistrationViewModel model);

        void Destroy(UserRegistrationViewModel model);

        bool CheckEmail(string email);
        bool CheckUsername(string username);

        string GetEmaiBylID(string ID);

        void UpdatebyAdmin(UserRegistrationViewModel model);

        List<RoleUser> GetWebsiteRoleUsers(string userId = null);

        IEnumerable<ApplicationUser> GetAllApplicationUsers();
        List<ApplicationUser> GetPmsByIds(List<string> pmIds);
        Task<IdentityResult> UpdatePassword(UserRegistrationViewModel userPasswordChange);
    }
}
