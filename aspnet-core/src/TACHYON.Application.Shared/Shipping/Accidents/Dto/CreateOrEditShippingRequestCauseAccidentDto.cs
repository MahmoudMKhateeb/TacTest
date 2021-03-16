using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Shipping.Accidents.Dto
{
 public   class CreateOrEditShippingRequestCauseAccidentDto:EntityDto
    {
        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string DisplayName { get; set; }
    }
}
