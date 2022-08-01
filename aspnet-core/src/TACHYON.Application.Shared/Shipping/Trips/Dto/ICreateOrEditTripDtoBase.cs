using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TACHYON.Routs.RoutPoints;

namespace TACHYON.Shipping.Trips.Dto
{
    public interface ICreateOrEditTripDtoBase 
    {
        long ShippingRequestId { get; set; }
        [Required]  DateTime? StartTripDate { get; set; }

         DateTime? EndTripDate { get; set; }

         bool HasAttachment { get; set; }

         bool NeedsDeliveryNote { get; set; }

        [StringLength(ShippingRequestTripConsts.MaxNoteLength)]
         string Note { get; set; }

         string TotalValue { get; set; }


    }
}
