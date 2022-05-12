using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;

namespace TACHYON.Dashboards.Shipper.Dto
{
    public class MostUsedOriginsDto
    {
        public string CityName { get; set; }
        public int NumberOfRequests { get; set; }
    }
}