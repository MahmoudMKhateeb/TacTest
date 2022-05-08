using Abp.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TACHYON.Authorization.Roles;
using TACHYON.MultiTenancy;

namespace TACHYON.Authorization.Users
{
    public class UserClaimsPrincipalFactory : AbpUserClaimsPrincipalFactory<User, Role>
    {
        private readonly TenantManager _tenantManager;

        public UserClaimsPrincipalFactory(
            UserManager userManager,
            RoleManager roleManager,
            IOptions<IdentityOptions> optionsAccessor,
            TenantManager tenantManager)
            : base(
                userManager,
                roleManager,
                optionsAccessor)
        {
            _tenantManager = tenantManager;
        }

        public override async Task<ClaimsPrincipal> CreateAsync(User user)
        {
            var claim = await base.CreateAsync(user);

            if (user.TenantId.HasValue)
            {
                var editionId = (await _tenantManager.GetByIdAsync(user.TenantId.Value)).EditionId;
                claim.Identities.First().AddClaim(new Claim(AppConsts.UserEditionId, editionId.ToString()));
            }

            return claim;
        }
    }
}