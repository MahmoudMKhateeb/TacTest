using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using Castle.Core.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using TACHYON.Documents.DocumentsEntities;
using TACHYON.Documents.DocumentTypes.Dtos;

namespace TACHYON.Documents.DocumentFiles.Dtos
{
    public class CreateOrEditDocumentFileDto : EntityDto<Guid?>, ICustomValidate
    {
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (DocumentTypeDto == null)
            {
                return;
            }
            if (!DocumentTypeDto.HasNumber)
            {
                return;
            }

            if (DocumentTypeDto.NumberMaxDigits != 0 && Number.ToString().Length > DocumentTypeDto.NumberMaxDigits)
            {
                context.Results.Add(new ValidationResult("Number digits must be less than or equal " + DocumentTypeDto.NumberMaxDigits));
            }

            if (DocumentTypeDto.NumberMinDigits != 0 && Number.ToString().Length < DocumentTypeDto.NumberMinDigits)
            {
                context.Results.Add(new ValidationResult("Number digits must be greater than or equal " + DocumentTypeDto.NumberMinDigits));
            }

            if (IsAccepted && IsRejected)
                context.Results.Add(new ValidationResult("document cant be accepted and rejected at same time "));

            //? FYI DisplayName Mapped From Core Entity and Not From Translation Entity
            if (DocumentTypeDto.DisplayName.ToUpper().Contains("OTHER") && OtherDocumentTypeName.IsNullOrEmpty())
                context.Results.Add(new ValidationResult("Document Type Can't Be Other And Empty"));
        }

        public DocumentTypeDto DocumentTypeDto { get; set; }
        public string OtherDocumentTypeName { get; set; }
        public UpdateDocumentFileInput UpdateDocumentFileInput { get; set; }

        [Required]
        [StringLength(DocumentFileConsts.MaxNameLength, MinimumLength = DocumentFileConsts.MinNameLength)]
        public string Name { get; set; }

        [StringLength(DocumentFileConsts.MaxExtnLength, MinimumLength = DocumentFileConsts.MinExtnLength)]
        public string Extn { get; set; }

        public Guid BinaryObjectId { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public virtual bool IsAccepted { get; set; }

        public virtual bool IsRejected { get; set; }

        public long? DocumentTypeId { get; set; }
        public long? TruckId { get; set; }

        public long? TrailerId { get; set; }

        public long? UserId { get; set; }

        public long? RoutStepId { get; set; }

        public int? ShippingRequestTripId { get; set; }

        public string Number { get; set; }
        public string Notes { get; set; }
        public string HijriExpirationDate { get; set; }


        //Entity ID is the id of the :truck, user,trailer, shipment or any other entity
        public string EntityId { get; set; }
        public DocumentsEntitiesEnum EntityType { get; set; }

    }
}