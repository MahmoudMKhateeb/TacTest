using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Threading;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using TACHYON.Authorization;
using TACHYON.Authorization.Users;
using TACHYON.EntityFrameworkCore;

namespace TACHYON.Migrations.Seed.Tenants
{
    public class TrackingPermissionForDriverSeeder
    {
        private readonly TACHYONDbContext _dbContext;
        private readonly UserManager _userManager;
        private readonly PermissionManager _permissionManager;

        public TrackingPermissionForDriverSeeder(TACHYONDbContext dbContext)
        {
            _dbContext = dbContext;
            _userManager = IocManager.Instance.Resolve<UserManager>();
            _permissionManager = IocManager.Instance.Resolve<PermissionManager>();
        }


        public void GrantTrackingPermissionForDrivers()
        {

            var drivers = _dbContext.Users.IgnoreQueryFilters().Where(x => x.IsDriver && !x.IsDeleted).AsQueryable();


            var notGrantedTrackingDrivers = (from driver in drivers
                from permission in _dbContext.UserPermissions.IgnoreQueryFilters().Where(x =>
                     x.Name == AppPermissions.Pages_Tracking && x.IsGranted && x.UserId == driver.Id &&
                    x.TenantId == driver.TenantId).DefaultIfEmpty()
                where permission == null
                select driver).ToList();

            if (notGrantedTrackingDrivers.IsNullOrEmpty()) return;
            
            var trackingPermission = _permissionManager.GetPermission(AppPermissions.Pages_Tracking);
            
            foreach (var driver in notGrantedTrackingDrivers)
            {
                AsyncHelper.RunSync(() => _userManager.GrantPermissionAsync(driver, trackingPermission));
            }

            _dbContext.SaveChanges();

        }
        
    }
}