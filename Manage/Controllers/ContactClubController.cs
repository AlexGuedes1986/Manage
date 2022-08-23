using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Manage.Models;
using Manage.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Controllers
{
    public class ContactClubController : Controller
    {
        private readonly IContactClubService _contactClubService;
        public ContactClubController(IContactClubService contactClubService)
        {
            _contactClubService = contactClubService;
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
    }
}
