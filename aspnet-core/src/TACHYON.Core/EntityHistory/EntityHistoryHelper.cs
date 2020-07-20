using TACHYON.Cities;
using TACHYON.Routs.RoutTypes;
using TACHYON.Goods.GoodCategories;
using TACHYON.Trailers;
using TACHYON.Trailers.TrailerStatuses;
using TACHYON.Trailers.TrailerTypes;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks;
using System;
using System.Linq;
using Abp.Organizations;
using TACHYON.Authorization.Roles;
using TACHYON.MultiTenancy;
using TACHYON.Trailers.PayloadMaxWeights;

namespace TACHYON.EntityHistory
{
    public static class EntityHistoryHelper
    {
        public const string EntityHistoryConfigurationName = "EntityHistory";

        public static readonly Type[] HostSideTrackedTypes =
        {
            typeof(City),
            typeof(RoutType),
            typeof(GoodCategory),
            typeof(TrailerStatus),
            typeof(PayloadMaxWeight),
            typeof(TrailerType),
            typeof(TrucksType),
            typeof(OrganizationUnit), typeof(Role), typeof(Tenant)
        };

        public static readonly Type[] TenantSideTrackedTypes =
        {
            typeof(Trailer),
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
