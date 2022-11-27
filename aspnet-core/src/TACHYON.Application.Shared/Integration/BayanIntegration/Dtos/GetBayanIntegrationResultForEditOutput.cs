using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Integration.BayanIntegration.Dtos
{
    public class GetBayanIntegrationResultForEditOutput
    {
        public CreateOrEditBayanIntegrationResultDto BayanIntegrationResult { get; set; }

        public string ShippingRequestTripContainerNumber { get; set; }

    }
}