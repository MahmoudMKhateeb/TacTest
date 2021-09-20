using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Rating.dtos
{
    public class CreateDriverAndDERatingByReceiverDto
    {
        public CreateDriverRatingByReceiverDto CreateDriverRatingByReceiverInput { get; set; }
        public CreateDeliveryExpRateByReceiverDto CreateDeliveryExpRateByReceiverInput { get; set; }
    }
}
