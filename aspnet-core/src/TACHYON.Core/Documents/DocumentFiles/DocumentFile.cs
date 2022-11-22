using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Editions;
using Abp.Application.Features;
using TACHYON.Authorization.Users;
using TACHYON.Documents.DocumentTypes;
using TACHYON.Routs.RoutSteps;
using TACHYON.Trailers;
using TACHYON.Trucks;
using TACHYON.MultiTenancy;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.ShippingRequestAndTripNotes;
using TACHYON.Actors;

namespace TACHYON.Documents.DocumentFiles
{
    [Table("DocumentFiles")]
    [Audited]
    public class DocumentFile : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [ForeignKey("TenantId")] public Tenant TenantFk { get; set; }

        [Required]
        [StringLength(DocumentFileConsts.MaxNameLength, MinimumLength = DocumentFileConsts.MinNameLength)]
        public virtual string Name { get; set; }

      
        [StringLength(DocumentFileConsts.MaxExtnLength, MinimumLength = DocumentFileConsts.MinExtnLength)]
        public virtual string Extn { get; set; }

        /// <summary>
        /// nullAble Because we use it in import-Trucks-from-excel action
        /// </summary>
        public virtual Guid? BinaryObjectId { get; set; }

        public virtual DateTime? ExpirationDate { get; set; }

        public virtual bool IsAccepted { get; set; }
        public virtual bool IsRejected { get; set; }
        public string RejectionReason { get; set; }

        public virtual long? DocumentTypeId { get; set; }

        [ForeignKey("DocumentTypeId")] public DocumentType DocumentTypeFk { get; set; }

        /// <summary>
        /// in trip entity case, user select attachment type "document type", when the option selected to others, this field must be filled
        /// </summary>
        public string OtherDocumentTypeName { get; set; }

        public virtual long? TruckId { get; set; }

        [ForeignKey("TruckId")] public Truck TruckFk { get; set; }

        public virtual long? TrailerId { get; set; }

        [ForeignKey("TrailerId")] public Trailer TrailerFk { get; set; }

        public virtual long? UserId { get; set; }

        [ForeignKey("UserId")] public User UserFk { get; set; }

        public virtual long? RoutStepId { get; set; }

        public virtual int? NoteId { get; set; }
        [ForeignKey("NoteId")] public ShippingRequestAndTripNote NoteFk { get; set; }
        [ForeignKey("RoutStepId")] public RoutStep RoutStepFk { get; set; }

        public virtual int? ShippingRequestTripId { get; set; }

        [ForeignKey("ShippingRequestTripId")] public ShippingRequestTrip ShippingRequestTripFk { get; set; }


        public string Number { get; set; }

        public string Notes { get; set; }
        public string HijriExpirationDate { get; set; }
        public virtual int? ActorId  { get; set; }

        [ForeignKey("ActorId")] 
        public Actor ActorFk { get; set; }

    }
}