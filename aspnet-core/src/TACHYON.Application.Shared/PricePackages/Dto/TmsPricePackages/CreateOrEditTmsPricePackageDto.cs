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
        
        public PricePackageCommissionType CommissionType { get; set; }
        
        public decimal Price { get; set; }
        
        public decimal Commission { get; set; }

        [JsonIgnore]
        public decimal TotalPrice
        {
            get
            {
                switch (CommissionType)
                {
                    case PricePackageCommissionType.Value:
                        return Price + Commission;
                    case PricePackageCommissionType.Percentage:
                        return Price + (Commission > 0 ? (Commission*Price)/100 : Price);
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        public PricePackageType Type { get; set; }
    }
}