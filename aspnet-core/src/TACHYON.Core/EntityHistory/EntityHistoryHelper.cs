using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks;
using System;
using System.Linq;
using Abp.Organizations;
using TACHYON.Authorization.Roles;
using TACHYON.MultiTenancy;

namespace TACHYON.EntityHistory
{
    public static class EntityHistoryHelper
    {
        public const string EntityHistoryConfigurationName = "EntityHistory";

        public static readonly Type[] HostSideTrackedTypes =
        {
            typeof(TrucksType),
            typeof(OrganizationUnit), typeof(Role), typeof(Tenant)
        };

        public static readonly Type[] TenantSideTrackedTypes =
        {
            typeof(TrucksType),
            typeof(TruckStatus),
            typeof(OrganizationUnit), typeof(Role)
        };

        public static readonly Type[] TrackedTypes =
            HostSideTrackedTypes
                .Concat(TenantSideTrackedTypes)
                .GroupBy(type => type.FullName)
                .Select(types => types.First())
                .ToArray();
    }
}
