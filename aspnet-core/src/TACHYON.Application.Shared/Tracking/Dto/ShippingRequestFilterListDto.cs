using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Tracking.Dto
{
   public class ShippingRequestFilterListDto: EntityDto<long>
    {
        public string ReferenceNumber { get; set; }
    }
}
