using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.DriverLocationLogs.dtos
{
    public class DriverLocationLogDto : EntityDto<long>
    {
        public int? TripId { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}