using System;
using Abp.Application.Services.Dto;

namespace TACHYON.ShippingRequestVases.Dtos
{
    public class ShippingRequestVasDto : EntityDto<long>
    {

        public int VasId { get; set; }

    }
}