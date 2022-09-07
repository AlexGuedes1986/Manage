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
        public IActionResult ContactClubAddEdit(bool doNotGetCountries, string codeCountry = "", string league = "", string teamId = "")
        {
            var contactClubVMSession = HttpContext.Session.GetString("contactClubVM");
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
                List<LeagueVM> leagues = new List<LeagueVM>();   
                var leaguesJson = JArray.Parse(leaguesJsonResponse);
                foreach (var leagueJson in leaguesJson)
                {
                    var leagueJObject = JObject.Parse(leagueJson.ToString());
                    var leagueJToken = leagueJObject["league"];
                    var currentLeague = JsonConvert.DeserializeObject<LeagueVM>(leagueJToken.ToString());
                    leagues.Add(new LeagueVM
                    {
                        Id = currentLeague.Id,
                        Name = currentLeague.Name
                    });
                }
                contactClubVM.AvailableLeagues = leagues; 
                contactClubVM.Country.Code = codeCountry;
            }
            if (!String.IsNullOrEmpty(league))
            {
                var season = DateTime.Now.Year;
                var responseTeams = _restClient.MakeRequest("/teams?", $"league={league}&season={season}");
                var teamsJObject = JObject.Parse(responseTeams);
                var teamsJsonResponse = teamsJObject["response"].ToString();
                var teamsJson = JArray.Parse(teamsJsonResponse);
                List<TeamVM> teams = new List<TeamVM>();
                foreach (var team in teamsJson)
                {
                    var teamJObject = JObject.Parse(team.ToString());
                    var teamJToken = teamJObject["team"];
                    var currentTeam = JsonConvert.DeserializeObject<TeamVM>(teamJToken.ToString());
                    teams.Add(new TeamVM
                    {
                        Id = currentTeam.Id,
                        Name = currentTeam.Name
                    });
                }
                contactClubVM.AvailableTeams = teams;
                contactClubVM.League = league;
            }
            if (!String.IsNullOrEmpty(teamId))
            {
                //var responseTeam = _restClient.MakeRequest("/teams?", $"id={teamId}");
                //var teamJObject = JObject.Parse(responseTeam);
                //var teamJsonResponse = teamJObject["response"].ToString();
                //var teamJson = JArray.Parse(teamJsonResponse);

                //var teamJsonJObject = JObject.Parse(teamJson.ToString());
                //var teamJsonJToken = teamJsonJObject["team"];
                //var currentTeam = JsonConvert.DeserializeObject<TeamVM>(teamJsonJToken.ToString());
                contactClubVM.Team = teamId;                
            }
            HttpContext.Session.SetString("contactClubVM", JsonConvert.SerializeObject(contactClubVM));
            return View(contactClubVM);        
        }
    }
}
