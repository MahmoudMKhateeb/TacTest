using TACHYON.Actors;

using System;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TACHYON.Documents.DocumentFiles.Dtos;

namespace TACHYON.Actors.Dtos
{
    public class CreateOrEditActorDto : EntityDto<int?>
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

        /// <summary>
        /// logo
        /// </summary>
        public CreateOrEditDocumentFileDto CreateOrEditDocumentFileDto { get; set; }

        /// <summary>
        /// required documents
        /// </summary>
        public List<CreateOrEditDocumentFileDto> CreateOrEditDocumentFileDtos { get; set; }



    }
}