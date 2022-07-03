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
            commestionValues.ItemPrice = amount;
            await InitPenalty(PenaltyType.TripCancelation, tenantId, destinationTenantId, tripId, commestionValues);
        }
        public async Task ApplyDetentionPenalty(int tenantId, int destinationTenantId, DateTime arriveTime,int tripId, string facilityName,long? waybillNumber)
        {
            DateTime leaveTime = Clock.Now;
            var allowedHouers = Convert.ToInt32(await _featureChecker.GetValueAsync(tenantId, AppFeatures.AllowedDetentionPeriod));
            var violatedHours = Convert.ToDecimal((arriveTime - leaveTime).TotalHours);
            violatedHours -= allowedHouers;

            if (allowedHouers < violatedHours)
            {
                var price = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.DetentionFeesIncreaseRate));
                var minAmount = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.BaseDetentionFeesAmount));
                var maxAmount = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.MaxDetentionFeesAmount));

                decimal amount = violatedHours * price;

                amount = CheckMinMaxAmount(minAmount, maxAmount, amount);

                var commestionValues = await CalculateCommestions(tenantId, amount,
                AppFeatures.DetentionCommissionType, AppFeatures.DetentionCommissionMinValue,
                AppFeatures.DetentionCommissionPercentage, AppFeatures.DetentionCommissionValue);
                commestionValues.ItemPrice = price;
                await InitPenalty(PenaltyType.DetentionPeriodExceedMaximumAllowedTime, tenantId, destinationTenantId, tripId, commestionValues);
                await _appNotifier.NotifyShipperWhenApplyDetention(tenantId, facilityName, waybillNumber.ToString(), commestionValues.TotalAmount(), tripId);
            }

        }

        public async Task ApplyNotAssigningTruckAndDriverPenalty(int tenantId, int destinationTenantId, DateTime startTripDate, int tripId)
        {
            var amount = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotAssignTruckAndDriverStartDate_Amount));
            var startAmount = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotAssignTruckAndDriverStartDate_StartingAmount));
            var maxAmount = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotAssignTruckAndDriverStartDate_MaximumAmount));
            var unitOfMeasure = (UnitOfMeasure)Convert.ToInt32(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotAssignTruckAndDriverStartDate_UnitsOfMeasure));
            var numberUnitOfMeasure = Convert.ToInt32(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotAssignTruckAndDriverStartDate_NumberOfUnitsOfMeasure));
            var numberOfDelay = (decimal)GetDelayBasedOnUnitOfMeasure(startTripDate, unitOfMeasure);
            numberOfDelay -= numberUnitOfMeasure;

            if (numberOfDelay > 0)
            {
                var finalAmount = numberOfDelay * amount;

                finalAmount = CheckMinMaxAmount(startAmount, maxAmount, finalAmount);

                var commestionValues = await CalculateCommestions(tenantId, finalAmount,
                AppFeatures.NotAssignTruckAndDriverStartDate_CommissionType,
                AppFeatures.NotAssignTruckAndDriverStartDate_CommissionMinValue,
                AppFeatures.NotAssignTruckAndDriverStartDate_CommissionPercentage,
                AppFeatures.NotAssignTruckAndDriverStartDate_CommissionValue);
                commestionValues.ItemPrice = amount;
                await InitPenalty(PenaltyType.NotAssigningTruckAndDriverBeforeTheDateForTheTrip, tenantId, destinationTenantId, tripId, commestionValues);
            }

        }
        public async Task ApplyNotDeliveringAllDropsPenalty(int tenantId, int destinationTenantId, DateTime endTripDate, int tripId)
        {
            var amount = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotDeliveringAllDropsBeforeEndDate_Amount));
            var startAmount = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotDeliveringAllDropsBeforeEndDate_StartingAmount));
            var maxAmount = Convert.ToDecimal(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotDeliveringAllDropsBeforeEndDate_MaximumAmount));
            var unitOfMeasure = (UnitOfMeasure)Convert.ToInt32(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotDeliveringAllDropsBeforeEndDate_UnitsOfMeasure));
            var numberUnitOfMeasure = Convert.ToInt32(await _featureChecker.GetValueAsync(tenantId, AppFeatures.NotDeliveringAllDropsBeforeEndDate_NumberOfUnitsOfMeasure));
            var numberOfDelay = (decimal)GetDelayBasedOnUnitOfMeasure(endTripDate, unitOfMeasure);
            numberOfDelay -= numberUnitOfMeasure;
                if (numberOfDelay > 0)
            {
                var finalAmount = numberOfDelay * amount;

                finalAmount = CheckMinMaxAmount(startAmount, maxAmount, finalAmount);

                var commestionValues = await CalculateCommestions(tenantId, finalAmount,
               AppFeatures.NotDeliveringAllDropsBeforeEndDate_CommissionType, AppFeatures.NotDeliveringAllDropsBeforeEndDate_CommissionMinValue,
               AppFeatures.NotDeliveringAllDropsBeforeEndDate_CommissionPercentage, AppFeatures.NotDeliveringAllDropsBeforeEndDate_CommissionValue);

                commestionValues.ItemPrice = amount;
                await InitPenalty(PenaltyType.NotDeliveringAllDropsBeforeExpectedTripEndDate, tenantId, destinationTenantId, tripId, commestionValues);
            }
        }
        #endregion

        #region Detention Notfications
        public async Task SendNotficationBeforeViolateDetention(int shipperTenantId, long pointId)
        {
            var routPoint = await _routPointrepository
                .GetAllIncluding(c => c.ShippingRequestTripFk)
                .Where(x => x.Id == pointId)
                .FirstOrDefaultAsync();

            if (routPoint.Status == RoutePointStatus.ArrivedToDestination
                || routPoint.Status == RoutePointStatus.StartLoading
                || routPoint.Status == RoutePointStatus.ArriveToLoadingLocation
                || routPoint.Status == RoutePointStatus.StartOffloading)
            {
                await _appNotifier.NotifyShipperBeforApplyDetention(shipperTenantId, routPoint.ShippingRequestTripFk.WaybillNumber.ToString(), routPoint.ShippingRequestTripId);
            }
        }
        public async Task NotficationBeforeViolateDetention(int shipperTenantId, long pointId)
        {
            var allowedDelay = Convert.ToInt32(await _featureChecker.GetValueAsync(shipperTenantId, AppFeatures.AllowedDetentionPeriod));

            await _backgroundJobManager.EnqueueAsync<NotficationBeforeViolateDetention, (int shipperId, long pointId)>((shipperTenantId, pointId), delay: new TimeSpan(allowedDelay, -15, 00));
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

            return CalculateValues(amount, commestionType, commestionMinValue, commestionPercentage, commestionValue, taxVat);
        }

        public  PenaltyCommestionDto CalculateValues(decimal amount, PriceOfferCommissionType commestionType, decimal commestionMinValue, decimal commestionPercentage, decimal commestionValue, decimal taxVat)
        {
            var res = new PenaltyCommestionDto();
            res.AmountPreCommestion = amount;
            res.CommissionType = commestionType;
            res.VatAmount = Calculate.CalculateVat(res.AmountPreCommestion, taxVat);
            res.TaxVat = taxVat;
            
            switch (commestionType)
            {
                case PriceOfferCommissionType.CommissionPercentage:
                    res.CommissionValue = amount * (commestionPercentage / 100);
                    amount += res.CommissionValue;                    
                    
                    break;

                case PriceOfferCommissionType.CommissionValue:
                    res.CommissionValue = commestionValue;
                    amount += commestionValue;
                    break;

                case PriceOfferCommissionType.CommissionMinimumValue:
                    res.CommissionValue = commestionMinValue;
                    amount += commestionMinValue;
                    break;

                default:
                    break;
            }
            res.AmountPostCommestion = amount;
            res.VatPostCommestion = Calculate.CalculateVat(res.AmountPostCommestion, taxVat);

            return res;
        }

        private async Task InitPenalty(PenaltyType penaltyType, int tenantId, int destinationTenantId, int? tripId, PenaltyCommestionDto commestion)
        {
            if (!(await _featureChecker.IsEnabledAsync(tenantId, AppFeatures.Carrier) || await _featureChecker.IsEnabledAsync(tenantId, AppFeatures.Shipper)))
                throw new UserFriendlyException(L("YouShouldAddPenaltyToShipperOrCarrier"));

            var penalty = new Penalty
            {
                TotalAmount = commestion.AmountPostCommestion,
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
                VatAmount = commestion.VatAmount,
                VatPostCommestion = commestion.VatPostCommestion,
                //VatPreCommestion = commestion.VatPreCommestion,
                ItmePrice = commestion.ItemPrice,
                TaxVat=commestion.TaxVat
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
        private static decimal CheckMinMaxAmount(decimal minAmount, decimal maxAmount, decimal amount)
        {
            if (amount < minAmount)
                amount = minAmount;

            if (amount > maxAmount)
                amount = maxAmount;

            return amount;
        }
        #endregion
    }
}
