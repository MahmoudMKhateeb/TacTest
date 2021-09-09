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
        private readonly IFirebaseNotifier _firebaseNotifier;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<User, long> _lookupUserRepo;
        private readonly IRepository<Tenant> _lookupTenantRepo;

        public TripUpdatedEventHandler(BayanIntegrationManager bayanIntegrationManager, IAppNotifier appNotifier, IFirebaseNotifier firebaseNotifier, IUnitOfWorkManager unitOfWorkManager,
            IRepository<User, long> lookupUserRepo, IRepository<Tenant> lookupTenantRepo)
        {
            _bayanIntegrationManager = bayanIntegrationManager;
            _appNotifier = appNotifier;
            _firebaseNotifier = firebaseNotifier;
            _unitOfWorkManager = unitOfWorkManager;
            _lookupUserRepo = lookupUserRepo;
            _lookupTenantRepo = lookupTenantRepo;
        }

        public void HandleEvent(EntityUpdatedEventData<ShippingRequestTrip> eventData)
        {
            //todo send notification for driver and shipper and carrier using eventBus

            AsyncHelper.RunSync(async () => await NotifyTripUpdated(eventData.Entity));

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
        private async Task NotifyTripUpdated(ShippingRequestTrip trip)
        {
            #region AllRequiredData

            var shipperTenantId = trip.ShippingRequestFk.TenantId;
            var carrierTenantId = trip.ShippingRequestFk.CarrierTenantId;
            string tenantName;
            var waybillNo = trip.WaybillNumber;
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                tenantName = await (from user in _lookupUserRepo.GetAll()
                                    where user.Id == trip.LastModifierUserId
                                    join tenant in _lookupTenantRepo.GetAll() on user.TenantId equals tenant.Id
                                    select tenant.companyName).FirstOrDefaultAsync();

                /*    tenantName = await _lookupTenantRepo.GetAll()
                            .Where(x => x.Id == trip.ShippingRequestFk.TenantId)
                            .Select(x => x.TenancyName).FirstOrDefaultAsync();*/

                await unitOfWork.CompleteAsync();
            }

            var input = new NotifyTripUpdatedInput()
            {
                ShipperTenantId = shipperTenantId,
                TripId = trip.Id,
                WaybillNumber = waybillNo.ToString(),
                UpdatedBy = tenantName
            };

            #endregion

            if (carrierTenantId is null)
            {
                await _appNotifier.NotifyShipperWhenTripUpdated(input);
                await _appNotifier.NotifyTachyonDealWhenTripUpdated(input);
            }
            else
            {
                input.CarrierTenantId = carrierTenantId.Value;

                await _appNotifier.NotifyAllWhenTripUpdated(input);

                var driverId = trip.AssignedDriverUserId;
                if (driverId is null || trip.DriverStatus != ShippingRequestTripDriverStatus.Accepted) return;

                var driverIdentifier = new UserIdentifier(carrierTenantId, driverId.Value);
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    input.DriverIdentifier = driverIdentifier;
                    await _firebaseNotifier.TripUpdated(input);
                    await unitOfWork.CompleteAsync();
                }
            }
        }
    }
}