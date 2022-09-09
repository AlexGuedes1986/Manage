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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using AutoMapper;

namespace Manage.Controllers
{
    public class ContactClubController : Controller
    {
        private readonly IContactClubService _contactClubService;
        private readonly IConfiguration _configuration;
        private readonly RestClient _restClient;
        private readonly IMapper _mapper;
        public ContactClubController(IContactClubService contactClubService, IConfiguration configuration, RestClient restClient, IMapper mapper)
        {
            _contactClubService = contactClubService;
            _configuration = configuration;
            _restClient = restClient;
            _mapper = mapper;
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

        //[HttpGet("ContactClub/ContactClubAddEdit/{getCountries}/{contactClubVM?}/{codeLeague?}", Name = "ContactClubAddEdit")]
        public IActionResult ContactClubAddEdit(bool doNotGetCountries, string codeCountry = "", string league = "", string teamId = "", ContactClubVM contactClubVMParameter = null)
        {
            ContactClubVM contactClubVM = new ContactClubVM();
            var contactClubVMSession = HttpContext.Session.GetString("contactClubVM");

            if (!String.IsNullOrEmpty(contactClubVMSession))
            {
                contactClubVM = JsonConvert.DeserializeObject<ContactClubVM>(contactClubVMSession);
            }
            if (String.IsNullOrEmpty(contactClubVMParameter.Email) && String.IsNullOrEmpty(contactClubVMParameter.PhoneNumber) && String.IsNullOrEmpty(contactClubVMParameter.Name)
                && String.IsNullOrEmpty(contactClubVMParameter.Position))
            {
                if (!doNotGetCountries)
                {                  
                    var countries = ContactClubHelper.GetCountries(_restClient);
                    contactClubVM.AvailableCountries = countries;
                }

                if (!String.IsNullOrEmpty(codeCountry))
                {
                    var leagues = ContactClubHelper.GetLeagues(_restClient, codeCountry);
                    contactClubVM.AvailableLeagues = leagues;
                    contactClubVM.CountryCode = codeCountry;
                }

                if (!String.IsNullOrEmpty(league))
                {
                    var teams = ContactClubHelper.GetTeams(_restClient, league);
                    contactClubVM.AvailableTeams = teams;
                    contactClubVM.League = league;
                }

                if (!String.IsNullOrEmpty(teamId))
                {
                    contactClubVM.Team = teamId;
                }

                HttpContext.Session.SetString("contactClubVM", JsonConvert.SerializeObject(contactClubVM));
                return View(contactClubVM);
            }
            else
            {
                var contactClub = _mapper.Map<ContactClub>(contactClubVMParameter);
                _contactClubService.Create(contactClub);             
                return View(contactClubVM);
            }

           
        }
    }
}
