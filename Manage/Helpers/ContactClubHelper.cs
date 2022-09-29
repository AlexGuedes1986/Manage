using Manage.Models;
using Manage.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Helpers
{
    public static class ContactClubHelper
    {
        public static List<Country> GetCountries(RestClient _restClient)
        {
            var responseCountries = _restClient.MakeRequest("/countries");
            var countriesJObject = JObject.Parse(responseCountries);
            var countriesJson = countriesJObject["response"];
            var countries = JsonConvert.DeserializeObject<List<Country>>(countriesJson.ToString());
            return countries;
        }

        public static List<League> GetLeagues(RestClient _restClient, string codeCountry = "")
        {
            var responseLeagues = "";
            if (String.IsNullOrEmpty(codeCountry))
            {
                responseLeagues = _restClient.MakeRequest("/leagues?");
            }
            else
            {
                responseLeagues = _restClient.MakeRequest("/leagues?", $"code={codeCountry}");
            }          
            var leaguesJObject = JObject.Parse(responseLeagues);
            var leaguesJsonResponse = leaguesJObject["response"].ToString();
            List<League> leagues = new List<League>();
            var leaguesJson = JArray.Parse(leaguesJsonResponse);
            foreach (var leagueJson in leaguesJson)
            {
                var leagueJObject = JObject.Parse(leagueJson.ToString());
                var leagueJToken = leagueJObject["league"];
                var countryJToken = leagueJObject["country"];
                var currentLeague = JsonConvert.DeserializeObject<League>(leagueJToken.ToString());
                var currentCountryLeague = JsonConvert.DeserializeObject<Country>(countryJToken.ToString());
                leagues.Add(new League
                {
                    Id = currentLeague.Id,
                    Name = currentLeague.Name,
                    CountryCode = currentCountryLeague.Code
                });
            }
           return leagues;
        }

        public static List<Team> GetTeams(RestClient _restClient, int league)
        {
            var season = DateTime.Now.Year;
            var responseTeams = _restClient.MakeRequest("/teams?", $"league={league}&season={season}");
            var teamsJObject = JObject.Parse(responseTeams);
            var teamsJsonResponse = teamsJObject["response"].ToString();
            var teamsJson = JArray.Parse(teamsJsonResponse);
            List<Team> teams = new List<Team>();
            foreach (var team in teamsJson)
            {
                var teamJObject = JObject.Parse(team.ToString());
                var teamJToken = teamJObject["team"];
                var currentTeam = JsonConvert.DeserializeObject<Team>(teamJToken.ToString());
                teams.Add(new Team
                {
                    Id = currentTeam.Id,
                    Name = currentTeam.Name
                });
            }
            return teams;
        }
    }
}
