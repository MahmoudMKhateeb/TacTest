using Abp.Application.Services.Dto;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PricePackages.Dto.TmsPricePackages
{
    public class TmsPricePackageListDto : EntityDto
    {
        public string OriginCity { get; set; }

        public string DestinationCity { get; set; }

        public string Shipper { get; set; }

        public PricePackageType Type { get; set; }

        public string DisplayName { get; set; }

        public string TruckType { get; set; }

        public string TransportType { get; set; }
        
    }
}