using TACHYON.Trailers.TrailerStatuses;
using TACHYON.Trailers.PayloadMaxWeight;
using TACHYON.Trailers.TrailerTypes;
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
            typeof(TrailerStatus),
            typeof(PayloadMaxWeight),
            typeof(TrailerType),
            typeof(TrucksType),
            typeof(OrganizationUnit), typeof(Role), typeof(Tenant)
        };

        public static readonly Type[] TenantSideTrackedTypes =
        {
            typeof(Truck),
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
