using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PricePackages.Dto.TmsPricePackages
{
    public class CreateOrEditTmsPricePackageDto : EntityDto<int?>, IShouldNormalize
    {
        
        [Required]
        [StringLength(PricePackagesConst.MaxDisplayNameLength,
            MinimumLength = PricePackagesConst.MinDisplayNameLength)]
        public string DisplayName { get; set; }

        public int TransportTypeId { get; set; }

        public long TrucksTypeId { get; set; }

        public int OriginCityId { get; set; }

        public int DestinationCityId { get; set; }

        public int ShipperId { get; set; }

        public ShippingRequestRouteType RouteType { get; set; }

        public decimal DirectRequestPrice { get; set; }

        public decimal TachyonManagePrice { get; set; }
        
        public decimal DirectRequestCommission { get; set; }
        
        public decimal TachyonManageCommission { get; set; }

        public decimal? DirectRequestTotalPrice { get; set; }
        
        public decimal? TachyonManageTotalPrice { get; set; }

        public PricePackageType Type { get; set; }
        public void Normalize()
        {
            if (!DirectRequestTotalPrice.HasValue)
                DirectRequestTotalPrice = DirectRequestPrice + DirectRequestCommission;
            if (!TachyonManageTotalPrice.HasValue)
                TachyonManageTotalPrice = TachyonManagePrice + TachyonManageCommission;
        }
    }
}