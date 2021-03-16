using System;
using Abp.Application.Services.Dto;

namespace TACHYON.ShippingRequestVases.Dtos
{
    public class ShippingRequestVasDto : EntityDto<long>
    {

        public long? Id { get; set; }
        public int RequestMaxAmount { get; set; }

        public int RequestMaxCount { get; set; }
        public int VasId { get; set; }

        public long? ShippingRequestId { get; set; }

    }
}