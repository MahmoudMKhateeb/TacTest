using System.Collections.Generic;
using TACHYON.PriceOffers.Dto;

namespace TACHYON.PricePackages.Dto
{
    public class PricePackageForPriceCalculationDto
    {
        public PriceOfferDto OfferDto { get; set; }

        public int NumberOfTrips { get; set; }
        
        public int NumberOfDrops { get; set; }
        
        public string DisplayName { get; set; }
        
        public string PricePackageReference { get; set; }
        
        public string TruckType { get; set; }
        
        public int OriginCityId { get; set; }
        
        public string Origin { get; set; }
        
        public string Destination { get; set; }
        
        public int DestinationCityId { get; set; }
        
        public int TransportTypeId { get; set; }
        
        public long TrucksTypeId { get; set; }
        
        public bool IsMultiDrop { get; set; }
        
        public bool HasDirectRequest { get; set; }
        
        public decimal SingleDropPrice { get; set; } // todo find from where this property got a value
        
        public decimal? PricePerAdditionalDrop { get; set; }
        
        public long PricePackageId { get; set; }
        
        public int TenantId { get; set; }
    }
}