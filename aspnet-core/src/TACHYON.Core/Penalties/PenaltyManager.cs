using Abp.Application.Features;
using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Common;
using TACHYON.Configuration;
using TACHYON.Features;
using TACHYON.Notifications;
using TACHYON.Penalties;
using TACHYON.Penalties.Dto;
using TACHYON.Penalties.Jobs;
using TACHYON.Penalties.UnitOfMeasures;
using TACHYON.PriceOffers;
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
        private readonly ISettingManager _settingManager;

        public PenaltyManager(IRepository<Penalty> penaltyRepository,
            IFeatureChecker featureChecker,
            IAppNotifier appNotifier,
            IRepository<RoutPoint, long> routPointrepository,
            IBackgroundJobManager backgroundJobManager,
            ISettingManager settingManager)
        {
            _penaltyRepository = penaltyRepository;
            _featureChecker = featureChecker;
            _appNotifier = appNotifier;
            _routPointrepository = routPointrepository;
            _backgroundJobManager = backgroundJobManager;
            _settingManager = settingManager;
        }

        #region Applying Penalties
        public async Task ApplyTripCancelationPenalty(int tenantId, int destinationTenantId, int tripId)
        {
            var amount = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.TripCancelation));
            var commestionValues = await CalculateCommestions(tenantId, amount,
                AppFeatures.TripCancelationCommissionType, AppFeatures.TripCancelationCommissionMinValue,
                AppFeatures.TripCancelationCommissionPercentage, AppFeatures.TripCancelationCommissionValue);

            await InitPenalty(PenaltyType.TripCancelation, tenantId, destinationTenantId, tripId, commestionValues);
        }
        public async Task ApplyDetentionPenalty(int tenantId, int destinationTenantId, DateTime arriveTime, DateTime leaveTime, int tripId)
        {
            var allowedHouers = Convert.ToInt32(await _featureChecker.GetValueAsync(tenantId, AppFeatures.AllowedDetentionPeriod));
            var violatedHours = (leaveTime - arriveTime);

            if (allowedHouers < violatedHours.Hours)
            {
                var price = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.DetentionFeesIncreaseRate));
                var minAmount = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.BaseDetentionFeesAmount));
                var maxAmount = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.MaxDetentionFeesAmount));

                decimal amount = violatedHours.Hours * price;

                if (amount < minAmount)
                    amount = minAmount;

                if (amount > maxAmount)
                    amount = maxAmount;

                var commestionValues = await CalculateCommestions(tenantId, amount,
                AppFeatures.DetentionCommissionType, AppFeatures.DetentionCommissionMinValue,
                AppFeatures.DetentionCommissionPercentage, AppFeatures.DetentionCommissionValue);

                await InitPenalty(PenaltyType.DetentionPeriodExceedMaximumAllowedTime, tenantId, destinationTenantId, tripId, commestionValues);
                await _appNotifier.NotifyShipperWhenApplyDetention(tenantId, "", "", commestionValues.TotalAmount(), tripId);
            }

        }
        public async Task ApplyNotAssigningTruckAndDriverPenalty(int tenantId, int destinationTenantId, DateTime startTripDate, int tripId)
        {
            var amount = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotAssignTruckAndDriverStartDate_Amount));
            var unitOfMeasure = (UnitOfMeasure)Convert.ToInt32(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotAssignTruckAndDriverStartDate_UnitsOfMeasure));
            var numberUnitOfMeasure = Convert.ToInt32(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotAssignTruckAndDriverStartDate_NumberOfUnitsOfMeasure));
            var numberOfDelay =(decimal) GetDelayBasedOnUnitOfMeasure(startTripDate, unitOfMeasure);
            if (numberOfDelay > 0)
            {
                var finalAmount = (numberOfDelay - numberUnitOfMeasure) * amount;

                var commestionValues = await CalculateCommestions(tenantId, finalAmount,
                AppFeatures.NotAssignTruckAndDriverStartDate_CommissionType,
                AppFeatures.NotAssignTruckAndDriverStartDate_CommissionMinValue,
                AppFeatures.NotAssignTruckAndDriverStartDate_CommissionPercentage,
                AppFeatures.NotAssignTruckAndDriverStartDate_CommissionValue);

                await InitPenalty(PenaltyType.NotAssigningTruckAndDriverBeforeTheDateForTheTrip, tenantId, destinationTenantId, tripId, commestionValues);
            }

        }
        public async Task ApplyNotDeliveringAllDropsPenalty(int tenantId, int destinationTenantId, DateTime startTripDate, int tripId)
        {
            var amount = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotDeliveringAllDropsBeforeEndDate_Amount));
            var unitOfMeasure = (UnitOfMeasure)Convert.ToInt32(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotDeliveringAllDropsBeforeEndDate_UnitsOfMeasure));
            var numberUnitOfMeasure = Convert.ToInt32(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotDeliveringAllDropsBeforeEndDate_NumberOfUnitsOfMeasure));
            var numberOfDelay = (decimal) GetDelayBasedOnUnitOfMeasure(startTripDate, unitOfMeasure);
            if (numberOfDelay > 0)
            {
                var finalAmount = (numberOfDelay - numberUnitOfMeasure) * amount;
                var commestionValues = await CalculateCommestions(tenantId, finalAmount,
               AppFeatures.NotDeliveringAllDropsBeforeEndDate_CommissionType, AppFeatures.NotDeliveringAllDropsBeforeEndDate_CommissionMinValue,
               AppFeatures.NotDeliveringAllDropsBeforeEndDate_CommissionPercentage, AppFeatures.NotDeliveringAllDropsBeforeEndDate_CommissionValue);

                await InitPenalty(PenaltyType.NotDeliveringAllDropsBeforeExpectedTripEndDate, tenantId, tripId, destinationTenantId,commestionValues);
            }
        }
        #endregion

        #region Detention Notfications
        public async Task SendNotficationBeforeViolateDetention(int shipperTenantId, long pointId)
        {
            var routPoint = await _routPointrepository
                .GetAllIncluding(c=> c.ShippingRequestTripFk)
                .Where(x=> x.Id == pointId)
                .FirstOrDefaultAsync();

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
        #endregion

        #region Helpers 
        private async Task<PenaltyCommestionDto> CalculateCommestions(int tenantId, decimal amount,
            string commestionTypeKey, string commestionMinValueKey,
            string commestionPercentageKey, string commestionValueKey)
        {
            var commestionType = (PriceOfferCommissionType)Convert.ToInt32(await _featureChecker.GetValueAsync(tenantId, commestionTypeKey));
            var commestionMinValue = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, commestionMinValueKey));
            var commestionPercentage = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, commestionPercentageKey));
            var commestionValue = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, commestionValueKey));
            var taxVat = _settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat);

            var res = new PenaltyCommestionDto();
            res.AmountPreCommestion = amount;
            res.CommissionType = commestionType;
            res.VatPreCommestion = Calculate.CalculateVat(res.AmountPreCommestion, taxVat);

            switch (commestionType)
            {
                case PriceOfferCommissionType.CommissionPercentage:
                    res.CommissionValue = amount * (commestionPercentage / 100);
                    amount += res.CommissionValue;
                    res.VatPostCommestion += res.VatPreCommestion * (commestionPercentage / 100);
                    amount += res.CommissionValue;
                    break;

                case PriceOfferCommissionType.CommissionValue:
                    res.CommissionValue = commestionValue;
                    amount += commestionValue;
                    res.VatPostCommestion += res.CommissionValue;
                    break;

                case PriceOfferCommissionType.CommissionMinimumValue:
                    res.CommissionValue = commestionMinValue;
                    amount += commestionMinValue;
                    res.VatPostCommestion += commestionMinValue;
                    break;

                default:
                    break;
            }
            res.AmountPostCommestion = amount;

            return res;
        }
        private async Task InitPenalty(PenaltyType penaltyType, int tenantId, int destinationTenantId, int? tripId, PenaltyCommestionDto commestion)
        {
            if (!(await _featureChecker.IsEnabledAsync(tenantId, AppFeatures.Carrier) || await _featureChecker.IsEnabledAsync(tenantId, AppFeatures.Shipper)))
                throw new UserFriendlyException(L("YouShouldAddPenaltyToShipperOrCarrier"));

            var penalty = new Penalty
            {
                TotalAmount = commestion.AmountPostCommestion + commestion.VatPostCommestion,
                TenantId = tenantId,
                DestinationTenantId = destinationTenantId,
                Type = penaltyType,
                ShippingRequestTripId = tripId,
                PenaltyName = penaltyType.ToString(),
                PenaltyDescrption = penaltyType.ToString(),
                CommissionType = commestion.CommissionType,
                AmountPreCommestion = commestion.AmountPreCommestion,
                AmountPostCommestion = commestion.AmountPostCommestion,
                CommissionValue = commestion.CommissionValue,
                VatAmount = commestion.VatAmount(),
                VatPostCommestion = commestion.VatPostCommestion,
                VatPreCommestion = commestion.VatPreCommestion,
            };

            await _penaltyRepository.InsertAsync(penalty);
        }
        private static double GetDelayBasedOnUnitOfMeasure(DateTime date, UnitOfMeasure unitOfMeasure)
        {
            switch (unitOfMeasure)
            {
                case UnitOfMeasure.Hourly: return (Clock.Now - date).TotalHours;

                case UnitOfMeasure.Daily: return (Clock.Now - date).TotalDays;
            }
            return 0;
        }

        #endregion
    }
}
