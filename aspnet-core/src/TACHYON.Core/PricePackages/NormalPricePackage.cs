using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Cities;

namespace TACHYON.PricePackages
{
    public class NormalPricePackage : BasePricePackage
    {
        [Range(PricePackagesConst.MinPriceNumber, PricePackagesConst.MaxPriceNumber)]
        public decimal DirectRequestPrice { get; set; }
        [Range(PricePackagesConst.MinPriceNumber, PricePackagesConst.MaxPriceNumber)]
        public decimal MarcketPlaceRequestPrice { get; set; }
        [Range(PricePackagesConst.MinPriceNumber, PricePackagesConst.MaxPriceNumber)]
        public decimal TachyonMSRequestPrice { get; set; }
        public decimal? PricePerExtraDrop { get; set; }
        public bool IsMultiDrop { get; set; }
        public int? OriginCityId { get; set; }
        [ForeignKey(nameof(OriginCityId))]
        public City OriginCityFK { get; set; }

        public int? DestinationCityId { get; set; }
        [ForeignKey(nameof(DestinationCityId))]
        public City DestinationCityFK { get; set; }
        public ICollection<PricePackageOffer> BidNormalPricePackages { get; set; }

    }
}