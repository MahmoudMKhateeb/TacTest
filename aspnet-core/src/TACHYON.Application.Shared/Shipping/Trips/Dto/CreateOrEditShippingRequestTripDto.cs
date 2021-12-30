using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.Runtime.Validation;
using Abp.UI;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.ShippingRequestTripVases.Dtos;

namespace TACHYON.Shipping.Trips.Dto
{
    public class CreateOrEditShippingRequestTripDto : EntityDto<int?>, ICustomValidate, IShouldNormalize
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

        [StringLength(ShippingRequestTripConsts.MaxNoteLength)]
        public string Note { get; set; }
        public List<CreateOrEditRoutPointDto> RoutPoints { get; set; }
        public List<CreateOrEditShippingRequestTripVasDto> ShippingRequestTripVases { get; set; }

        public CreateOrEditDocumentFileDto CreateOrEditDocumentFileDto { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            //document validation
            if (HasAttachment && CreateOrEditDocumentFileDto?.UpdateDocumentFileInput?.FileToken == null)
                context.Results.Add(new ValidationResult("document missing: " + CreateOrEditDocumentFileDto?.Name));


            if (!OriginFacilityId.HasValue)
                context.Results.Add(new ValidationResult("You Must Select Origin Facility"));
            if (!DestinationFacilityId.HasValue)
                context.Results.Add(new ValidationResult("You Must Select Destination Facility"));

            if (EndTripDate != null && StartTripDate.Date > EndTripDate.Value.Date)
            {
                context.Results.Add(new ValidationResult("The start date must be or equal to end date."));
            }



            var dropPoints = RoutPoints.Where(x => x.PickingType == PickingType.Dropoff);
            foreach (var drop in dropPoints)
            {
                if (drop.ReceiverId == null &&
                    (string.IsNullOrWhiteSpace(drop.ReceiverFullName) ||
                    string.IsNullOrWhiteSpace(drop.ReceiverPhoneNumber)))
                {
                    //throw new UserFriendlyException(L("YouMustEnterReceiver"));
                    throw new UserFriendlyException("YouMustEnterReceiver");
                }
            }
        }

        public void Normalize()
        {
            //document 
            if (!HasAttachment)
            {
                CreateOrEditDocumentFileDto = null;
            }

        }
    }
}