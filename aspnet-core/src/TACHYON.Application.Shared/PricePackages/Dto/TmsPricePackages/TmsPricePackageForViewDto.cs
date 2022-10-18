using Abp.Application.Services.Dto;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PricePackages.Dto.TmsPricePackages
{
    public class TmsPricePackageForViewDto : EntityDto
    {
        public string OriginCity { get; set; }

        public string DestinationCity { get; set; }

        public string Shipper { get; set; }

        public ShippingRequestRouteType RouteType { get; set; }

        public decimal DirectRequestPrice { get; set; }

        public decimal TachyonManagePrice { get; set; }

        public decimal DirectRequestCommission { get; set; }
        
        public decimal TachyonManageCommission { get; set; }

        public decimal DirectRequestTotalPrice { get; set; }
        
        public decimal TachyonManageTotalPrice { get; set; }

        public PricePackageType Type { get; set; }

        public string PricePackageId { get; set; }

        public string DisplayName { get; set; }

        public string TransportType { get; set; }

        public string TrucksType { get; set; }
    }
}