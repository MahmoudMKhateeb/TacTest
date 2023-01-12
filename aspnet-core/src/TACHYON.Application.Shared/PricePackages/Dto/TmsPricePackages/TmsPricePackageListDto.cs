using Abp.Application.Services.Dto;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PricePackages.Dto.TmsPricePackages
{
    public class TmsPricePackageListDto : EntityDto
    {
        public string OriginCity { get; set; }

        public string DestinationCity { get; set; }

        public string Shipper { get; set; }

        public string RouteType { get; set; }

        public string DisplayName { get; set; }

        public string TruckType { get; set; }

        public decimal TotalPrice { get; set; }
        
        public string EditionName { get; set; }

        public string Status { get; set; }

        public string ProposalName { get; set; }

        public string Appendix { get; set; }

        public int? ProposalId { get; set; }

        public bool HasProposal { get; set; }
    }
}