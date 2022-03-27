using Abp.Application.Features;
using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Abp.Timing;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Features;
using TACHYON.Notifications;
using TACHYON.Penalties;
using TACHYON.Penalties.Dto;
using TACHYON.Penalties.Jobs;
using TACHYON.Penalties.UnitOfMeasures;
using TACHYON.Routs.RoutPoints;

namespace TACHYON.Penalties
{
    public class PenaltyManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<Penalty> _penaltyRepository;
        private readonly IRepository<RoutPoint, long> _routPointrepository;
        private readonly IFeatureChecker _featureChecker;
        private readonly IAppNotifier _appNotifier;
        private readonly IBackgroundJobManager _backgroundJobManager;
        public PenaltyManager(IRepository<Penalty> penaltyRepository,
            IFeatureChecker featureChecker,
            IAppNotifier appNotifier,
            IRepository<RoutPoint, long> routPointrepository,
            IBackgroundJobManager backgroundJobManager)
        {
            _penaltyRepository = penaltyRepository;
            _featureChecker = featureChecker;
            _appNotifier = appNotifier;
            _routPointrepository = routPointrepository;
            _backgroundJobManager = backgroundJobManager;
        }
        public async Task SendNotficationBeforeViolateDetention(int shipperTenantId, long pointId)
        {
            var routPoint = await _routPointrepository.FirstOrDefaultAsync(pointId);
            if (routPoint.Status == RoutePointStatus.ArrivedToDestination
                || routPoint.Status == RoutePointStatus.ArriveToLoadingLocation)
            {
                await _appNotifier.NotifyShipperBeforApplyDetention(shipperTenantId, routPoint.ShippingRequestTripFk.WaybillNumber.ToString(), routPoint.ShippingRequestTripId);
            }
        }

        public async Task NotficationBeforeViolateDetention(int shipperTenantId, int pointId)
        {
            var allowedDelay = Convert.ToInt32(await _featureChecker.GetValueAsync(shipperTenantId, AppFeatures.AllowedDetentionPeriod));
            int[] args = new int[] { shipperTenantId, pointId };
            await _backgroundJobManager.EnqueueAsync<NotficationBeforeViolateDetention, int[]>(args, delay: new TimeSpan(allowedDelay, -15, 00));
        }
        public async Task NotficationWhenViolateDetention(int shipperTenantId, int pointId)
        {
            var allowedDelay = Convert.ToInt32(await _featureChecker.GetValueAsync(shipperTenantId, AppFeatures.AllowedDetentionPeriod));
            int[] args = new int[] { shipperTenantId, pointId };
            await _backgroundJobManager.EnqueueAsync<NotficationWhenViolateDetention, int[]>(args, delay: new TimeSpan(allowedDelay, 30, 00));
        }
        public async Task InitPenalty(PenaltyType penaltyType, int tenantId, long sourceId, DateTime date)
        {
            if (!(await _featureChecker.IsEnabledAsync(tenantId, AppFeatures.Carrier) || await _featureChecker.IsEnabledAsync(tenantId, AppFeatures.Shipper)))
                throw new UserFriendlyException(L("YouShouldAddPenaltyToShipperOrCarrier"));

            decimal featureAmount = await GetShipperPenaltyAmountByType(penaltyType, tenantId, date);

            if (featureAmount > 0)
            {
                var penalty = new Penalty
                {
                    Amount = featureAmount,
                    TenantId = tenantId,
                    Type = penaltyType,
                    PointId = sourceId,
                    TripId = sourceId,
                    PenaltyName = penaltyType.ToString(),
                    PenaltyDescrption = penaltyType.ToString()
                };
                await _penaltyRepository.InsertAsync(penalty);
            }
        }

        private async Task<decimal> GetShipperPenaltyAmountByType(PenaltyType penaltyType, int tenantId, DateTime startTripDate)
        {
            switch (penaltyType)
            {
                case PenaltyType.ShippingRequestCancelingDuringPostPriceProcess: return await ShippingRequestCancelingDuringPostPriceProcess(tenantId);

                case PenaltyType.ShippingRequestCancelBeforeCompletionTrips: return await ShippingRequestCancelBeforeCompletionTrips(tenantId);

                case PenaltyType.TripCancelingBeforeDeliveringAllDrops: return await TripCancelingBeforeDeliveringAllDrops(tenantId);

                case PenaltyType.NotAssigningTruckAndDriverBeforeTheDateForTheTrip: return await NotAssigningTruckAndDriverBeforeTheDateForTheTrip(tenantId, startTripDate);

                case PenaltyType.NotDeliveringAllDropsBeforeExpectedTripEndDate: return await NotDeliveringAllDropsBeforeExpectedTripEndDate(tenantId, startTripDate);

                default: throw new UserFriendlyException(L("YouShouldAddPenaltyToShipperOrCarrier"));
            }
        }
        private async Task<decimal> ShippingRequestCancelingDuringPostPriceProcess(int tenantId)
        {
            return Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.ShippingRequestCancelBeforeCompletionTripsAmount));
        }
        private async Task<decimal> ShippingRequestCancelBeforeCompletionTrips(int tenantId)
        {
            return Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.ShippingRequestCancelBeforeCompletionTripsAmount));
        }
        private async Task<decimal> TripCancelingBeforeDeliveringAllDrops(int tenantId)
        {
            return Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.TripCancelBeforeDeliveringAllDropsAmount));
        }
        private async Task<decimal> NotAssigningTruckAndDriverBeforeTheDateForTheTrip(int tenantId, DateTime startTripDate)
        {
            var amount = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotAssignTruckAndDriverStartDate_Amount));
            var unitOfMeasure = (UnitOfMeasure)Convert.ToInt32(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotAssignTruckAndDriverStartDate_UnitsOfMeasure));
            var numberUnitOfMeasure = Convert.ToInt32(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotAssignTruckAndDriverStartDate_NumberOfUnitsOfMeasure));
            var numberOfDelay = GetDelayBasedOnUnitOfMeasure(startTripDate, unitOfMeasure);
            return (numberOfDelay - numberUnitOfMeasure) * amount;
        }
        private async Task<decimal> NotDeliveringAllDropsBeforeExpectedTripEndDate(int tenantId, DateTime startTripDate)
        {
            var amount = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotDeliveringAllDropsBeforeEndDate_Amount));
            var unitOfMeasure = (UnitOfMeasure)Convert.ToInt32(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotDeliveringAllDropsBeforeEndDate_UnitsOfMeasure));
            var numberUnitOfMeasure = Convert.ToInt32(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotDeliveringAllDropsBeforeEndDate_NumberOfUnitsOfMeasure));
            var numberOfDelay = GetDelayBasedOnUnitOfMeasure(startTripDate, unitOfMeasure);
            return (numberOfDelay - numberUnitOfMeasure) * amount;
        }

        private static int GetDelayBasedOnUnitOfMeasure(DateTime date, UnitOfMeasure unitOfMeasure)
        {
            switch (unitOfMeasure)
            {
                case UnitOfMeasure.Hourly: return (date - Clock.Now).Hours;

                case UnitOfMeasure.Daily: return (date - Clock.Now).Days;
            }
            return 0;
        }

    }
}
