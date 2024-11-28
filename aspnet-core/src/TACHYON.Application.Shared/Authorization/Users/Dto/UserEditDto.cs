﻿using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using TACHYON.Actors;
using TACHYON.Integration.BayanIntegration;

namespace TACHYON.Authorization.Users.Dto
{
    //Mapped to/from User in CustomDtoMapper
    public class UserEditDto : IPassivable , ICanBeExcludedFromBayanIntegration , IMayHaveShipperActor
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

        public int? TenantId { get; set; }
        public bool IsActive { get; set; }

        public bool ShouldChangePasswordOnNextLogin { get; set; }

        public virtual bool IsTwoFactorEnabled { get; set; }

        public virtual bool IsLockoutEnabled { get; set; }

        public bool IsAvailable { get; set; }

        #region Tachyon details

        public virtual bool IsDriver { get; set; }

        public virtual string Address { get; set; }
        public virtual int? NationalityId { get; set; }
        public virtual string ExperienceField { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string HijriDateOfBirth { get; set; }

        public int? DriverLicenseTypeId { get; set; }

        public int? DriverIssueNumber { get; set; }

        public int? CarrierActorId { get; set; }

        public bool ExcludeFromBayanIntegration { get; set; }
        public int? ShipperActorId { get ; set ; }

        #endregion

    }
}