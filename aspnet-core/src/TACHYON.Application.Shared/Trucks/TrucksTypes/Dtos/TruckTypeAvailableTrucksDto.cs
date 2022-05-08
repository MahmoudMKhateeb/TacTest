using Abp.Application.Services.Dto;

namespace TACHYON.Trucks.TrucksTypes.Dtos
{
    public class TruckTypeAvailableTrucksDto : EntityDto<long?>
    {
        public string TruckType { get; set; }

        public int AvailableTrucksCount { get; set; }
    }
}