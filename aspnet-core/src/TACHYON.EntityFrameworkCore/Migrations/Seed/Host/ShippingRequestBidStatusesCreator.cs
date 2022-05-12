//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Abp.Timing;
//using Microsoft.EntityFrameworkCore;
//using PayPalCheckoutSdk.Orders;
//using TACHYON.Documents.DocumentsEntities;
//using TACHYON.EntityFrameworkCore;
//using TACHYON.Shipping.ShippingRequestBidStatuses;
//using TACHYON.Trucks.TruckCategories.TruckCapacities.Dtos;

//namespace TACHYON.Migrations.Seed.Host
//{
//    class ShippingRequestBidStatusesCreator
//    {
//        private readonly TACHYONDbContext _context;
//        public ShippingRequestBidStatusesCreator(TACHYONDbContext context)
//        {
//            _context = context;
//        }

//        public void Create()
//        {
//            CreateOrUpdateStatus(1, "StandBy");
//            CreateOrUpdateStatus(2, "Ongoing");
//            CreateOrUpdateStatus(3, "Closed");
//            CreateOrUpdateStatus(4, "Canceled");
//        }
//        private void CreateOrUpdateStatus(int id, string displayName)
//        {
//            var item = _context.ShippingRequestBidStatuses.IgnoreQueryFilters().FirstOrDefault(x => x.Id == id);
//            if (item != null)
//            {
//                UpdateStatus(item, displayName);
//            }
//            else
//            {
//                AddStatusToDBwithId(id, displayName);
//            }
//        }
//        private void AddStatusToDBwithId(int id,string displayName)
//        {
//            var item = new ShippingRequestBidStatus() { Id = id, IsDeleted = false, DisplayName = displayName, CreationTime = Clock.Now };
//            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[ShippingRequestBidStatuses] ON");
//            _context.ShippingRequestBidStatuses.Add(item);
//            _context.SaveChanges();
//            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[ShippingRequestBidStatuses] OFF");
//        }
//        private void UpdateStatus(ShippingRequestBidStatus item, string DisplayName)
//        {
//            item.DisplayName = DisplayName;
//            item.IsDeleted = false;
//            _context.ShippingRequestBidStatuses.Update(item);
//            _context.SaveChanges();
//        }


//    }
//}

