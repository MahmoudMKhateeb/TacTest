using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Cities.Dtos
{
    public class CreateOrEditCityDto : EntityDto<int?>
    {
        [Required]
        [StringLength(CityConsts.MaxDisplayNameLength, MinimumLength = CityConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }


        [StringLength(CityConsts.MaxCodeLength, MinimumLength = CityConsts.MinCodeLength)]
        public string Code { get; set; }


        //[StringLength(CityConsts.MaxLatitudeLength, MinimumLength = CityConsts.MinLatitudeLength)]
        public double Latitude { get; set; }


        //[StringLength(CityConsts.MaxLongitudeLength, MinimumLength = CityConsts.MinLongitudeLength)]
        public double Longitude { get; set; }

        public bool IsActive { get; set; } = true;

        public int CountyId { get; set; }
    }
}