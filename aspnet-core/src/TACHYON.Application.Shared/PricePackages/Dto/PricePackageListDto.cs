using Abp.Application.Services.Dto;
using System.Collections.Generic;
using TACHYON.ServiceAreas;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PricePackages.Dto
{
    public class PricePackageListDto : EntityDto<long>
    {
        public string OriginCity { get; set; }

        public string DestinationCity { get; set; }

        public string Company { get; set; }

        public ShippingRequestRouteType? RouteType { get; set; }

        public string DisplayName { get; set; }

        public string TruckType { get; set; }

        public decimal TotalPrice { get; set; }
        
        public string EditionName { get; set; }

        public string Status { get; set; }

        public string ProposalName { get; set; }

        public string Appendix { get; set; }

        public int? ProposalId { get; set; }

        public bool HasProposal { get; set; }

        public int TransportTypeId { get; set; }

        public long TruckTypeId { get; set; }

        public int OriginCityId { get; set; }

        public int DestinationCityId { get; set; }

        public int? TenantId { get; set; }

        public bool HasAppendix { get; set; }

        public decimal? DirectRequestPrice { get; set; }

        public PricePackageType Type { get; set; }

        public int? OriginCountryId { get; set; }

        public List<int> ServiceAreas { get; set; }

        public PricePackageUsageType UsageType { get; set; }

        public RoundTripType? RoundTrip { get; set; }
        
        public ShippingTypeEnum ShippingTypeId { get; set; }
        
        public long? OriginFacilityPortId { get; set; }
        
        public long? DestinationFacilityPortId { get; set; }
    }
}