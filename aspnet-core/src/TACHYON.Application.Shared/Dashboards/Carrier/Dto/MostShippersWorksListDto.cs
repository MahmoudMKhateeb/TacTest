using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;

namespace TACHYON.Dashboards.Carrier.Dto
{
    public class MostShippersWorksListDto
    {
        public int? Id { get; set; }
        public string ShipperName { get; set; }
        public int NumberOfTrips { get; set; }
        public decimal ShipperRating { get; set; }
        public int Count { get; set; }
    }


}