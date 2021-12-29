using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;

namespace TACHYON.Dashboards.Host.Dto
{
    public class GetUnpricedRequestsInMarketplaceInput
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }

}