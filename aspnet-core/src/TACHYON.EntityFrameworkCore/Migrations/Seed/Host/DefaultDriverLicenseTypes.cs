﻿using System;
using System.Linq;
using TACHYON.DriverLicenseTypes;
using TACHYON.EntityFrameworkCore;

namespace TACHYON.Migrations.Seed.Host
{
    public class DefaultDriverLicenseTypes
    {
        private readonly TACHYONDbContext _context;

        public DefaultDriverLicenseTypes(TACHYONDbContext context)
        {
            _context = context;
        }


        public void Create()
        {
            var driverLicenseTypeCount = _context.DriverLicenseTypes.Count();

            if (driverLicenseTypeCount == 0)
                AddPredefinedLicenseTypes();
        }

        private void AddPredefinedLicenseTypes()
        {
            var temporaryLicense = new DriverLicenseType()
            {
                Name = "TEMPORARY LICENSE (PERMISSION)",
                CreationTime = DateTime.Now,
                WasIIntegrationId = 1,
            };

            var motorcycle = new DriverLicenseType()
            {
                Name = "MOTORCYCLE",
                CreationTime = DateTime.Now,
                WasIIntegrationId = 2,
            };

            var privateLicenceType = new DriverLicenseType()
            {
                Name = "PRIVATE",
                CreationTime = DateTime.Now,
                WasIIntegrationId = 3,
            };

            var publicTaxi = new DriverLicenseType()
            {
                Name = "Public Taxi",
                CreationTime = DateTime.Now,
                WasIIntegrationId = 4,
            };

            _context.DriverLicenseTypes.AddRange(temporaryLicense, motorcycle, privateLicenceType, publicTaxi);
            _context.SaveChanges();
        }

    }
}