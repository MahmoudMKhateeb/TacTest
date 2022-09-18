using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Integration.BayanIntegration.Dtos
{
    public class BayanIntegrationResultDto : EntityDto<long>
    {
        public string ActionName { get; set; }

        public string InputJson { get; set; }

        public string ResponseJson { get; set; }

        public string Version { get; set; }

        public int ShippingRequestTripId { get; set; }

    }
}