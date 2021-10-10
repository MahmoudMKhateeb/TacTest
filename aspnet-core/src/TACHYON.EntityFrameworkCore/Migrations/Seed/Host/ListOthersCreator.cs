using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TACHYON.Documents.DocumentTypes;
using TACHYON.EntityFrameworkCore;
using TACHYON.Extension;
using TACHYON.Goods.GoodCategories;
using TACHYON.Shipping.Accidents;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.UnitOfMeasures;
using TACHYON.Vases;

namespace TACHYON.Migrations.Seed.Host
{
    public class ListOthersCreator
    {

        private readonly TACHYONDbContext _dbContext;

        public ListOthersCreator(TACHYONDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create()
        {
            //? Document Type Not Done Yet
            /*x
               - Vas
               - Goods Category
               - Unit Of Measure
               - Transport Type
               - Truck Type
               - Trip Accident
             */

            var otherUnitOfMeasure = new UnitOfMeasure()
            {
                CreationTime = DateTime.Now,
                IsDeleted = false,
            };

            if (_dbContext.UnitOfMeasures.FirstOrDefault(x => x.DisplayName.ToLowerContains(AppConsts.OthersDisplayName)) == null)
            {
                _dbContext.UnitOfMeasures.Add(otherUnitOfMeasure);
            }


            var otherGoodsCategory = new GoodCategory() { CreationTime = DateTime.Now, IsDeleted = false, IsActive = true, Key = AppConsts.OthersDisplayName.ToLower() };

            if (_dbContext.GoodCategories.FirstOrDefault(x => x.Key.ToLowerContains(AppConsts.OthersDisplayName)) == null)
            {
                _dbContext.GoodCategories.Add(otherGoodsCategory);
            }


            var otherVas = new Vas() { CreationTime = DateTime.Now, IsDeleted = false, Name = AppConsts.OthersDisplayName.ToLower(), HasAmount = false, HasCount = true };

            if (_dbContext.Vases.FirstOrDefault(x => x.Name.ToLowerContains(AppConsts.OthersDisplayName)) == null)
            {
                _dbContext.Vases.Add(otherVas);
            }


            var otherTransportType = new TransportType()
            {
                CreationTime = DateTime.Now,
                DisplayName = AppConsts.OthersDisplayName,
                IsDeleted = false
            };


            if (_dbContext.TransportTypes.FirstOrDefault(x => x.DisplayName.ToLowerContains(AppConsts.OthersDisplayName)) == null)
            {
                _dbContext.TransportTypes.Add(otherTransportType);
            }




            var otherTruckType = new TrucksType()
            {
                CreationTime = DateTime.Now,
                IsDeleted = true,
                IsActive = true,
                DisplayName = AppConsts.OthersDisplayName
            };

            if (_dbContext.TrucksTypes.FirstOrDefault(x => x.DisplayName.ToLowerContains(AppConsts.OthersDisplayName)) == null)
            {
                _dbContext.TrucksTypes.Add(otherTruckType);
            }



            var otherReasonAccident = new ShippingRequestReasonAccident()
            {
                CreationTime = DateTime.Now,
                Key = AppConsts.OthersDisplayName,
                IsDeleted = false
            };

            if (_dbContext.ShippingRequestReasonAccidents.FirstOrDefault(x => x.Key.ToLowerContains(AppConsts.OthersDisplayName)) == null)
            {
                _dbContext.ShippingRequestReasonAccidents.Add(otherReasonAccident);
            }




            _dbContext.SaveChanges();
        }
    }
}