using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;

namespace TACHYON.Dashboards.Shipper.Dto
{
    public class RequestsListPerMonthDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Count { get; set; }
    }
}