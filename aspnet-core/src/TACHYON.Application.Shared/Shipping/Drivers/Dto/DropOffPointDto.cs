using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Drivers.Dto
{
    public class DropOffPointDto : EntityDto<long>
    {
        public string Code { get; set; }
    }
}