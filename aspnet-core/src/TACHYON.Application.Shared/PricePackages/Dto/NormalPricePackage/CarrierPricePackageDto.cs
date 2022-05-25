using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.PricePackages.Dto.NormalPricePackage
{
    public class CarrierPricePackageDto
    {
        public int PricePackageId { get; set; }
        public string PricePackageReferance { get; set; }
        public int CarrierTenantId { get; set; }
    }
}