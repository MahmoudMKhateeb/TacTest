using Abp.Timing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using TACHYON.Cities;
using TACHYON.Countries;
using TACHYON.EntityFrameworkCore;

namespace TACHYON.Migrations.Seed.Host
{
    public class CountriesAndCitiesCreator
    {
        private readonly TACHYONDbContext _context;

        public CountriesAndCitiesCreator(TACHYONDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            var json=new WebClient().DownloadString("https://raw.githubusercontent.com/David-Haim-zz/CountriesToCitiesJSON/master/countriesToCities.json");
            var countryObject = JObject.Parse(json);

            List<string> countriesList = _context.Counties.Where(x => x.IsDeleted == false).Select(x => x.DisplayName).ToList();
            List<string> WantedCountriesList = new List<string>
                    { "Saudi Arabia", "MENA", "United Arab Emirates", "Bahrain", "Oman", "Iraq", "Hashemite Kingdom of Jordan",
                        "Egypt", "Turkey", "Libya", "Sudan" ,"Kuwait"
                    };
            //insert countries
            foreach (var co in countryObject.Properties())
            {
                var country = co.Name;
                    
                //if the first country exists , check cities
                if (countriesList.Contains(country))
                {
                    //break;
                    //contries which their cities will be added

                    if (WantedCountriesList.Contains(country))
                    {
                        int countryid = GetCountryId(country);
                        if (countryid != 0)
                        {
                            if (!_context.Cities.Any(x => x.CountyId == countryid))
                            {
                                //insert citites
                                foreach (var ci in co.Value.ToArray())
                                {
                                    string city = ci.ToString();
                                    CreateCityToDB(city, countryid);
                                }
                            }
                        }
                    }
                }

                //insert all countries
                else if (!string.IsNullOrWhiteSpace(country))
                {
                    int? countryId = AddCountryToDB(country)?.Id;
                    if (countryId != null)
                    {
                        if (WantedCountriesList.Contains(country))
                        {
                            //insert all citites
                            foreach (var ci in co.Value.ToArray())
                            {
                                string city = ci.ToString();
                                CreateCityToDB(city, countryId.Value);
                            }
                        }
                    }
                }

                    
            }
            
        }
        private County AddCountryToDB(string displayName)
        {
            var itemDB = _context.Cities.FirstOrDefault(x => x.DisplayName == displayName);
            if (itemDB == null)
            {
                var item = _context.Counties.AddAsync(new County { Code = "", CreationTime = Clock.Now, IsDeleted = false, DisplayName = displayName });
                _context.SaveChanges();
                return _context.Counties.FirstOrDefault(x => x.DisplayName == displayName);
            }
            return null;
        }

        private int GetCountryId(string displayName)
        {
            var item = _context.Counties.FirstOrDefault(x=>x.DisplayName==displayName);
            if (item != null)
                return item.Id;
            return 0;
        }

        private void CreateCityToDB(string cityName, int countryId)
        {
            _context.Cities.Add(new City { CountyId = countryId, IsDeleted = false, DisplayName = cityName, CreationTime = Clock.Now });
            _context.SaveChanges();

        }

    }
}
