using TACHYON.Actors;

using System;
using Abp.Application.Services.Dto;
using TACHYON.Documents.DocumentFiles.Dtos;

namespace TACHYON.Actors.Dtos
{
    public class ActorDto : EntityDto
    {
        public string CompanyName { get; set; }

        public ActorTypesEnum ActorType { get; set; }

        public string MoiNumber { get; set; }

        public string Address { get; set; }

        public string MobileNumber { get; set; }

        public string Email { get; set; }

        public int InvoiceDueDays { get; set; }
        public bool IsActive { get; set; }
        public DocumentFileDto DocumentFile { get; set; }

        #region SAP
        
        public int? CityId {get;set;}
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


    }
}