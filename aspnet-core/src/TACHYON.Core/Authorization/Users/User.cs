using Abp.Authorization.Users;
using Abp.Extensions;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Actors;
using TACHYON.DriverLicenseTypes;
using TACHYON.Integration.WaslIntegration;
using TACHYON.Nationalities;
using TACHYON.Rating;
using TACHYON.Shipping.Dedicated;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.Authorization.Users
{
    /// <summary>
    /// Represents a user in the system.
    /// </summary>
    public class User : AbpUser<User>, IWaslIntegrated,IHasRating, IMayHaveCarrierActor
    {
        [StringLength(12)] public string AccountNumber { get; set; }
        public virtual Guid? ProfilePictureId { get; set; }

        public virtual bool ShouldChangePasswordOnNextLogin { get; set; }

        public DateTime? SignInTokenExpireTimeUtc { get; set; }

        public string SignInToken { get; set; }

        public string GoogleAuthenticatorKey { get; set; }

        public List<UserOrganizationUnit> OrganizationUnits { get; set; }

        //Can add application specific user properties here
        public virtual bool IsDriver { get; set; }

        public UserDriverStatus? DriverStatus { get; set; }
        //public WorkingStatus WorkingStatus { get; set; }
        //public string WorkingShippingRequestReference { get; set; }
        public virtual string Address { get; set; }
        public virtual int? NationalityId { get; set; }

        [ForeignKey("NationalityId")] public virtual Nationality NationalityFk { get; set; }
        public virtual string ExperienceField { get; set; }
        public virtual DateTime? DateOfBirth { get; set; }
        public string HijriDateOfBirth { get; set; }
        public int? DriverLicenseTypeId { get; set; }

        [ForeignKey("DriverLicenseTypeId")]
        public DriverLicenseType DriverLicenseTypeFk { get; set; }

        public bool IsWaslIntegrated { get; set; }
        public string WaslIntegrationErrorMsg { get; set; }
        [ForeignKey("CarrierActorId")]
        public Actor CarrierActorFk { get; set; }

        public int? CarrierActorId { get; set; }



        /// <summary>
        /// This field is final rate for driver
        /// </summary>
        public decimal Rate { get; set; }

        public int RateNumber { get; set; }

        public int? DriverIssueNumber { get; set; }

        public ICollection<DedicatedShippingRequestDriver> DedicatedShippingRequestDrivers { get; set; }

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

        //public void StartWork(string requestReference)
        //{
        //    WorkingStatus = WorkingStatus.Busy;
        //    WorkingShippingRequestReference = requestReference;
        //}

        //public void EndWork()
        //{
        //    WorkingStatus = WorkingStatus.Active;
        //    WorkingShippingRequestReference = "";
        //}
    }
}