using System.Collections.Generic;
using TACHYON.Cities.Dtos;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.Packing.PackingTypes.Dtos;
using TACHYON.Trucks.TruckCategories.TransportTypes.Dtos;
using TACHYON.Trucks.TruckCategories.TruckCapacities.Dtos;
using TACHYON.Trucks.TrucksTypes.Dtos;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class GetShippingRequestSearchListDto
    {
        public List<CityDto> Cities { get; set; }
        public List<TrucksTypeDto> TrucksTypes { get; set; }
        public List<TransportTypeDto> TransportTypes { get; set; }
        public List<CapacityDto> Capacities { get; set; }
        public List<GoodCategoryDto> GoodsCategories { get; set; }
        public List<PackingTypeDto> PackingTypes { get; set; }
    }
}