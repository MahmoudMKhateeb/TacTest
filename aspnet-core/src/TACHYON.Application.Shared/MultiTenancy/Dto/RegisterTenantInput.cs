using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using System.ComponentModel.DataAnnotations;
using TACHYON.MultiTenancy.Payments;

namespace TACHYON.MultiTenancy.Dto
{
    public class RegisterTenantInput
    {
        [Required]
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        public string TenancyName { get; set; }

        [Required]
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        public string companyName { get; set; }

        //tenant mobile no
        [Required]
        [StringLength(RegisterTenantInputConsts.MaxMobileNumberLength,
            MinimumLength = RegisterTenantInputConsts.MinMobileNumberLength)]
        public string MobileNo { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string AdminEmailAddress { get; set; }

        [StringLength(AbpUserBase.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string AdminPassword { get; set; }

        [DisableAuditing] public string CaptchaResponse { get; set; }

        public SubscriptionStartType SubscriptionStartType { get; set; }

        public int? EditionId { get; set; }

        public virtual string Address { get; set; }
        public virtual int CountryId { get; set; }
        public virtual int CityId { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string UserAdminFirstName { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxSurnameLength)]
        public string UserAdminSurname { get; set; }

        [Required]
        [RegularExpression(TenantConsts.MoiNumberRegex)]
        public string MoiNumber { get; set; }

        public string FinancialName { get; set; }
        public string FinancialPhone { get; set; }
        public string FinancialEmail { get; set; }

    }

    /// <summary>
    /// Constants For RegisterTenantInput
    /// Usually Used For Validation and Constrains ...etc
    /// </summary>
    public static class RegisterTenantInputConsts
    {
        public const int MaxMobileNumberLength = 16;
        public const int MinMobileNumberLength = 11;
    }
}