using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.Extensions;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.DriverLicenseTypes;
using TACHYON.Integration.WaslIntegration;
using TACHYON.Nationalities;

namespace TACHYON.Authorization.Users
{
    /// <summary>
    /// Represents a user in the system.
    /// </summary>
    public class User : AbpUser<User>, IWaslIntegrated
    {
        [StringLength(12)]
        public string AccountNumber { get; set; }
        public virtual Guid? ProfilePictureId { get; set; }

        public virtual bool ShouldChangePasswordOnNextLogin { get; set; }

        public DateTime? SignInTokenExpireTimeUtc { get; set; }

        public string SignInToken { get; set; }

        public string GoogleAuthenticatorKey { get; set; }

        public List<UserOrganizationUnit> OrganizationUnits { get; set; }

        //Can add application specific user properties here
        public virtual bool IsDriver { get; set; }

        public virtual string Address { get; set; }
        public virtual int? NationalityId { get; set; }

        [ForeignKey("NationalityId")]
        public virtual Nationality NationalityFk { get; set; }
        public virtual string ExperienceField { get; set; }
        public virtual DateTime? DateOfBirth { get; set; }
        public string HijriDateOfBirth { get; set; }
        public int? DriverLicenseTypeId { get; set; }

        [ForeignKey("DriverLicenseTypeId")]
        public DriverLicenseType DriverLicenseTypeFk { get; set; }

        public bool IsWaslIntegrated { get; set; }
        public string WaslIntegrationErrorMsg { get; set; }



        public User()
        {
            IsLockoutEnabled = true;
            IsTwoFactorEnabled = true;
        }

        /// <summary>
        /// Creates admin <see cref="User"/> for a tenant.
        /// </summary>
        /// <param name="tenantId">Tenant Id</param>
        /// <param name="emailAddress">Email address</param>
        /// <returns>Created <see cref="User"/> object</returns>
        public static User CreateTenantAdminUser(int tenantId, string emailAddress)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = AdminUserName,
                Name = AdminUserName,
                Surname = AdminUserName,
                EmailAddress = emailAddress,
                Roles = new List<UserRole>(),
                OrganizationUnits = new List<UserOrganizationUnit>()
            };

            user.SetNormalizedNames();

            return user;
        }

        public override void SetNewPasswordResetCode()
        {
            /* This reset code is intentionally kept short.
             * It should be short and easy to enter in a mobile application, where user can not click a link.
             */
            PasswordResetCode = Guid.NewGuid().ToString("N").Truncate(10).ToUpperInvariant();
        }

        public void Unlock()
        {
            AccessFailedCount = 0;
            LockoutEndDateUtc = null;
        }

        public void SetSignInToken()
        {
            SignInToken = Guid.NewGuid().ToString();
            SignInTokenExpireTimeUtc = Clock.Now.AddMinutes(1).ToUniversalTime();
        }
    }
}