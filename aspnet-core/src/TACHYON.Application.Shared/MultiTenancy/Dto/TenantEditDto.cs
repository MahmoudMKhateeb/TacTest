using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.MultiTenancy;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.MultiTenancy.Dto
{
    public class TenantEditDto : EntityDto
    {
        [Required]
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        [RegularExpression(TenantConsts.TenancyNameRegex)]
        public string TenancyName { get; set; }


        [Required]
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        [RegularExpression(TenantConsts.TenancyLegalNameRegex)]
        public string companyName { get; set; }


        [Required]
        [StringLength(TenantConsts.MaxNameLength)]
        [RegularExpression(TenantConsts.TenancyLegalNameRegex)]
        public string Name { get; set; }

        public string MobileNo { get; set; }
        [Required] public virtual string Address { get; set; }
        [Required] public virtual int CountryId { get; set; }
        [Required] public virtual int CityId { get; set; }

        [DisableAuditing] public string ConnectionString { get; set; }

        public bool IsActive { get; set; }

        public DateTime? SubscriptionEndDateUtc { get; set; }

        public bool IsInTrialPeriod { get; set; }

        [Required]
        [RegularExpression(TenantConsts.MoiNumberRegex)]
        public string MoiNumber { get; set; }
        public int? InsuranceCoverage { get; set; }
        public int? ValueOfGoods { get; set; }
    }
}