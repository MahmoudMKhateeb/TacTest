using Abp;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.IdentityFramework;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TACHYON.Authorization.Roles;
using TACHYON.Authorization.Users;
using TACHYON.MultiTenancy;
using TACHYON.Net.Sms;

namespace TACHYON.Mobile
{
    public class MobileManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<UserOTP> _userOTPRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<UserLoginAttempt, long> _userLoginAttemptRepository;
        private readonly UserManager _userManager;
        private readonly AbpUserClaimsPrincipalFactory<User, Role> _claimsPrincipalFactory;
        private IClientInfoProvider ClientInfoProvider { get; set; }
        private readonly ISmsSender _smsSender;
        public MobileManager(IRepository<UserOTP> userOTPRepository,
            IRepository<Tenant> tenantRepository,
            IRepository<UserLoginAttempt, long> userLoginAttemptRepository,
            UserManager userManager,
            AbpUserClaimsPrincipalFactory<User, Role> claimsPrincipalFactory,
            ISmsSender smsSender)
        {
            _userOTPRepository = userOTPRepository;
            _tenantRepository = tenantRepository;
            _userLoginAttemptRepository = userLoginAttemptRepository;
            _userManager = userManager;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            ClientInfoProvider = NullClientInfoProvider.Instance;
            _smsSender = smsSender;
        }
        public async Task<double> CreateOTP(User user, string Language)
        {
            var userOTP = new UserOTP(user.Id);
            await _userOTPRepository.InsertAsync(userOTP);
            await _smsSender.SendAsync(user.PhoneNumber, L(TACHYONConsts.SMSOTP, new CultureInfo(Language), userOTP.OTP));
            return (userOTP.ExpireTime - Clock.Now).TotalSeconds;
        }

        public async Task<double?> CheckIsExistOTP(long userId)
        {
            var current = Clock.Now;
            var userOtp = await _userOTPRepository.GetAll().FirstOrDefaultAsync(x => x.UserId == userId && x.ExpireTime >= current);
            if (userOtp != null)
                return (userOtp.ExpireTime - current).TotalSeconds;

            var oldDate = Clock.Now.Subtract(new TimeSpan(0, 1, 0, 0, 0));
            var CountAttempts = await _userOTPRepository.GetAll().Where(x => x.UserId == userId && x.CreationTime > oldDate).CountAsync();
            if (CountAttempts >= 3)
                throw new UserFriendlyException(L("PleaseTryAgainInAnHour"));
            return null;
        }

        public async Task OTPValidate(long userId, string OTP)
        {
            var userOTP = await _userOTPRepository.FirstOrDefaultAsync(x => x.UserId == userId && x.OTP == OTP && x.ExpireTime >= Clock.Now);
            if (userOTP != null)
            {
                await _userOTPRepository.DeleteAsync(userOTP);
            }
            else
                throw new AbpAuthorizationException(L("InvalidOTPNumberORExpired"));
        }



        [UnitOfWork]
        public async Task<AbpLoginResult<Tenant, User>> LoginAsyn(string username, string tenancyName)
        {
            var result = await LoginAsyncInternal(username, tenancyName, true);
            await SaveLoginAttemptAsync(result, tenancyName, username);
            return result;
        }

        private async Task<AbpLoginResult<Tenant, User>> LoginAsyncInternal(string userNameOrEmailAddress, string tenancyName, bool shouldLockout)
        {
            //Get and check tenant
            Tenant tenant = default;
            using (UnitOfWorkManager.Current.SetTenantId(null))
            {

                //if (!TACHYONConsts.MultiTenancyEnabled)
                //{
                //    tenant = await GetDefaultTenantAsync();
                //}
                //else
                if (!string.IsNullOrWhiteSpace(tenancyName))
                {
                    tenant = await _tenantRepository.FirstOrDefaultAsync(t => t.TenancyName == tenancyName);
                    if (tenant == null)
                    {
                        return new AbpLoginResult<Tenant, User>(AbpLoginResultType.InvalidTenancyName);
                    }

                    if (!tenant.IsActive)
                    {
                        return new AbpLoginResult<Tenant, User>(AbpLoginResultType.TenantIsNotActive, tenant);
                    }
                }
            }

            var tenantId = tenant == null ? (int?)null : tenant.Id;
            using (UnitOfWorkManager.Current.SetTenantId(tenantId))
            {
                await _userManager.InitializeOptionsAsync(tenantId);


                var user = await _userManager.FindByNameOrEmailAsync(tenantId, userNameOrEmailAddress);
                if (user == null)
                {
                    return new AbpLoginResult<Tenant, User>(AbpLoginResultType.InvalidUserNameOrEmailAddress, tenant);
                }

                if (await _userManager.IsLockedOutAsync(user))
                {
                    return new AbpLoginResult<Tenant, User>(AbpLoginResultType.LockedOut, tenant, user);
                }

                if (shouldLockout)
                {
                    if (await TryLockOutAsync(tenantId, user.Id))
                    {
                        return new AbpLoginResult<Tenant, User>(AbpLoginResultType.LockedOut, tenant, user);
                    }
                }



                await _userManager.ResetAccessFailedCountAsync(user);

                return await CreateLoginResultAsync(user, tenant);
            }
        }

        private async Task<bool> TryLockOutAsync(int? tenantId, long userId)
        {

            using (UnitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                (await _userManager.AccessFailedAsync(user)).CheckErrors();

                var isLockOut = await _userManager.IsLockedOutAsync(user);


                return isLockOut;
            }
        }
        private async Task<Tenant> GetDefaultTenantAsync()
        {
            var tenant = await _tenantRepository.FirstOrDefaultAsync(t => t.TenancyName == AbpTenant<User>.DefaultTenantName);
            if (tenant == null)
            {
                throw new AbpException("There should be a 'Default' tenant if multi-tenancy is disabled!");
            }

            return tenant;
        }

        private async Task<AbpLoginResult<Tenant, User>> CreateLoginResultAsync(User user, Tenant tenant = null)
        {
            if (!user.IsActive)
            {
                return new AbpLoginResult<Tenant, User>(AbpLoginResultType.UserIsNotActive);
            }



            //if (await IsPhoneConfirmationRequiredForLoginAsync(user.TenantId) && !user.IsPhoneNumberConfirmed)
            //{
            //    return new AbpLoginResult<TTenant, TUser>(AbpLoginResultType.UserPhoneNumberIsNotConfirmed);
            //}

            var principal = await _claimsPrincipalFactory.CreateAsync(user);

            return new AbpLoginResult<Tenant, User>(
                tenant,
                user,
                principal.Identity as ClaimsIdentity
            );
        }

        private async Task SaveLoginAttemptAsync(AbpLoginResult<Tenant, User> loginResult, string tenancyName, string userNameOrEmailAddress)
        {
            var tenantId = loginResult.Tenant != null ? loginResult.Tenant.Id : (int?)null;
            using (UnitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var loginAttempt = new UserLoginAttempt
                {
                    TenantId = tenantId,
                    TenancyName = tenancyName,

                    UserId = loginResult.User != null ? loginResult.User.Id : (long?)null,
                    UserNameOrEmailAddress = userNameOrEmailAddress,

                    Result = loginResult.Result,

                    BrowserInfo = ClientInfoProvider.BrowserInfo,
                    ClientIpAddress = ClientInfoProvider.ClientIpAddress,
                    ClientName = ClientInfoProvider.ComputerName,
                };

                await _userLoginAttemptRepository.InsertAsync(loginAttempt);
            }
        }

    }
}