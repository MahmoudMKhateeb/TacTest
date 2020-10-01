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

namespace TACHYON.Documents.DocumentFiles
{
    [Table("DocumentFiles")]
    [Audited]
    public class DocumentFile : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }


        [Required]
        [StringLength(DocumentFileConsts.MaxNameLength, MinimumLength = DocumentFileConsts.MinNameLength)]
        public virtual string Name { get; set; }

        [Required]
        [StringLength(DocumentFileConsts.MaxExtnLength, MinimumLength = DocumentFileConsts.MinExtnLength)]
        public virtual string Extn { get; set; }

        public virtual Guid BinaryObjectId { get; set; }

        public virtual DateTime ExpirationDate { get; set; }

        public virtual bool IsAccepted { get; set; }
        public virtual bool IsRejected { get; set; }


        public virtual long DocumentTypeId { get; set; }

        [ForeignKey("DocumentTypeId")]
        public DocumentType DocumentTypeFk { get; set; }

        public virtual Guid? TruckId { get; set; }

        [ForeignKey("TruckId")]
        public Truck TruckFk { get; set; }

        public virtual long? TrailerId { get; set; }

        [ForeignKey("TrailerId")]
        public Trailer TrailerFk { get; set; }

        public virtual long? UserId { get; set; }

        [ForeignKey("UserId")]
        public User UserFk { get; set; }

        public virtual long? RoutStepId { get; set; }

        [ForeignKey("RoutStepId")]
        public RoutStep RoutStepFk { get; set; }

        public int? Number { get; set; }

        public string Notes { get; set; }



    }
}