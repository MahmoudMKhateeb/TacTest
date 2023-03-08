using Abp.Application.Services.Dto;

namespace TACHYON.PricePackages.Dto
{
     
    public class PricePackageForPricingDto : EntityDto<long>
    {
        public string OriginCity { get; set; }

        public string DestinationCity { get; set; }

        public string CompanyName { get; set; }

        public string CompanyEditionName { get; set; }

        public string RouteType { get; set; }
        
        public decimal TotalPrice { get; set; }

        public decimal Commission { get; set; }

        public decimal Price { get; set; }

        public string Type { get; set; }

        public string CommissionType { get; set; }

        public string DisplayName { get; set; }

        public string TransportType { get; set; }

        public string TruckType { get; set; }

        public string OfferStatusTitle { get; set; }

        public int? ProposalId { get; set; }

        public int? AppendixId { get; set; }

        public int? DestinationTenantId { get; set; }
        
    }
}