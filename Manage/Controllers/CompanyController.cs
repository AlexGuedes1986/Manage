using AutoMapper;
using BioTech.Constants;
using BioTech.Helpers;
using BioTech.Models;
using BioTech.Services;
using BioTech.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        public CompanyController(ICompanyService companyService, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _companyService = companyService;
            _mapper = mapper;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Companies_Read([DataSourceRequest] DataSourceRequest request)
        {            
            IEnumerable<Company> data = _companyService.GetAll().OrderBy(c => c.Name);            
            var obj = data.ToDataSourceResult(request);
            return Json(obj);
        }

        public IActionResult CompanyAddEdit(int Id, string message)
        {
            Company model = new Company();
            CompanyViewModel companyVM = new CompanyViewModel();
            if (Id > 0)
            {
                model = _companyService.GetById(Id);
                companyVM = _mapper.Map<CompanyViewModel>(model);
            }
            if (!String.IsNullOrEmpty(message))
            {
                ViewBag.CompanyMessage = message;
            }
            companyVM.AvailableStates.Add(new SelectListItem() { Text = "Select State", Value = "" });
            companyVM.AvailableStates.AddRange(States.PopulateAllStates());
            return View(companyVM);
        }

        [HttpPost]
        public IActionResult CompanyAddEdit(Company model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { errors = errorMessages });
                }

                if (model.Id > 0)
                {
                    _companyService.Update(model);
                    return RedirectToAction("CompanyAddEdit", new { Id = model.Id, message = "Company has been updated successfully" });                 
                }                   
                else
                {
                    _companyService.Create(model);
                    return RedirectToAction("CompanyAddEdit", new { message = "Company has been created successfully" });                   
                }             
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                CompanyViewModel companyVM = new CompanyViewModel();
                companyVM.AvailableStates.Add(new SelectListItem() { Text = "Select State", Value = "" });
                companyVM.AvailableStates.AddRange(States.PopulateAllStates());
                return View(companyVM);                
            }
        }

        public IActionResult CompanyDetails(int Id)
        {
            ApplicationUser appUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var websiteRoles = _userManager.GetRolesAsync(appUser).Result;
            ViewBag.CanEdit = RolesUserHelper.CanManage(websiteRoles);
            var company = _companyService.GetById(Id);
            return View(company);
        }

        [AcceptVerbs("Post")]
        public IActionResult Company_Destroy([DataSourceRequest] DataSourceRequest request, Company company)
        {
            if (company != null)
            {
                _companyService.Destroy(company);
            }

            return Json(new[] { company }.ToDataSourceResult(request, null));
        }
    }
}
