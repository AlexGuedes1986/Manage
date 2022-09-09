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
        public static List<CountryVM> GetCountries(RestClient _restClient)
        {
            var responseCountries = _restClient.MakeRequest("/countries");
            var countriesJObject = JObject.Parse(responseCountries);
            var countriesJson = countriesJObject["response"];
            var countries = JsonConvert.DeserializeObject<List<CountryVM>>(countriesJson.ToString());
            return countries;
        }

        public static List<LeagueVM> GetLeagues(RestClient _restClient, string codeCountry)
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
           return leagues;
        }

        public static List<TeamVM> GetTeams(RestClient _restClient, string league)
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
            return teams;
        }
    }
}
