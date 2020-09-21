
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.AddressBook.Dtos
{
    public class CreateOrEditFacilityDto : EntityDto<long?>
    {

        [Required]
        [StringLength(FacilityConsts.MaxNameLength, MinimumLength = FacilityConsts.MinNameLength)]
        public string Name { get; set; }


        [Required]
        [StringLength(FacilityConsts.MaxAdressLength, MinimumLength = FacilityConsts.MinAdressLength)]
        public string Adress { get; set; }


        public decimal Longitude { get; set; }


        public decimal Latitude { get; set; }



        public int CityId { get; set; }


    }
}