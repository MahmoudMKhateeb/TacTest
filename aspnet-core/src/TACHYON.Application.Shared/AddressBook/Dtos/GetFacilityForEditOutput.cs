using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.AddressBook.Dtos
{
    public class GetFacilityForEditOutput
    {
		public CreateOrEditFacilityDto Facility { get; set; }

		public string CityDisplayName { get; set;}


    }
}