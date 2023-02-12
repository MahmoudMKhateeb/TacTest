using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.Runtime.Validation;
using Abp.UI;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.ShippingRequestTripVases.Dtos;

namespace TACHYON.Shipping.Trips.Dto
{
    public class CreateOrEditShippingRequestTripDto : EntityDto<int?>, ICreateOrEditTripDtoBase, ICustomValidate, IShouldNormalize
    {
        [Required] public DateTime? StartTripDate { get; set; }

        public DateTime? EndTripDate { get; set; }


        public long ShippingRequestId { get; set; }

        public bool HasAttachment { get; set; }

        public bool NeedsDeliveryNote { get; set; }

        public DateTime? ExpectedDeliveryTime { get; set; }
        //Facility
        public virtual long? OriginFacilityId { get; set; }

        public virtual long? DestinationFacilityId { get; set; }
        public string TotalValue { get; set; }

        [StringLength(ShippingRequestTripConsts.MaxNoteLength)]
        public string Note { get; set; }
        public DateTime? SupposedPickupDateFrom { get; set; }
        public DateTime? SupposedPickupDateTo { get; set; }
        public string ContainerNumber { get; set; }
        public string SealNumber { get; set; }
        #region Dedicated
        public ShippingRequestRouteType? RouteType { get; set; }
        public int NumberOfDrops { get; set; }

       
        #endregion

        public List<CreateOrEditRoutPointDto> RoutPoints { get; set; }
        public List<CreateOrEditShippingRequestTripVasDto> ShippingRequestTripVases { get; set; }

        public CreateOrEditDocumentFileDto CreateOrEditDocumentFileDto { get; set; }

        [JsonIgnore]
        public int TenantId { get; set; }

        #region HomeDelivery
        public ShippingRequestTripFlag ShippingRequestTripFlag { get; set; }
        public long? TruckId { set; get; }
        public long? DriverUserId { set; get; }

        #endregion
        public void AddValidationErrors(CustomValidationContext context)
        {
            //document validation
            if (HasAttachment && CreateOrEditDocumentFileDto?.UpdateDocumentFileInput != null && CreateOrEditDocumentFileDto?.UpdateDocumentFileInput?.FileToken == null)
                context.Results.Add(new ValidationResult("document missing: " + CreateOrEditDocumentFileDto?.Name));


            if (!OriginFacilityId.HasValue)
                context.Results.Add(new ValidationResult("You Must Select Origin Facility"));
            if (ShippingRequestTripFlag!= ShippingRequestTripFlag.HomeDelivery && !DestinationFacilityId.HasValue)
                context.Results.Add(new ValidationResult("You Must Select Destination Facility"));

            if (ShippingRequestTripFlag == ShippingRequestTripFlag.HomeDelivery && RoutPoints.Count(x=>x.PickingType == PickingType.Dropoff)>0 && !DestinationFacilityId.HasValue)
                context.Results.Add(new ValidationResult("You Must Select Destination Facility"));

            if (EndTripDate != null && StartTripDate?.Date > EndTripDate.Value.Date)
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
                    throw new UserFriendlyException("YouMustEnterReceiver");
                }
            }
            if(ShippingRequestTripFlag == ShippingRequestTripFlag.HomeDelivery && RoutPoints!=null && RoutPoints.Count(x => x.PickingType == PickingType.Dropoff) > 0)
            {
                if (RoutPoints.Any(x => x.PickingType == PickingType.Dropoff && x.NeedsPOD == null))
                {
                    context.Results.Add(new ValidationResult("NeedsPODForDropsRequired"));
                }
                if (RoutPoints.Any(x => x.PickingType == PickingType.Dropoff && x.NeedsReceiverCode == null))
                {
                    context.Results.Add(new ValidationResult("NeedsReceiverCodeForDropsRequired"));
                }
            }
            
            if (ShippingRequestTripFlag != ShippingRequestTripFlag.HomeDelivery && RoutPoints.Where(x=>x.PickingType == PickingType.Dropoff).SelectMany(x=>x.GoodsDetailListDto).Any(x => x.UnitOfMeasureId == null))
            {
                context.Results.Add(new ValidationResult("GoodsUnitOfMeasureIsRequired"));
            }

            if (ShippingRequestTripFlag != ShippingRequestTripFlag.HomeDelivery && RoutPoints.Where(x => x.PickingType == PickingType.Dropoff).SelectMany(x => x.GoodsDetailListDto).Any(x => x.Description == null))
            {
                context.Results.Add(new ValidationResult("GoodsDescriptionIsRequired"));
            }

            if (ShippingRequestTripFlag != ShippingRequestTripFlag.HomeDelivery && RoutPoints.Where(x => x.PickingType == PickingType.Dropoff).SelectMany(x => x.GoodsDetailListDto).Any(x => x.Amount == null))
            {
                context.Results.Add(new ValidationResult("GoodsQuantityIsRequired"));
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