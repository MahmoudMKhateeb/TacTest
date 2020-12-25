using TACHYON.TermsAndConditions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace TACHYON.TermsAndConditions
{
    [Table("TermAndConditionTranslations")]
    [Audited]
    public class TermAndConditionTranslation : Entity, IEntityTranslation<TermAndCondition>
    {

        [Required]
        public virtual string Content { get; set; }

        [Required]
        [StringLength(TermAndConditionTranslationConsts.MaxLanguageLength, MinimumLength = TermAndConditionTranslationConsts.MinLanguageLength)]
        public virtual string Language { get; set; }

            [ForeignKey("CoreId")]
        public TermAndCondition Core { get; set; }

        public virtual int CoreId { get; set; }


    }
}