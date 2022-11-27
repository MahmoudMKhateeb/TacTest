using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Integration.BayanIntegration.Dtos
{
    public class CreateOrEditBayanIntegrationResultDto : EntityDto<long?>
    {

        [Required]
        public string ActionName { get; set; }

        [Required]
        public string InputJson { get; set; }

        public string ResponseJson { get; set; }

        public string Version { get; set; }

        public int ShippingRequestTripId { get; set; }

    }
}