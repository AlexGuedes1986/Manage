using Manage.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Controllers
{
    public class ContactsClubsController : Controller
    {
        private readonly IContactClubService _contactClubService;
        public ContactsClubsController(IContactClubService contactClubService)
        {
            _contactClubService = contactClubService;
        }

        public IActionResult Index()
        { 
            var contactClubs = _contactClubService.GetAll();
            return View(contactClubs);
        }
    }
}
