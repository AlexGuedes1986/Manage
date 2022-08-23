using AutoMapper;
using BioTech.Constants;
using BioTech.Models;
using BioTech.Services;
using BioTech.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using BioTech.Helpers;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BioTech.Controllers
{
    [Authorize(Roles="Admin,Project Manager")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        private readonly IBillingRoleService _billingRoleService;
        private readonly ILogger _logger;
        private readonly IProposalApplicationUserService _proposalApplicationUserService;

        public AdminController(IUserService userService, RoleManager<ApplicationRole> roleManager, ICompanyService companyService
           ,IMapper mapper, IBillingRoleService billingRoleService, ILogger<AdminController> logger
            , IProposalApplicationUserService proposalApplicationUserService)
        {
            _userService = userService;
            _roleManager = roleManager;
            _companyService = companyService;
            _billingRoleService = billingRoleService;
            _mapper = mapper;
            _logger = logger;
            _proposalApplicationUserService = proposalApplicationUserService;
        }

        public ActionResult Index()
        {
            return View();
        }

        #region Users

        public IActionResult Users_Read([DataSourceRequest] DataSourceRequest request)
        {
            try
            {
                IEnumerable<UserRegistrationViewModel> data = _userService.GetAll();
                foreach (var userRegistrationVM in data)
                {
                    userRegistrationVM.BillingRole = _billingRoleService.GetBillingRoleById(userRegistrationVM.BillingRoleId).Name;
                }
                var obj = data.ToDataSourceResult(request);
                return Json(obj);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
                throw;
            }
           
        }

        public IActionResult EditUser(string Id)
        {
            UserRegistrationViewModel model = _userService.GetById(Id);
            var userRoles = _userService.GetWebsiteRoleUsers(Id);
            model.WebsiteRoles = _userService.GetWebsiteRoleUsers().OrderBy(wsr => wsr.Name)?.ToList();
            RolesUserHelper.SetAndReturnSelectedUserWebsiteRoles(model);           
            model.BillingRoles = _billingRoleService.GetBillingRolesSelectListItems().ToList();
            RolesUserHelper.SetAndReturnSelectedUserBillingRoles(model);         
            return View("~/Views/Admin/UsersAddEdit.cshtml", model);
        }

        [HttpPost]
        public IActionResult EditUser(UserRegistrationViewModel model)
        {
            // on an update we don't set the password
            ModelState.Remove("Password");
            StringBuilder websiteRolesSB = new StringBuilder();
            var isRoleSelected = false;
            foreach (var websiteRole in model.WebsiteRoles)
            {
                if (websiteRole.IsSelected == true)
                {
                    isRoleSelected = true;
                }
            }
           
            if (!isRoleSelected)
            {
                ModelState.AddModelError("Role","You must select at least one website role");
            }

            // Compare if we have a new Email address for this user
            string OldEmail = _userService.GetEmaiBylID(model.Id);
            string NewEmail = model.Email;            
                if (OldEmail != NewEmail)
                {
                    if (_userService.CheckEmail(model.Email))
                    {
                    ModelState.AddModelError("Email", $"Email {NewEmail} is already taken");                      
                    }

                }
            string OldUsername = _userService.GetById(model.Id).UserName;
            string NewUsername = model.UserName;
            if (OldUsername != NewUsername)
            {
                //Checking if new username exists already in the db
                var userExistsAlready = _userService.CheckUsername(model.UserName);
                if (userExistsAlready)
                {
                    ModelState.AddModelError("UserName", "Username already registered, please select a different username");
                }
            }
           

            if (ModelState.IsValid)
            {
                // save and return to grid
                _userService.UpdatebyAdmin(model);
                return RedirectToAction("Index");
            }
            
            UserRegistrationViewModel originalModelOtherInstance = _userService.GetById(model.Id);
            originalModelOtherInstance.WebsiteRoles = _userService.GetWebsiteRoleUsers();
            RolesUserHelper.SetAndReturnSelectedUserWebsiteRoles(originalModelOtherInstance);
            originalModelOtherInstance.BillingRoles = _billingRoleService.GetBillingRolesSelectListItems().ToList();
            RolesUserHelper.SetAndReturnSelectedUserBillingRoles(originalModelOtherInstance);
            return View("~/Views/Admin/UsersAddEdit.cshtml", originalModelOtherInstance);           
        }

        [AcceptVerbs("Post")]
        public ActionResult Users_Update([DataSourceRequest] DataSourceRequest request, UserRegistrationViewModel user)
        {
            ModelState.AddModelError("Password", "Test error message");

            if (user != null && ModelState.IsValid)
            {
                _userService.Update(user);
            }

            return Json(new[] { user }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs("Post")]
        public ActionResult Users_Destroy([DataSourceRequest] DataSourceRequest request, UserRegistrationViewModel user)
        {
            if (user != null)
            {
                _userService.Destroy(user);
                _proposalApplicationUserService.DestroyByUserId(user.Id);
            }

            return Json(new[] { user }.ToDataSourceResult(request, null));
        }

        public IActionResult ChangePassword(string userId)
        {
            var user = _userService.GetById(userId);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(UserRegistrationViewModel userChangePwd)
        {            
           var result = await _userService.UpdatePassword(userChangePwd);
            if (result.Succeeded)
            {
                ViewBag.PasswordUpdated = "Password have been updated";              
            }           
            return View(userChangePwd);
        }

        #endregion

        #region Company

        public IActionResult Companies_Read([DataSourceRequest] DataSourceRequest request)
        {
            IEnumerable<Company> data = _companyService.GetAll();
            var obj = data.ToDataSourceResult(request);
            return Json(obj);
        }

        [AcceptVerbs("Post")]
        public IActionResult Companies_Update([DataSourceRequest] DataSourceRequest request, Company model)
        {
            _companyService.Update(model);
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs("Post")]
        public IActionResult Companies_Destroy([DataSourceRequest] DataSourceRequest request, Company model)
        {
            try
            {
                _companyService.Destroy(model);
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }
        }
        public IActionResult CompanyAddEdit(int Id)
        {
            Company model = new Company();
            CompanyViewModel companyVM = new CompanyViewModel();
            if (Id > 0)
            {
                model = _companyService.GetById(Id);
                companyVM = _mapper.Map<CompanyViewModel>(model);
            }
            companyVM.AvailableStates.Add(new SelectListItem() { Text = "Select State", Value="" });
            companyVM.AvailableStates.AddRange(States.PopulateAllStates());           
            return View(companyVM);         
        }

        [HttpPost]
        public IActionResult CompanyAddEdit(Company model)
        {
            try
            {
                if (!ModelState.IsValid) {
                    var errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);                   
                    return Json(new { errors = errorMessages });
                        }

                if (model.Id > 0)
                    _companyService.Update(model);
                else
                    _companyService.Create(model);
                var allCompanies = _companyService.GetAll();
                return Json(allCompanies);                
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Json(new { errors = ex.Message});               
            }
        }
        #endregion       

    }
}
