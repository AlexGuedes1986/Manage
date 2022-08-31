using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.ViewModels
{
    public class ContactClubVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> AvailableCountries { get; set; }
        public string Country { get; set; }
        public string Team { get; set; }
        public string League { get; set; }
        public string Position { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public ContactClubVM()
        {
            AvailableCountries = GetCountries();
        }

        public List<string> GetCountries()
        { 
            List<string> cultureList = new List<string>();
            CultureInfo[] getCultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo getCulture in getCultureInfo)
            {
                RegionInfo getRegionInfo = new RegionInfo(getCulture.LCID);
                if (!cultureList.Contains(getRegionInfo.EnglishName))
                {
                    cultureList.Add(getRegionInfo.EnglishName);
                }
            }
            cultureList.Sort();
            return cultureList;
        }
    }
}
