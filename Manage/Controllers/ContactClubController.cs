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

        //[HttpGet("ContactClub/ContactClubAddEdit/{getCountries}/{contactClubVM?}/{codeLeague?}", Name = "ContactClubAddEdit")]
        public IActionResult ContactClubAddEdit(bool doNotGetCountries, string codeCountry = "", string leagueId = "", string teamId = "")
        {
            var contactClubVMSession = HttpContext.Session.GetString("contactClubVM");     //HttpContext.Session.get("contactClubVM", JsonConvert.DeserializeObject<ContactClubVM>(contactClubVM));
            ContactClubVM contactClubVM = new ContactClubVM();

            if (!String.IsNullOrEmpty(contactClubVMSession))
            {
                contactClubVM = JsonConvert.DeserializeObject<ContactClubVM>(contactClubVMSession);
            }

            if (!doNotGetCountries)
            {
                var responseCountries = _restClient.MakeRequest("/countries");
                var countriesJObject = JObject.Parse(responseCountries);
                var countriesJson = countriesJObject["response"];
                var countries = JsonConvert.DeserializeObject<List<CountryVM>>(countriesJson.ToString());
                contactClubVM.AvailableCountries = countries;
            }
        
            if (!String.IsNullOrEmpty(codeCountry))
            {
                var responseLeagues = _restClient.MakeRequest("/leagues?", $"code={codeCountry}");
                var leaguesJObject = JObject.Parse(responseLeagues);
                var leaguesJsonResponse = leaguesJObject["response"].ToString();
                var leaguesJson = JArray.Parse(leaguesJsonResponse);
                foreach (var league in leaguesJson)
                {
                    var leagueJObject = JObject.Parse(league.ToString());
                    var leagueJToken = leagueJObject["league"];
                    var currentLeague = JsonConvert.DeserializeObject<LeagueVM>(leagueJToken.ToString());
                    contactClubVM.AvailableLeagues.Add(new LeagueVM
                    {
                        Id = currentLeague.Id,
                        Name = currentLeague.Name
                    });
                }
            }
            HttpContext.Session.SetString("contactClubVM", JsonConvert.SerializeObject(contactClubVM));
            return View(contactClubVM);        
        }
    }
}
