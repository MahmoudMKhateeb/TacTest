﻿using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trailers.TrailerTypes;
using TACHYON.Goods.GoodCategories;
using TACHYON.Routs;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Offers
{
	[Table("Offers")]
    public class Offer : FullAuditedEntity , IMustHaveTenant
    {
			public int TenantId { get; set; }
			

		[StringLength(OfferConsts.MaxDisplayNameLength, MinimumLength = OfferConsts.MinDisplayNameLength)]
		public virtual string DisplayName { get; set; }
		
		[StringLength(OfferConsts.MaxDescriptionLength, MinimumLength = OfferConsts.MinDescriptionLength)]
		public virtual string Description { get; set; }
		
		public virtual decimal Price { get; set; }
		

		public virtual Guid TrucksTypeId { get; set; }
		
        [ForeignKey("TrucksTypeId")]
		public TrucksType TrucksTypeFk { get; set; }
		
		public virtual int TrailerTypeId { get; set; }
		
        [ForeignKey("TrailerTypeId")]
		public TrailerType TrailerTypeFk { get; set; }
		
		public virtual int? GoodCategoryId { get; set; }
		
        [ForeignKey("GoodCategoryId")]
		public GoodCategory GoodCategoryFk { get; set; }
		
		public virtual int RouteId { get; set; }
		
        [ForeignKey("RouteId")]
		public Route RouteFk { get; set; }
		
    }
}