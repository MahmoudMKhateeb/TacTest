using System;
using Abp.Application.Services.Dto;
using TACHYON.Vases.Dtos;

namespace TACHYON.ShippingRequestVases.Dtos
{
    public class ShippingRequestVasDto : EntityDto<long?>
    {
        public int RequestMaxAmount { get; set; }
        public int RequestMaxCount { get; set; }
        public long? ShippingRequestId { get; set; }
        public string OtherVasName { get; set; }
        public VasDto Vas { get; set; }
    }
}