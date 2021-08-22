using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Documents.DocumentTypes;
using TACHYON.EntityFrameworkCore;
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
                CreationTime = DateTime.Now, DisplayName = "Others", IsDeleted = false,
            };
            _dbContext.UnitOfMeasures.Add(otherUnitOfMeasure);

            var otherGoodsCategory = new GoodCategory() {CreationTime = DateTime.Now,IsDeleted = false, IsActive = true, Key = "Others"};
            _dbContext.GoodCategories.Add(otherGoodsCategory);

            var otherVas = new Vas(){CreationTime = DateTime.Now,IsDeleted = false,Name = "Others",HasAmount = false,HasCount = true};
            _dbContext.Vases.Add(otherVas);

            var otherTransportType = new TransportType()
            {
                CreationTime = DateTime.Now, DisplayName = "Others", IsDeleted = false
            };
            _dbContext.TransportTypes.Add(otherTransportType);

            var otherTruckType = new TrucksType()
            {
                CreationTime = DateTime.Now, IsDeleted = true, IsActive = true, DisplayName = "Other"
            };
            _dbContext.TrucksTypes.Add(otherTruckType);

            var otherReasonAccident = new ShippingRequestReasonAccident()
            {
                CreationTime = DateTime.Now, Key = "Other", IsDeleted = false
            };
            _dbContext.ShippingRequestReasonAccidents.Add(otherReasonAccident);

            _dbContext.SaveChanges();
        }
    }
}
