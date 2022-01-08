
using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.DriverLicenseTypes.Dtos
{
    public class CreateOrEditDriverLicenseTypeDto : EntityDto<int?>
    {

        [Required]
        [StringLength(DriverLicenseTypeConsts.MaxNameLength, MinimumLength = DriverLicenseTypeConsts.MinNameLength)]
        public string Name { get; set; }


        [Range(DriverLicenseTypeConsts.MinWasIIntegrationIdValue, DriverLicenseTypeConsts.MaxWasIIntegrationIdValue)]
        public int WasIIntegrationId { get; set; }


        public bool ApplicableforWaslRegistration { get; set; }



    }
}