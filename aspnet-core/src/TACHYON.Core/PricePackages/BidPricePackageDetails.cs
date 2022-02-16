using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.PriceOffers.Base;
using TACHYON.Vases;

namespace TACHYON.PricePackages
{
    public class BidPricePackageDetails : PriceOfferBase
    {
        public long BidNormalPricePackageId { get; set; }
        [ForeignKey(nameof(BidNormalPricePackageId))]
        public BidNormalPricePackage BidNormalPricePackageFK { get; set; }


    }
}