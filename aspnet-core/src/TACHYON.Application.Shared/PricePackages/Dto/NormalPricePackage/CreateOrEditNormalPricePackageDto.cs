using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.PricePackages.Dto.NormalPricePackage
{
    public class CreateOrEditNormalPricePackageDto : EntityDto<int?>
    {
        [Required]
        [StringLength(PricePackagesConst.MaxDisplayNameLength, MinimumLength = PricePackagesConst.MinDisplayNameLength)] public string DisplayName { get; set; }
        public int TransportTypeId { get; set; }
        public long TrucksTypeId { get; set; }
        [Range(PricePackagesConst.MinPriceNumber, PricePackagesConst.MaxPriceNumber)]
        public decimal DirectRequestPrice { get; set; }
        [Range(PricePackagesConst.MinPriceNumber, PricePackagesConst.MaxPriceNumber)]
        public decimal MarcketPlaceRequestPrice { get; set; }
        [Range(PricePackagesConst.MinPriceNumber, PricePackagesConst.MaxPriceNumber)]
        public decimal TachyonMSRequestPrice { get; set; }
        public decimal? PricePerExtraDrop { get; set; }
        public bool IsMultiDrop { get; set; }
        [Required]
        public int OriginCityId { get; set; }
        [Required]
        public int DestinationCityId { get; set; }
    }
}