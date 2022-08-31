using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Manage.Models;
using Manage.Services;
using Manage.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manage.Helpers;

namespace Manage.Controllers
{
    public class ContactClubController : Controller
    {
        private readonly IContactClubService _contactClubService;
        private readonly IConfiguration _configuration;
        private readonly RestClient _restClient;
        public ContactClubController(IContactClubService contactClubService, IConfiguration configuration, RestClient restClient)
        {
            _contactClubService = contactClubService;
            _configuration = configuration;
            _restClient = restClient;
        }

        public IActionResult Index()
        {            
            return View();
        }

        public IActionResult ContactClubs_Read([DataSourceRequest] DataSourceRequest request)
        { 
            var contactClubs = _contactClubService.GetAll();       
            var obj = contactClubs.ToDataSourceResult(request);
            return Json(obj);
        }

        public IActionResult ContactClubAddEdit()
        {                 
            var countries = _restClient.MakeRequest("/countries");
            ContactClubVM contactClubVM = new ContactClubVM();
            return View(contactClubVM);        
        }
    }
}
