using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Cities.Dtos
{
    public class GetCityForEditOutput
    {
		public CreateOrEditCityDto City { get; set; }

		public string CountyDisplayName { get; set;}


    }
}