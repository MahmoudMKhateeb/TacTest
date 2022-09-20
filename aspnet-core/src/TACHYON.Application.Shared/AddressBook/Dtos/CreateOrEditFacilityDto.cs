using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TACHYON.AddressBook.Dtos
{
    public class CreateOrEditFacilityDto : EntityDto<long?>
    {
        [Required]
        [StringLength(FacilityConsts.MaxNameLength, MinimumLength = FacilityConsts.MinNameLength)]
        public string Name { get; set; }


        [Required]
        [StringLength(FacilityConsts.MaxAdressLength, MinimumLength = FacilityConsts.MinAdressLength)]
        public string Address { get; set; }


        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public int CityId { get; set; }
        public int? ShipperId { get; set; }
        [Required]
        public List<CreateOrEditFacilityWorkingHourDto> FacilityWorkingHours { get; set; }
    }
}