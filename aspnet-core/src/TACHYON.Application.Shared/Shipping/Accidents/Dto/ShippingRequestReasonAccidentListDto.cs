using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Accidents.Dto
{
   public class ShippingRequestReasonAccidentListDto: FullAuditedEntityDto
    {
        public string Name { get; set; }
    }
}
