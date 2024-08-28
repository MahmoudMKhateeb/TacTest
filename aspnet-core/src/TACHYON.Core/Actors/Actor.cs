using TACHYON.Actors;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Organizations;
using System.Collections.Generic;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Cities;

namespace TACHYON.Actors
{
    [Table("Actors")]
    public class Actor : FullAuditedEntity, IMustHaveTenant ,IMustHaveOrganizationUnit
    {
        public int TenantId { get; set; }

        [Required]
        [StringLength(ActorConsts.MaxCompanyNameLength, MinimumLength = ActorConsts.MinCompanyNameLength)]
        public virtual string CompanyName { get; set; }

        public virtual ActorTypesEnum ActorType { get; set; }

        
        [RegularExpression(ActorConsts.MoiNumberRegex)]
        public virtual string MoiNumber { get; set; }

       
        public virtual string Address { get; set; }

        public virtual string Logo { get; set; }

       
        public virtual string MobileNumber { get; set; }

       
        public virtual string Email { get; set; }

        public long OrganizationUnitId { get; set; }

        public int? InvoiceDueDays { get; set; }
        public bool IsActive { get; set; } = true;
        /// <summary>
        /// Additional fields when no documents required
        /// </summary>
        public string CR { get; set; }
        /// <summary>
        /// Additional fields when no documents required
        /// </summary>
        public string VatCertificate { get; set; }

        [InverseProperty(nameof(ShippingRequest.ShipperActorFk))]
        public ICollection<ShippingRequest> ActorShipperShippingRequests { get; set; }

         [InverseProperty(nameof(ShippingRequest.CarrierActorFk))]
        public ICollection<ShippingRequest> ActorCarrierShippingRequests { get; set; }

        #region SAP
        public int? CityId {get;set;}

        [ForeignKey("CityId")] public City CityFk { get; set; }
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