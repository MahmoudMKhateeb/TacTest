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

        
        [RegularExpression(ActorConsts.MoiNumberRegex)]
        public string MoiNumber { get; set; }

       
        public string Address { get; set; }

        public string Logo { get; set; }

        
        public string MobileNumber { get; set; }

       
        public string Email { get; set; }

        
        public int? InvoiceDueDays { get; set; }
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

        #region SAP
        public int CityId {get;set;}
        public string Region {get;set;}
        public string FirstName {get;set;}
        public string LastName {get;set;}
        public SalesOfficeTypeEnum SalesOfficeType {get;set;}
        public string SalesGroup {get;set;}
        public string TrasportationZone {get;set;}
        public string Reconsaccoun {get;set;}
        public string PostalCode {get;set;}
        public string Division {get;set;}
        public string District {get;set;}
        public string CustomerGroup {get;set;}
        public string BuildingCode {get;set;}
        public string AccountType {get;set;}
        public ActorDischannelEnum? actorDischannelEnum{get;set;}
        #endregion
        public void AddValidationErrors(CustomValidationContext context)
        {
            if(CreateOrEditDocumentFileDtos is null || CreateOrEditDocumentFileDtos.Count == 0)
            {
                if(string.IsNullOrEmpty(CR))
                    context.Results.Add(new ValidationResult("CR is required"));
                if(VatCertificate is null)
                    context.Results.Add(new ValidationResult("Vat Certificate is required"));
            }

            if (ActorType is ActorTypesEnum.MySelf)
            {
                context.Results.Add(new ValidationResult("You can not create or edit an actor with myself type"));
            }
        }
    }
}