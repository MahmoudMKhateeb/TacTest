using Abp.Application.Services.Dto;

namespace TACHYON.PricePackages.Dto.TmsPricePackages
{
    public class PricePackageSelectItemDto : EntityDto
    {
        public string TruckType { get; set; }

        public string DisplayName { get; set; }

        public decimal TotalPrice { get; set; }
        public string OriginCity { get; set; }
        
        public string DestinationCity { get; set; }

        public string PricePackageId { get; set; }
    }
}