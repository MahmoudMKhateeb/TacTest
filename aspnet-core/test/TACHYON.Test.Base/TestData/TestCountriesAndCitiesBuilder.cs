using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Cities;
using TACHYON.EntityFrameworkCore;

namespace TACHYON.Test.Base.TestData
{
    public class TestCountriesAndCitiesBuilder
    {
        private readonly TACHYONDbContext _context;

        public TestCountriesAndCitiesBuilder(TACHYONDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateCountry("SA", "SA");
            _context.SaveChanges();
            CreateCity("Maka", "Maka", "SA");
            _context.SaveChanges();
        }


        private void CreateCountry(string displayName, string code)
        {
            var country = new Countries.County() { DisplayName = displayName, Code = code, };
            _context.Counties.Add(country);
        }

        private void CreateCity(string displayName,
            string code,
            string countryCode)
        {
            var city = new City()
            {
                DisplayName = displayName,
                Code = code,
                CountyFk = _context.Counties.Single(x => x.Code == countryCode)
            };

            _context.Cities.Add(city);
        }
    }
}