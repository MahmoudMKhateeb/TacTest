using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Trips.Dto
{
    public class RemarksInputDto : EntityDto<int>
    {
        public bool CanBePrinted { get; set; }
        public string RoundTrip { get; set; }
        public string ContainerNumber { get; set; }
    }
}