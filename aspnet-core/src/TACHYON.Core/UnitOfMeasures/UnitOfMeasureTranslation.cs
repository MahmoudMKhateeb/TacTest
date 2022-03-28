using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Common;

namespace TACHYON.UnitOfMeasures
{
    [Table("UnitOfMeasureTranslations")]
    public class UnitOfMeasureTranslation : Entity, IEntityTranslation<UnitOfMeasure>, IHasDisplayName
    {
        public string DisplayName { get; set; }
        public UnitOfMeasure Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }
    }
}