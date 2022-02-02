
using Abp.Application.Services.Dto;
using System;

namespace TACHYON.DriverLicenseTypes.Dtos
{
    public class DriverLicenseTypeDto : EntityDto
    {
        public string Name { get; set; }

        public int WasIIntegrationId { get; set; }

        public bool ApplicableforWaslRegistration { get; set; }



    }
}