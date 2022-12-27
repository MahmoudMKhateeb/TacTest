using Abp.Application.Services.Dto;

namespace TACHYON.PricePackages.Dto.TmsPricePackages
{
    public class TmsPricePackageSelectItemDto : EntityDto
    {
        public string TruckType { get; set; }

        public string DisplayName { get; set; }

        public decimal DirectRequestTotalPrice { get; set; }

        public decimal TachyonManageTotalPrice { get; set; }

        public string OriginCity { get; set; }
        
        public string DestinationCity { get; set; }
    }
}