using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.ShippingRequestTripVases.Dtos;

namespace TACHYON.Shipping.Trips.Dto
{
    public class CreateOrEditShippingRequestTripDto : EntityDto<int?>, ICustomValidate
    {
        [Required]
        public DateTime StartTripDate { get; set; }
        [Required]
        public DateTime EndTripDate { get; set; }


        //public long? AssignedDriverUserId { get; set; }
        //public long? AssignedTruckId { get; set; }
        public long ShippingRequestId { get; set; }


        //Facility
        public virtual long? OriginFacilityId { get; set; }

        public virtual long? DestinationFacilityId { get; set; }

        public List<CreateOrEditRoutPointDto> RoutPoints { get; set; }
        public List<CreateOrEditShippingRequestTripVasDto> ShippingRequestTripVases { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (StartTripDate.Date> EndTripDate.Date)
            {
                context.Results.Add(new ValidationResult("The start date must be or equal to end date."));
            }
        }
    }
}
