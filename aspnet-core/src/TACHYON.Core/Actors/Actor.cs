using TACHYON.Actors;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Organizations;
using System.Collections.Generic;
using TACHYON.Shipping.ShippingRequests;

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

        [Required]
        [RegularExpression(ActorConsts.MoiNumberRegex)]
        public virtual string MoiNumber { get; set; }

        [Required]
        public virtual string Address { get; set; }

        public virtual string Logo { get; set; }

        [Required]
        public virtual string MobileNumber { get; set; }

        [Required]
        public virtual string Email { get; set; }

        public long OrganizationUnitId { get; set; }

        public int InvoiceDueDays { get; set; }

        [InverseProperty(nameof(ShippingRequest.ShipperActorFk))]
        public ICollection<ShippingRequest> ActorShipperShippingRequests { get; set; }

         [InverseProperty(nameof(ShippingRequest.CarrierActorFk))]
        public ICollection<ShippingRequest> ActorCarrierShippingRequests { get; set; }
    }
}