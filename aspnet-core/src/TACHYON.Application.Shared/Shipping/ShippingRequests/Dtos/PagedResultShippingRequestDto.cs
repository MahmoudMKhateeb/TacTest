using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
   public class PagedResultShippingRequestDto<T> : PagedResultDto<T>
    {
        public int NoOfPostPriceWithoutTrips { get; set; }
        public PagedResultShippingRequestDto(int totalCount, IReadOnlyList<T> items,int NoOfPostPriceWithoutTrips) : base(totalCount, items)
        {
            this.NoOfPostPriceWithoutTrips = NoOfPostPriceWithoutTrips;
        }

    }
}
