using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TACHYON.Shipping.Dedicated;

namespace TACHYON.Shipping.ShippingRequests.Dtos.TruckAttendance
{
    public class CreateOrEditTruckAttendanceDto :EntityDto<long?>
    {
        [Required]
        public long DedicatedShippingRequestTruckId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public AttendaceStatus AttendaceStatus { get; set; }
    }
}
