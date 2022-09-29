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
        private readonly IManageDataService _manageDataService;
        private readonly ICallService _callService;
        private readonly ICountryService _countryService;
        private readonly ILeagueService _leagueService;
        private readonly IConfiguration _configuration;
        private readonly RestClient _restClient;
        private readonly IMapper _mapper;
        public ContactClubController(IContactClubService contactClubService, ICallService callService, IConfiguration configuration
            , RestClient restClient, IMapper mapper, IManageDataService manageDataService, ICountryService countryService
            , ILeagueService leagueService)
        {
            _contactClubService = contactClubService;
            _configuration = configuration;
            _restClient = restClient;
            _mapper = mapper;
            _callService = callService;
            _manageDataService = manageDataService;
            _countryService = countryService;
            _leagueService = leagueService;
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
     
        public IActionResult ContactClubAddEdit(int contactId = 0, bool doNotGetCountries = false, string codeCountry = "", string league = ""
            , string teamId = "", ContactClubVM contactClubVMParameter = null)
        {
            ContactClubVM contactClubVM = new ContactClubVM();
            var contactClubVMSession = HttpContext.Session.GetString("contactClubVM");           
            if (contactId > 0)
            {
                var contactClub = _contactClubService.GetById(contactId);
                contactClubVM = _mapper.Map<ContactClubVM>(contactClub);
                contactClubVM.AvailableCountries = _countryService.GetAll();
                contactClubVM.AvailableLeagues = _leagueService.GetLeaguesByCountryCode(contactClubVM.CountryCode);
                contactClubVM.AvailableTeams = ContactClubHelper.GetTeams(_restClient, contactClubVM.LeagueId);
                return View(contactClubVM);
            }

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
                if (contactClubVMParameter.Id == 0)
                {
                    contactClub.CreatedDate =  DateTime.Now;
                    _contactClubService.Create(contactClub);
                }
                else
                {
                    contactClub.ModifiedDate = DateTime.Now;
                    _contactClubService.Update(contactClub);
                }

                return View(contactClubVM);
            }

        }

        public IActionResult CallLogs_Read([DataSourceRequest] DataSourceRequest request, int contactClubId)
        {
            IEnumerable<Call> data = _callService.GetCallLogsByContactId(contactClubId);
            var obj = data.ToDataSourceResult(request);
            return Json(obj);
        }

        public IActionResult AddCallLog(int contactId)
        {
            if (!(contactId > 0))
            {
                return RedirectToAction("Index");
            }
            ContactClub contactClub = _contactClubService.GetById(contactId);
            Call call = new Call();
            call.ContactClubId = contactId;           
            return View(call);
        }

        [AcceptVerbs("Post")]
        public IActionResult AddCallLog(Call model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            model.Date = DateTime.Now;        
            _callService.Create(model);
            return RedirectToAction("ContactClubAddEdit", new { Id = model.ContactClubId });
        }
        public IActionResult RefreshDatabase()
        {
            var countries = ContactClubHelper.GetCountries(_restClient);
            _manageDataService.UpdateCountries(countries);

            List<League> leagues = ContactClubHelper.GetLeagues(_restClient);
            _manageDataService.UpdateLeagues(leagues);
            return new EmptyResult();
        }
    }
}
