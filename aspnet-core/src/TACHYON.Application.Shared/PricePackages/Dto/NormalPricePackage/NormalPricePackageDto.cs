using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.PricePackages.Dto.NormalPricePackage
{
    public class NormalPricePackageDto
    {
        public string PricePackageId { get; set; }
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string TruckType { get; set; }
        public decimal DirectRequestPrice { get; set; }
        public decimal MarcketPlaceRequestPrice { get; set; }
        public decimal TachyonMSRequestPrice { get; set; }
        public string PricePerExtraDrop { get; set; }
        public bool IsMultiDrop { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
    }
}