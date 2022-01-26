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
        public float DirectRequestPrice { get; set; }
        [Range(PricePackagesConst.MinPriceNumber, PricePackagesConst.MaxPriceNumber)]
        public float MarcketPlaceRequestPrice { get; set; }
        [Range(PricePackagesConst.MinPriceNumber, PricePackagesConst.MaxPriceNumber)]
        public float TachyonMSRequestPrice { get; set; }
        public float? PricePerExtraDrop { get; set; }
        public bool IsMultiDrop { get; set; }
        [Required]
        public int OriginCityId { get; set; }
        [Required]
        public int DestinationCityId { get; set; }
    }
}