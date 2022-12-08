using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PricePackages.Dto.TmsPricePackages
{
    public class CreateOrEditTmsPricePackageDto : EntityDto<int?>
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

        public decimal TotalPrice { get; set; }

        public PricePackageType Type { get; set; }
    }
}