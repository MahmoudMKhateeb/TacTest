using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.TachyonPriceOffers.dtos
{
    public class GetAllTachyonPriceOfferInput : PagedAndSortedResultRequestDto
    {
        public long? ShippingRequestId { get; set; }
    }
}