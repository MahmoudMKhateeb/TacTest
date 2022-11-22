using TACHYON.Actors;

using System;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TACHYON.Documents.DocumentFiles.Dtos;
using Abp.Runtime.Validation;

namespace TACHYON.Actors.Dtos
{
    public class CreateOrEditActorDto : EntityDto<int?>, ICustomValidate
    {

        [Required]
        [StringLength(ActorConsts.MaxCompanyNameLength, MinimumLength = ActorConsts.MinCompanyNameLength)]
        public string CompanyName { get; set; }

        public ActorTypesEnum ActorType { get; set; }

        [Required]
        [RegularExpression(ActorConsts.MoiNumberRegex)]
        public string MoiNumber { get; set; }

        [Required]
        public string Address { get; set; }

        public string Logo { get; set; }

        [Required]
        public string MobileNumber { get; set; }

        [Required]
        public string Email { get; set; }

        [Range(1,int.MaxValue)]
        public int InvoiceDueDays { get; set; }
        public bool IsActive { get; set; }
        public string CR { get; set; }
        public string VatCertificate { get; set; }

        /// <summary>
        /// logo
        /// </summary>
        public CreateOrEditDocumentFileDto CreateOrEditDocumentFileDto { get; set; }

        /// <summary>
        /// required documents
        /// </summary>
        public List<CreateOrEditDocumentFileDto> CreateOrEditDocumentFileDtos { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if(CreateOrEditDocumentFileDtos is null || CreateOrEditDocumentFileDtos.Count == 0)
            {
                if(string.IsNullOrEmpty(CR))
                    context.Results.Add(new ValidationResult("CR is required"));
                if(VatCertificate is null)
                    context.Results.Add(new ValidationResult("Vat Certificate is required"));
            }
        }
    }
}