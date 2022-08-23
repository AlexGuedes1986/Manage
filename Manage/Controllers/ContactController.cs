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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BioTech.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;
        private readonly IMapper _mapper;
        private readonly ICompanyService _companyService;
        private readonly ITouchLogService _touchLogService;
        private readonly UserManager<ApplicationUser> _userManager;
        public ContactController(IContactService contactService, IMapper mapper, ICompanyService companyService
            , ITouchLogService touchLogService, UserManager<ApplicationUser> userManager)
        {
            _contactService = contactService;
            _mapper = mapper;
            _companyService = companyService;
            _touchLogService = touchLogService;
            _userManager = userManager;
        }

        public ActionResult Index()
        {
            return View();
        }

        public IActionResult Contacts_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<ContactFlatKendoGridVM> contactsKendoGrid = new List<ContactFlatKendoGridVM>();
            IEnumerable<Contact> data = _contactService.GetAll();
            foreach (var contact in data)
            {
                contactsKendoGrid.Add(new ContactFlatKendoGridVM
                {
                    Id = contact.Id,
                    CompanyName = contact.Company.Name,                   
                    BussinessPhone = contact.BusinessPhone,
                    EmailAddress = contact.EmailAddress,
                    FirstName = contact.FirstName,
                    LastName = contact.LastName,
                    Active = contact.Active
                });
            }
            var sortedcontactsKendoGrid = contactsKendoGrid.OrderBy(c => c.LastName);
            var obj = sortedcontactsKendoGrid.ToDataSourceResult(request);
            return Json(obj);
        }

        public IActionResult ContactDetails(int Id)
        {
            ApplicationUser appUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var websiteRoles = _userManager.GetRolesAsync(appUser).Result;
            ViewBag.CanEdit = RolesUserHelper.CanManage(websiteRoles);
            var contact = _contactService.GetById(Id);
            return View(contact);
        }

        public IActionResult ContactAddEdit(int Id)
        {
            Contact model = new Contact();
            ContactViewModel contactVM = new ContactViewModel();
            if (Id > 0)
            {
                model = _contactService.GetById(Id);
                contactVM = _mapper.Map<ContactViewModel>(model);
            }
            contactVM.AvailableContactTypes.Add(new SelectListItem() { Text = "Select Contact Type", Value = "" });
            contactVM.AvailableStates.Add(new SelectListItem() { Text = "Select State", Value = "" });
            contactVM.AvailableCompanies.Add(new SelectListItem() { Text = "Select Company", Value = "0" });
            contactVM.AvailableContactTypes.AddRange(ContactType.ContactTypes.OrderBy(ct => ct.Text));
            contactVM.AvailableStates.AddRange(States.PopulateAllStates());
            var allCompanies = _companyService.GetAll();
            contactVM.AvailableCompanies.AddRange(Helper.CompaniesToSelectListItems(allCompanies));
            return View(contactVM);
        }

        [HttpPost]
        public IActionResult ContactAddEdit(ContactViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var allCompanies = _companyService.GetAll();
                    Helper.PopulateDropDownsWhenErrorAddingContact(model, allCompanies);
                    model.Company = _companyService.GetById(model.CompanyId);
                    return View(model);
                }
                Contact contact = new Contact()
                {
                    Id = model.Id > 0 ? model.Id : 0,
                    Company = model.Company,
                    CompanyId = model.CompanyId,
                    ContactType = model.ContactType,
                    EmailAddress = model.EmailAddress,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    MobilePhone = model.MobilePhone,
                    JobTitle = model.JobTitle,
                    Suffix = model.Suffix,
                    BusinessPhone = model.BusinessPhone,
                    Active = model.Active
                };


                if (model.Id > 0)
                    _contactService.Update(contact);
                else
                    _contactService.Create(contact);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var allCompanies = _companyService.GetAll();
                Helper.PopulateDropDownsWhenErrorAddingContact(model, allCompanies);
                ModelState.AddModelError("", ex.Message);
                return View("~/Views/Contact/ContactAddEdit.cshtml", model);
            }
        }

        [AcceptVerbs("Post")]
        public ActionResult Contact_Destroy([DataSourceRequest] DataSourceRequest request, Contact user)
        {
            if (user != null)
            {
                var contact = _contactService.GetById(user.Id);
                contact.Active = false;
                _contactService.Update(contact);
            }
            return RedirectToAction("Index");        
        }

        [AcceptVerbs("Post")]
        public ActionResult Contact_Update(int id, string type)
        {
            if (id > 0)
            {
                var contact = _contactService.GetById(id);
                if (type == "Deactivate")
                {
                    contact.Active = false;
                }
                else
                {
                    contact.Active = true;
                }               
              
                _contactService.Update(contact);
            }
            return Json(new { success ="success"});
         
        }

        public IActionResult TouchLogs_Read([DataSourceRequest] DataSourceRequest request, int contactId)
        {
            IEnumerable<TouchLog> data = _touchLogService.GetTouchLogsByContactId(contactId);
            var obj = data.ToDataSourceResult(request);
            return Json(obj);
        }

        public IActionResult AddTouchLog(int contactId)
        {
            if (!(contactId > 0))
            {
                return RedirectToAction("Index");
            }
            Contact currentContact = _contactService.GetById(contactId);
            TouchLog touchLog = new TouchLog();
            touchLog.ContactId = contactId;
            touchLog.UserId = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;
            ViewBag.ContactName = $"{currentContact?.FirstName } {currentContact?.LastName}";
            return View(touchLog);
        }

        [AcceptVerbs("Post")]
        public IActionResult AddTouchLog(TouchLog model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser appUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var employee = $"{appUser?.FirstName } {appUser?.LastName}";
            model.Date = DateTime.Now;
            model.Employee = employee;
            _touchLogService.Create(model);
            return RedirectToAction("ContactDetails", new { Id = model.ContactId });
        }

        [HttpPost]
        public JsonResult AutoCompleteCompany(string prefix)
        {
            var companyByPrefix = _companyService.GetAll().Where(c => c.Name.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(c => c.Name)
                .Select(obj => new
                {
                    label = $"{obj.Name}",
                    val = obj.Id,
                    address1 = obj.AddressLine1,
                    city = obj.City,
                    state = obj.State,
                    zipcode = obj.ZipCode
                });
            return Json(companyByPrefix);
        }

    }
}
