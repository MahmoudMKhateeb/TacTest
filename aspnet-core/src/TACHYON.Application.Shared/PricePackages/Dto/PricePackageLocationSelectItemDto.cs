using Abp.Application.Services.Dto;
using TACHYON.AddressBook;

namespace TACHYON.PricePackages.Dto
{
    public class PricePackageLocationSelectItemDto: EntityDto<string>
    {
        public int? CityId { get; set; }

        public long? PortId { get; set; }

        public string DisplayName { get; set; }
        
        public PricePackageLocationType LocationType { get; set; }
    }

    public enum PricePackageLocationType
    {
        City = 1,
        Port = 2
    }
}