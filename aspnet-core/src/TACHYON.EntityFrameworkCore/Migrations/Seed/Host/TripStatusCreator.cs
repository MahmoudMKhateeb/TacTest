using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TACHYON.EntityFrameworkCore;
using TACHYON.Shipping.TripStatuses;

namespace TACHYON.Migrations.Seed.Host
{
    public class TripStatusCreator
    {
        private readonly TACHYONDbContext _context;
        public TripStatusCreator(TACHYONDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateOrUpdateTripStatus(1,"StandBy");
            CreateOrUpdateTripStatus(2,"Start loading");
            CreateOrUpdateTripStatus(3,"Start offloading");
            CreateOrUpdateTripStatus(4,"Start moving to the next location");
            CreateOrUpdateTripStatus(5,"Arrived at the next location");
            CreateOrUpdateTripStatus(6,"finalized");
        }

        private void CreateOrUpdateTripStatus(int id,string displayName)
        {
            var item = _context.TripStatuses.IgnoreQueryFilters().FirstOrDefault(x =>x.Id==id && x.DisplayName == displayName);
            if (item == null)
            {
                AddStatusToDB(id,displayName);
            }
            else
            {
                UpdateStatus(item, displayName);
            }
        }

        private void AddStatusToDB(int id, string displayName)
        {
            var item = new TripStatus() {Id = id, DisplayName = displayName};
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[TripStatuses] ON");
            _context.TripStatuses.Add(item);
            _context.SaveChanges();
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[TripStatuses] OFF");
        }

        private void UpdateStatus(TripStatus item, string displayName)
        {
            item.DisplayName = displayName;
            item.IsDeleted = false;
            _context.TripStatuses.Update(item);
            _context.SaveChanges();
        }

    }
}
