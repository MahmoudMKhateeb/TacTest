using System.Collections.Generic;
using TACHYON.Cities.Dtos;
using TACHYON.Trucks.TrucksTypes.Dtos;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class GetShippingRequestSearchListDto
    {
        public List<CityDto> Cities { get; set; }
        public List<TrucksTypeDto> TrucksTypes { get; set; }
    }
}