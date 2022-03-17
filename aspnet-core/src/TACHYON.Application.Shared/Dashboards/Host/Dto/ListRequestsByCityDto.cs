using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;

namespace TACHYON.Dashboards.Host.Dto
{
    public class ListRequestsByCityDto
    {
        public int? Id { get; set; }
        public string CityName { get; set; }
        public int NumberOfRequests { get; set; }
        public int minimumValueOfRequests { get; set; }
        public int maximumValueOfRequests { get; set; }
    }

}