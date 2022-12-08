using Abp.Application.Services.Dto;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PricePackages.Dto.TmsPricePackages
{
    public class TmsPricePackageForViewDto : EntityDto
    {
        public string OriginCity { get; set; }

        public string DestinationCity { get; set; }

        public string CompanyName { get; set; }

        public int CompanyTenantId { get; set; }

        public ShippingRequestRouteType RouteType { get; set; }
        public decimal FinalPrice { get; set; }

        public string PricePackageId { get; set; }

        public string DisplayName { get; set; }

        public string TransportType { get; set; }

        public string TruckType { get; set; }
    }
}