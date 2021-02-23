using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Vases.Dtos
{
    public class CreateOrEditShippingRequestVasListDto
    {
        public long? Id { get; set; }
        public int RequestMaxAmount { get; set; }

        public int RequestMaxCount { get; set; }
        public int VasId { get; set; }

        public long? ShippingRequestId { get; set; }
    }
}
