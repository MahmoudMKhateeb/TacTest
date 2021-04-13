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
        public string TenancyName { get; set; }

        [Required]
        [StringLength(TenantConsts.MaxNameLength)]
        public string Name { get; set; }

        public string MobileNo { get; set; }
        [Required]
        public virtual string Address { get; set; }
        [Required]
        public virtual int CountryId { get; set; }
        [Required]
        public virtual int CityId { get; set; }

        [DisableAuditing]
        public string ConnectionString { get; set; }

        public int? EditionId { get; set; }

        public bool IsActive { get; set; }

        public DateTime? SubscriptionEndDateUtc { get; set; }

        public bool IsInTrialPeriod { get; set; }
    }
}