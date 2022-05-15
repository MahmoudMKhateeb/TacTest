using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Common;

namespace TACHYON.DriverLicenseTypes
{
    [Table("DriverLicenseTypes")]
    public class DriverLicenseType : FullAuditedEntity, IMultiLingualEntity<DriverLicenseTypeTranslation>, IHasKey
    {
        public virtual string Key { get; set; }

        [Range(DriverLicenseTypeConsts.MinWasIIntegrationIdValue, DriverLicenseTypeConsts.MaxWasIIntegrationIdValue)]
        public virtual int WasIIntegrationId { get; set; }

        public virtual bool ApplicableforWaslRegistration { get; set; }
        public ICollection<DriverLicenseTypeTranslation> Translations { get; set; }
    }
}