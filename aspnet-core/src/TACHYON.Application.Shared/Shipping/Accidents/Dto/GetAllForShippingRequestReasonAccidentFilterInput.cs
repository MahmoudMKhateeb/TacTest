using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Accidents.Dto
{
    public class GetAllForShippingRequestReasonAccidentFilterInput 
    {
        public string Filter { get; set; }
        public string Sorting { get; set; }


    }
}
