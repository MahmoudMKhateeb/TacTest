// unset

using Abp;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Extensions;
using Abp.Threading;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.Record;
using System;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.BayanIntegration;
using TACHYON.Firebases;
using TACHYON.MultiTenancy;
using TACHYON.Notifications;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;

namespace TACHYON.Shipping.Trips
{
    public class TripUpdatedEventHandler : IEventHandler<EntityUpdatedEventData<ShippingRequestTrip>>, ITransientDependency
    {
        private readonly BayanIntegrationManager _bayanIntegrationManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<User, long> _lookupUserRepo;
        private readonly IRepository<Tenant> _lookupTenantRepo;

        public TripUpdatedEventHandler(BayanIntegrationManager bayanIntegrationManager, IAppNotifier appNotifier, IUnitOfWorkManager unitOfWorkManager,
            IRepository<User, long> lookupUserRepo, IRepository<Tenant> lookupTenantRepo)
        {
            _bayanIntegrationManager = bayanIntegrationManager;
            _appNotifier = appNotifier;
            _unitOfWorkManager = unitOfWorkManager;
            _lookupUserRepo = lookupUserRepo;
            _lookupTenantRepo = lookupTenantRepo;
        }

        public void HandleEvent(EntityUpdatedEventData<ShippingRequestTrip> eventData)
        {
            //todo send notification for driver and shipper and carrier using eventBus
            AsyncHelper.RunSync(async () => await _appNotifier.NotifyTripUpdated(eventData.Entity));

            // Add BayanIntegration Jobs To the Queue
            if (eventData.Entity.AssignedDriverUserId.HasValue && eventData.Entity.AssignedTruckId.HasValue)
            {
                if (eventData.Entity.BayanId.IsNullOrEmpty())
                {
                    AsyncHelper.RunSync(() => _bayanIntegrationManager.QueueCreateConsignmentNote(eventData.Entity.Id));
                }
                else
                {
                    AsyncHelper.RunSync(() => _bayanIntegrationManager.QueueEditConsignmentNote(eventData.Entity.Id));
                }
            }
        }

        // todo add localization for notifications messages 
    }
}