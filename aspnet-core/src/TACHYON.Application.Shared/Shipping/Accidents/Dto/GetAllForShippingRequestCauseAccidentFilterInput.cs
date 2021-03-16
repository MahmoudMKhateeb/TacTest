using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Accidents.Dto
{
    public class GetAllForShippingRequestCauseAccidentFilterInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

    }
}
