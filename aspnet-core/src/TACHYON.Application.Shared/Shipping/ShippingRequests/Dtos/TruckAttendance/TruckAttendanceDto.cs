using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Shipping.Dedicated;

namespace TACHYON.Shipping.ShippingRequests.Dtos.TruckAttendance
{
    public class TruckAttendanceDto: EntityDto<long>
    {
        public DateTime AttendanceDate { get; set; }
        public AttendaceStatus AttendaceStatus { get; set; }
        public string AttendanceStatusTitle { get; set; }
    }
}
