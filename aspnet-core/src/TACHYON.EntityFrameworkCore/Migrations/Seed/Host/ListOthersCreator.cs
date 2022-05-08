using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Extensions;
using Abp.Linq.Expressions;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using TACHYON.Common;
using TACHYON.EntityFrameworkCore;
using TACHYON.Extension;
using TACHYON.Goods.GoodCategories;
using TACHYON.Shipping.Accidents;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations;
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
            _dbContext.UnitOfMeasures.SeedEntity();
            _dbContext.GoodCategories.SeedEntity();
            _dbContext.Vases.SeedEntity();
            _dbContext.TransportTypes.SeedEntity();
            _dbContext.ShippingRequestReasonAccidents.SeedEntity();

            #region TruckTypeSeeding

            if (!_dbContext.TrucksTypes.Any(OthersExpressions.ContainsOthersKeyExpression))
            {
                var otherTruckType = new TrucksType()
                {
                    CreationTime = DateTime.Now,
                    IsDeleted = true,
                    IsActive = true,
                    Key = TACHYONConsts.OthersDisplayName,
                    DisplayName = TACHYONConsts.OthersDisplayName
                };

                _dbContext.TrucksTypes.Add(otherTruckType);
            }

            #endregion

            _dbContext.SaveChanges();

            // TODO: Add Check Translation For UnitOfMeasure When it Has Translation

            _dbContext.GoodCategories.CheckTranslation(_dbContext.GoodCategoryTranslations, 1);
            _dbContext.Vases.CheckTranslation(_dbContext.VasTranslations, 1);
            _dbContext.TransportTypes.CheckTranslation(_dbContext.TransportTypesTranslations, 1);
            _dbContext.ShippingRequestReasonAccidents.CheckTranslation(
                _dbContext.ShippingRequestReasonAccidentTranslations, 1);
            _dbContext.TrucksTypes.CheckTranslation(_dbContext.TrucksTypesTranslations, 1L);

            _dbContext.SaveChanges();
        }
    }
}