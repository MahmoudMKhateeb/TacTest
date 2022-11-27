using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Integration.BayanIntegration.Dtos
{
    public class GetAllBayanIntegrationResultsForExcelInput
    {
        public string Filter { get; set; }

        public string ActionNameFilter { get; set; }

        public string InputJsonFilter { get; set; }

        public string ResponseJsonFilter { get; set; }

        public string VersionFilter { get; set; }

        public string ShippingRequestTripContainerNumberFilter { get; set; }

    }
}