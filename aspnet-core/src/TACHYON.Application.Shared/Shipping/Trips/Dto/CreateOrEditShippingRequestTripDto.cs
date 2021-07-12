using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.ShippingRequestTripVases.Dtos;

namespace TACHYON.Shipping.Trips.Dto
{
    public class CreateOrEditShippingRequestTripDto : EntityDto<int?>, ICustomValidate
    {
        [Required]
        public DateTime StartTripDate { get; set; }

        public DateTime? EndTripDate { get; set; }


        //public long? AssignedDriverUserId { get; set; }
        //public long? AssignedTruckId { get; set; }
        public long ShippingRequestId { get; set; }

        public bool HasAttachment { get; set; }
        public bool NeedsDeliveryNote { get; set; }
        //Facility
        public virtual long? OriginFacilityId { get; set; }

        public virtual long? DestinationFacilityId { get; set; }
        public string TotalValue { get; set; }

        [StringLength(ShippingRequestTripConsts.MaxTripNoteLength,
            MinimumLength = ShippingRequestTripConsts.MinTripNoteLength)]
        public string TripNote { get; set; }
        public List<CreateOrEditRoutPointDto> RoutPoints { get; set; }
        public List<CreateOrEditShippingRequestTripVasDto> ShippingRequestTripVases { get; set; }

        public CreateOrEditDocumentFileDto CreateOrEditDocumentFileDto { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (EndTripDate!=null && StartTripDate.Date > EndTripDate.Value.Date)
            {
                context.Results.Add(new ValidationResult("The start date must be or equal to end date."));
            }

            //document 
            if (HasAttachment && CreateOrEditDocumentFileDto.UpdateDocumentFileInput.FileToken.IsNullOrEmpty())
            {
                context.Results.Add(new ValidationResult("document missing: " + CreateOrEditDocumentFileDto.Name));
            }
        }
    }
}
