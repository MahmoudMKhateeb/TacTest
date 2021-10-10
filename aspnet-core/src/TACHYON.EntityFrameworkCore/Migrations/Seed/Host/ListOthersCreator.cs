using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TACHYON.Common;
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
                CreationTime = DateTime.Now,
                IsDeleted = false,
            };


            if (!_dbContext.UnitOfMeasures.Any(OthersExpressions.ContainsOthersDisplayNameExpression))
            {
                _dbContext.UnitOfMeasures.Add(otherUnitOfMeasure);
            }


            var otherGoodsCategory = new GoodCategory() { CreationTime = DateTime.Now, IsDeleted = false, IsActive = true, Key = TACHYONConsts.OthersDisplayName.ToLower() };

            if (!_dbContext.GoodCategories.Any(OthersExpressions.ContainsOthersKeyExpression))
            {
                _dbContext.GoodCategories.Add(otherGoodsCategory);
            }


            var otherVas = new Vas() { CreationTime = DateTime.Now, IsDeleted = false, Name = TACHYONConsts.OthersDisplayName.ToLower(), HasAmount = false, HasCount = true };

            if (!_dbContext.Vases.Any(OthersExpressions.ContainsOthersNameExpression))
            {
                _dbContext.Vases.Add(otherVas);
            }


            var otherTransportType = new TransportType()
            {
                CreationTime = DateTime.Now,
                DisplayName = TACHYONConsts.OthersDisplayName,
                IsDeleted = false
            };


            if (!_dbContext.TransportTypes.Any(OthersExpressions.ContainsOthersDisplayNameExpression))
            {
                _dbContext.TransportTypes.Add(otherTransportType);
            }




            var otherTruckType = new TrucksType()
            {
                CreationTime = DateTime.Now,
                IsDeleted = true,
                IsActive = true,
                DisplayName = TACHYONConsts.OthersDisplayName
            };

            if (!_dbContext.TrucksTypes.Any(OthersExpressions.ContainsOthersDisplayNameExpression))
            {
                _dbContext.TrucksTypes.Add(otherTruckType);
            }



            var otherReasonAccident = new ShippingRequestReasonAccident()
            {
                CreationTime = DateTime.Now,
                Key = TACHYONConsts.OthersDisplayName,
                IsDeleted = false
            };

            if (!_dbContext.ShippingRequestReasonAccidents.Any(OthersExpressions.ContainsOthersKeyExpression))
            {
                _dbContext.ShippingRequestReasonAccidents.Add(otherReasonAccident);
            }




            _dbContext.SaveChanges();
        }
    }


}