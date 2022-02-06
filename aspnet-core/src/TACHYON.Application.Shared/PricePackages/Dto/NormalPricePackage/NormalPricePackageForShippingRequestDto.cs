using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.PricePackages.Dto.NormalPricePackage
{
    public class NormalPricePackageForShippingRequestDto
    {
        public int Id { get; set; }
        public string CarrierName { get; set; }
        public decimal CarrierRate { get; set; }
        public string PricePackageId { get; set; }
        public string DisplayName { get; set; }
        public string TruckType { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
    }
}