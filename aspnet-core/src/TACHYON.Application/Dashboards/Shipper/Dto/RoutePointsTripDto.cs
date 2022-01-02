using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.PickingTypes;

namespace TACHYON.Dashboards.Shipper.Dto
{
    public class RoutePointsTripDto : EntityDto<long>
    {
        public long? WaybillNumber { get; set; }
        public string PickingType { get; set; }
        public string Facility { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}