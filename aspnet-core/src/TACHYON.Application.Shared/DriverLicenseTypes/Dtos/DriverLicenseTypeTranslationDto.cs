using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.DriverLicenseTypes.Dtos
{
    public class DriverLicenseTypeTranslationDto : EntityDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Language { get; set; }

        public int CoreId { get; set; }
    }
}