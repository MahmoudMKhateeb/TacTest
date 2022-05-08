using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.MultiTenancy.Dto
{
    public class CreateTenantInput
    {
        [Required]
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        [RegularExpression(TenantConsts.TenancyNameRegex)]
        public string TenancyName { get; set; }

        [Required]
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        [RegularExpression(TenantConsts.TenancyNameRegex)]
        public string companyName { get; set; }

        public string MobileNo { get; set; }

        [Required]
        [StringLength(TenantConsts.MaxNameLength)]
        [RegularExpression(TenantConsts.TenancyLegalNameRegex)]
        public string Name { get; set; }


        [Required] public virtual string Address { get; set; }
        [Required] public virtual int CountryId { get; set; }
        [Required] public virtual int CityId { get; set; }


        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string AdminEmailAddress { get; set; }

        [StringLength(AbpUserBase.MaxPasswordLength)]
        [DisableAuditing]
        public string AdminPassword { get; set; }

        [MaxLength(AbpTenantBase.MaxConnectionStringLength)]
        [DisableAuditing]
        public string ConnectionString { get; set; }

        public bool ShouldChangePasswordOnNextLogin { get; set; }

        public bool SendActivationEmail { get; set; }

        public int? EditionId { get; set; }

        public bool IsActive { get; set; }

        public DateTime? SubscriptionEndDateUtc { get; set; }

        public bool IsInTrialPeriod { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string UserAdminFirstName { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxSurnameLength)]
        public string UserAdminSurname { get; set; }

        [Required]
        [RegularExpression(TenantConsts.MoiNumberRegex)]
        public string MoiNumber { get; set; }
    }
}