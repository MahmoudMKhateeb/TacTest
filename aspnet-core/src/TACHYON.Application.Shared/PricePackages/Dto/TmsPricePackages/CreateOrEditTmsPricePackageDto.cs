﻿using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PricePackages.Dto.TmsPricePackages
{
    public class CreateOrEditTmsPricePackageDto : EntityDto<int?>, ICustomValidate
    {
        
        [Required]
        [StringLength(PricePackagesConst.MaxDisplayNameLength,
            MinimumLength = PricePackagesConst.MinDisplayNameLength)]
        public string DisplayName { get; set; }

        public int TransportTypeId { get; set; }

        public long TrucksTypeId { get; set; }

        public int OriginCityId { get; set; }

        public int DestinationCityId { get; set; }

        public int? DestinationTenantId { get; set; }

        public ShippingRequestRouteType RouteType { get; set; }
        
        public int ShippingTypeId { get; set; }

        public decimal TotalPrice { get; set; }

        public PricePackageType Type { get; set; }
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (TransportTypeId == default)
                context.Results.Add(new ValidationResult("You must select transport type"));
            if (TrucksTypeId == default)
                context.Results.Add(new ValidationResult("You must select truck type"));
            if (OriginCityId == default)
                context.Results.Add(new ValidationResult("You must select origin city"));
            if (DestinationCityId == default)
                context.Results.Add(new ValidationResult("You must select destination city"));
            if (RouteType == default)
                context.Results.Add(new ValidationResult("You must select route type"));
            if (Type == default)
                context.Results.Add(new ValidationResult("You must select price package type"));
        }
    }
}