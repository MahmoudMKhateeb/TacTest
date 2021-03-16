using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
  public  class GetAllShippingRequestsOutput
    {
        public PagedResultDto<GetShippingRequestForViewOutput> Data { get; set; }
        public int NoOfPostPriceWithoutTrips { get; set; }
    }
}
