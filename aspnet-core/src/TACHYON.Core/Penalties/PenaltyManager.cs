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

        #region Applying Penalties
        public async Task ApplyTripCancelationPenalty(int tenantId, long tripId)
        {
            var amount = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.TripCancelation));
            await InitPenalty(amount, PenaltyType.TripCancelation, tenantId, tripId, null);
        }
        public async Task ApplyDetentionPenalty(int tenantId, DateTime arriveTime, DateTime leaveTime, int tripId)
        {
            var allowedHouers = Convert.ToInt32(await _featureChecker.GetValueAsync(tenantId, AppFeatures.AllowedDetentionPeriod));
            var violatedHours = (leaveTime - arriveTime);

            if (allowedHouers > violatedHours.Hours)
            {
                var price = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.DetentionFeesIncreaseRate));
                var minAmount = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.BaseDetentionFeesAmount));
                var maxAmount = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.MaxDetentionFeesAmount));
                decimal amount = violatedHours.Hours * price;

                if (amount < minAmount)
                    amount = minAmount;

                if (amount > maxAmount)
                    amount = maxAmount;

                await InitPenalty(amount, PenaltyType.DetentionPeriodExceedMaximumAllowedTime, tenantId, tripId, null);
            }

        }
        public async Task ApplyNotAssigningTruckAndDriverPenalty(int tenantId, DateTime startTripDate, int tripId)
        {
            var amount = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotAssignTruckAndDriverStartDate_Amount));
            var unitOfMeasure = (UnitOfMeasure)Convert.ToInt32(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotAssignTruckAndDriverStartDate_UnitsOfMeasure));
            var numberUnitOfMeasure = Convert.ToInt32(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotAssignTruckAndDriverStartDate_NumberOfUnitsOfMeasure));
            var numberOfDelay = GetDelayBasedOnUnitOfMeasure(startTripDate, unitOfMeasure);
            if (numberOfDelay > 0)
            {
                var finalAmount = (numberOfDelay - numberUnitOfMeasure) * amount;
                await InitPenalty(finalAmount, PenaltyType.DetentionPeriodExceedMaximumAllowedTime, tenantId, tripId, null);
            }

        }
        public async Task ApplyNotDeliveringAllDropsPenalty(int tenantId, DateTime startTripDate, int tripId)
        {
            var amount = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotDeliveringAllDropsBeforeEndDate_Amount));
            var unitOfMeasure = (UnitOfMeasure)Convert.ToInt32(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotDeliveringAllDropsBeforeEndDate_UnitsOfMeasure));
            var numberUnitOfMeasure = Convert.ToInt32(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotDeliveringAllDropsBeforeEndDate_NumberOfUnitsOfMeasure));
            var numberOfDelay = GetDelayBasedOnUnitOfMeasure(startTripDate, unitOfMeasure);
            if (numberOfDelay > 0)
            {
                var finalAmount = (numberOfDelay - numberUnitOfMeasure) * amount;
                await InitPenalty(finalAmount, PenaltyType.DetentionPeriodExceedMaximumAllowedTime, tenantId, tripId, null);
            }
        }
        #endregion

        #region Detention Notfications
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
        #endregion

        #region Helpers 
        private async Task InitPenalty(decimal amount, PenaltyType penaltyType, int tenantId, long? tripId, long? pointId)
        {
            if (!(await _featureChecker.IsEnabledAsync(tenantId, AppFeatures.Carrier) || await _featureChecker.IsEnabledAsync(tenantId, AppFeatures.Shipper)))
                throw new UserFriendlyException(L("YouShouldAddPenaltyToShipperOrCarrier"));

            var penalty = new Penalty
            {
                TotalAmount = amount,
                TenantId = tenantId,
                Type = penaltyType,
                PointId = pointId,
                TripId = tripId,
                PenaltyName = penaltyType.ToString(),
                PenaltyDescrption = penaltyType.ToString()
            };

            await _penaltyRepository.InsertAsync(penalty);
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

        #endregion
    }
}
