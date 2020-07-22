using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Authorization.Users.Dto
{
    //Mapped to/from User in CustomDtoMapper
    public class UserEditDto : IPassivable
    {
        /// <summary>
        /// Set null to create a new user. Set user's Id to update a user
        /// </summary>
        public long? Id { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxSurnameLength)]
        public string Surname { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        [StringLength(UserConsts.MaxPhoneNumberLength)]
        public string PhoneNumber { get; set; }

        // Not used "Required" attribute since empty value is used to 'not change password'
        [StringLength(AbpUserBase.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }

        public bool IsActive { get; set; }

        public bool ShouldChangePasswordOnNextLogin { get; set; }

        public virtual bool IsTwoFactorEnabled { get; set; }

        public virtual bool IsLockoutEnabled { get; set; }


        #region Tachyon details
        public virtual bool IsDriver { get; set; }

        public virtual string Address { get; set; }
        public virtual string Nationality { get; set; }
        public virtual string DrivingLicenseNumber { get; set; }
        public virtual DateTime DrivingLicenseIssuingDate { get; set; }
        public virtual DateTime DrivingLicenseExpiryDate { get; set; }
        public virtual string ExperienceField { get; set; }

        #endregion
    }
}