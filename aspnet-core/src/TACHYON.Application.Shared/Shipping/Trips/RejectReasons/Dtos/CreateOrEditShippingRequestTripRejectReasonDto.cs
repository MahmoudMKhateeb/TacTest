using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Shipping.Trips.RejectReasons.Dtos
{
   public class CreateOrEditShippingRequestTripRejectReasonDto: EntityDto
    {
        public ICollection<ShippingRequestTripRejectReasonTranslationDto> Translations { get; set; }

    }
}
