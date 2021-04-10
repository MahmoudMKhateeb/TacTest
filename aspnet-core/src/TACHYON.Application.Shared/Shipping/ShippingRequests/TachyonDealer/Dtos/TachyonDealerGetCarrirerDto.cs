using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.TachyonDealer.Dtos
{
    public class TachyonDealerGetCarrirerDto : EntityDto
    {
        public string Name { get; set; }
        public bool IsRequestSent { get; set; }
    }
}
