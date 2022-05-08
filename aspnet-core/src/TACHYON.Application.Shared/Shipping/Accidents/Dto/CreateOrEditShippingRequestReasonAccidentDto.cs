using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Shipping.Accidents.Dto
{
    public class CreateOrEditShippingRequestReasonAccidentDto : EntityDto
    {
        public ICollection<ShippingRequestReasonAccidentTranslationDto> Translations { get; set; }
    }
}