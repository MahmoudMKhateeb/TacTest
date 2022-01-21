using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using TACHYON.Dashboards.Shipper.Dto;
using TACHYON.Shipping.ShippingRequestStatuses;

namespace TACHYON.Dashboards.Shipper.Dto
{
    public class CompletedTripVsPodListDto
    {
        public List<RequestsListPerMonthDto> CompletedTrips { get; set; }
        public List<RequestsListPerMonthDto> PODTrips { get; set; }
    }
}