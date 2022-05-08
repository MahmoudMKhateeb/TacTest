using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.AddressBook.Dtos
{
    public class GetFacilityForEditOutput
    {
        public CreateOrEditFacilityDto Facility { get; set; }

        public string CityDisplayName { get; set; }
        public string CountryDisplayName { get; set; }
        public int? CountryId { get; set; }
        public string CountryCode { get; set; }
    }
}