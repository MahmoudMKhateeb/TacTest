using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.DriverLicenseTypes.Dtos
{
    public class GetDriverLicenseTypeForEditOutput
    {
        public CreateOrEditDriverLicenseTypeDto DriverLicenseType { get; set; }


    }
}