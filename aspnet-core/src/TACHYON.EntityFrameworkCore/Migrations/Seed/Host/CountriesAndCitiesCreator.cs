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
            //insert countries
            foreach (var co in countryObject.Properties())
            {
                var country = co.Name;
                    
                if (countriesList.Contains(country))
                {
                    break;
                }
                int countryId = AddCountryToDB(country).Id;
                if (country == "Saudi Arabia")
                {
                    //insert citites
                    foreach (var ci in co.Value.ToArray())
                    {
                        string city = ci.ToString();
                        CreateCityToDB(city, countryId);
                    }
                }
                    
            }
            
        }
        private County AddCountryToDB(string displayName)
        {
            var item= _context.Counties.AddAsync(new County { Code = "", CreationTime = Clock.Now, IsDeleted = false, DisplayName = displayName });
            _context.SaveChanges();
            return _context.Counties.FirstOrDefault(x => x.DisplayName == displayName);
        }

        private void CreateCityToDB(string cityName, int countryId)
        {
            _context.Cities.Add(new City { CountyId = countryId, IsDeleted = false, DisplayName = cityName, CreationTime = Clock.Now });
            _context.SaveChanges();
        }

    }
}
