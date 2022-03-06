using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using TACHYON.Tenants.Dashboard.Dto;

namespace TACHYON.Dashboards.Host.Dto
{
    public class GetDataByDateFilterInput
    {
        public SalesSummaryDatePeriod SalesSummaryDatePeriod { get; set; }

    }

}