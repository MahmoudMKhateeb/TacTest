using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.PriceOffers.Base;
using TACHYON.Vases;

namespace TACHYON.PricePackages
{
    public class PricePackageOfferItem : PriceOfferBase
    {
        public long BidNormalPricePackageId { get; set; }
        [ForeignKey(nameof(BidNormalPricePackageId))]
        public PricePackageOffer BidNormalPricePackageFK { get; set; }


    }
}