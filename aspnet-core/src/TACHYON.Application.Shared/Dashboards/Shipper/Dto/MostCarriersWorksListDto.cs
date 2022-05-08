using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;

namespace TACHYON.Dashboards.Shipper.Dto
{
    public class MostTenantWorksListDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int NumberOfTrips { get; set; }
        public decimal? Rating { get; set; }
        public int Count { get; set; }
    }


}