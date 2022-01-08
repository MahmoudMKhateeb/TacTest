using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.DriverLicenseTypes
{
    [Table("DriverLicenseTypes")]
    public class DriverLicenseType : FullAuditedEntity
    {

        [Required]
        [StringLength(DriverLicenseTypeConsts.MaxNameLength, MinimumLength = DriverLicenseTypeConsts.MinNameLength)]
        public virtual string Name { get; set; }

        [Range(DriverLicenseTypeConsts.MinWasIIntegrationIdValue, DriverLicenseTypeConsts.MaxWasIIntegrationIdValue)]
        public virtual int WasIIntegrationId { get; set; }

        public virtual bool ApplicableforWaslRegistration { get; set; }


    }
}