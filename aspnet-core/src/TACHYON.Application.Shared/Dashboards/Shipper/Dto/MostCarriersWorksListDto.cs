using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;

namespace TACHYON.Dashboards.Shipper.Dto
{
    public class MostCarriersWorksListDto
    {
        public int? Id { get; set; }
        public string CarrierName { get; set; }
        public int NumberOfTrips { get; set; }
        public decimal? CarrierRating { get; set; }
        public int Count { get; set; }
    }


}