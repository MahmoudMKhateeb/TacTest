using Abp.Application.Services.Dto;

namespace TACHYON.AddressBook.Dtos
{
    public class FacilityLocationListDto : EntityDto<long>
    {
        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public string Name { get; set; }

        public string CityName { get; set; }

        public string CountryName { get; set; }
    }
}