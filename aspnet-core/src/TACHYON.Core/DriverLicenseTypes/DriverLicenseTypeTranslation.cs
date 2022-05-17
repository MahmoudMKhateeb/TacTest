using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Common;

namespace TACHYON.DriverLicenseTypes
{
    [Table("DriverLicenseTypeTranslations")]
    public class DriverLicenseTypeTranslation : Entity, IEntityTranslation<DriverLicenseType>, IHasDisplayName
    {
        public string DisplayName { get; set; }
        public DriverLicenseType Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }
    }
}