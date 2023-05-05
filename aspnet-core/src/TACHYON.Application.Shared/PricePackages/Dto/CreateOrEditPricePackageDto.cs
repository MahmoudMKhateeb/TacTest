using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TACHYON.ServiceAreas;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PricePackages.Dto
{
    public class CreateOrEditPricePackageDto : EntityDto<long?>, ICustomValidate
    {
        [Required]
        [StringLength(PricePackagesConst.MaxDisplayNameLength,
            MinimumLength = PricePackagesConst.MinDisplayNameLength)]
        public string DisplayName { get; set; }

        public int TransportTypeId { get; set; }

        public long TruckTypeId { get; set; }

        public long? DestinationTenantId { get; set; }

        public ShippingRequestRouteType? RouteType { get; set; }

        public ShippingTypeEnum ShippingTypeId { get; set; }

        public decimal TotalPrice { get; set; }

        public PricePackageType Type { get; set; }

        public PricePackageUsageType? UsageType { get; set; }

        public decimal? DirectRequestPrice { get; set; }

        public decimal? PricePerAdditionalDrop { get; set; }

        public int? OriginCountryId { get; set; }

        public ICollection<CreateOrEditServiceAreaDto> ServiceAreas { get; set; }

        [StringLength(150, MinimumLength = 3)] public string ProjectName { get; set; }

        [StringLength(300, MinimumLength = 3)] public string ScopeOfWork { get; set; }

        public RoundTripType? RoundTrip { get; set; }

        public PricePackageLocationSelectItemDto OriginLocation { get; set; }
        
        public PricePackageLocationSelectItemDto DestinationLocation { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (TransportTypeId == default)
                context.Results.Add(new ValidationResult("You must select transport type"));
            if (TruckTypeId == default)
                context.Results.Add(new ValidationResult("You must select truck type"));
            if (OriginLocation?.CityId is null && Type != PricePackageType.Dedicated)
                context.Results.Add(new ValidationResult("You must select origin city"));
            if (DestinationLocation?.CityId is null && Type != PricePackageType.Dedicated)
                context.Results.Add(new ValidationResult("You must select destination city"));
            if (RouteType == default && Type == PricePackageType.PerTrip)
                context.Results.Add(new ValidationResult("You must select route type"));
            if (Type == default)
                context.Results.Add(new ValidationResult("You must select price package type"));
            if (ShippingTypeId == default)
                context.Results.Add(new ValidationResult("You must select price package type"));

            if (ShippingTypeId == ShippingTypeEnum.ImportPortMovements && OriginLocation?.PortId is null)
            {
                context.Results.Add(new ValidationResult("You Must select an origin port"));
            }

            if (Type != PricePackageType.Dedicated) return;

            if (OriginLocation != null || DestinationLocation != null)
                context.Results.Add(
                    new ValidationResult(
                        "You can not select origin or destination cities for dedicated price package"));
            
            if (!OriginCountryId.HasValue)
            {
                context.Results.Add(new ValidationResult("You must select origin country"));
            }

            if (ServiceAreas.IsNullOrEmpty())
            {
                context.Results.Add(new ValidationResult("You must select Service Areas"));
            }
        }
    }
}