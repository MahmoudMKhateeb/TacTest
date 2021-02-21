using Abp.Application.Features;
using Abp.Configuration;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Configuration;
using TACHYON.Features;
using TACHYON.Invoices.Periods;
using TACHYON.MultiTenancy;

namespace TACHYON.Invoices.Balances
{
  public  class BalanceManager : TACHYONDomainServiceBase
    {
        private decimal TaxVat;
        private readonly ISettingManager _settingManager;
        private readonly IRepository<Tenant> _Tenant;
        private readonly IFeatureChecker _featureChecker;


        public BalanceManager(ISettingManager settingManager, IRepository<Tenant> Tenant, IFeatureChecker featureChecker)
        {
            _settingManager = settingManager;
            _Tenant = Tenant;
            _featureChecker = featureChecker;
            TaxVat =  _settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat);
        }
        #region Shipper
        public async Task ChangeShipperBalanceWhenPriceRequestApprove(int ShipperTenantId, decimal Price)
        {
            var Tenant = await GetTenant(ShipperTenantId);
            Tenant.Balance -= CalculateAmountWithVat(Price);
            Tenant.CreditBalance -= CalculateAmountWithVat(Price);
        }

        public async Task ChangeShipperBalanceWhenRequestRejected(int ShipperTenantId, decimal Price)
        {
            var Tenant = await GetTenant(ShipperTenantId);
            Tenant.Balance += CalculateAmountWithVat(Price);
            Tenant.CreditBalance += CalculateAmountWithVat(Price);
        }


        public async Task<bool> ShipperCanAcceptPrice(int ShipperTenantId, decimal Price)
        {
            var PeriodType = (InvoicePeriodType)byte.Parse(await _featureChecker.GetValueAsync(ShipperTenantId, AppFeatures.ShipperPeriods));
            if (PeriodType == InvoicePeriodType.PayInAdvance) return true;
            var Tenant = await GetTenant(ShipperTenantId);
            if (Tenant.Balance >= Price) return true;
            decimal CreditLimit = decimal.Parse(await _featureChecker.GetValueAsync(ShipperTenantId, AppFeatures.ShipperCreditLimit)) * -1;
            decimal CurrentBalance = Tenant.Balance - Price;
            if (CurrentBalance < CreditLimit) return false;
            return true;
        }

        public async Task AddBalanceToShipper(int ShipperTenantId, decimal Amount)
        {

            var Tenant = await GetTenant(ShipperTenantId);
            Tenant.Balance += Amount;

        }

        #endregion

        #region Carrier
        public async Task AddBalanceToCarrier(int CarrierTenantId, decimal Price)
        {
            var Tenant = await GetTenant(CarrierTenantId);
            Tenant.Balance += CalculateAmountWithVat(Price);
        }

        public async Task RemoveBalanceToCarrier(int CarrierTenantId, decimal Price)
        {
            var Tenant = await GetTenant(CarrierTenantId);
            Tenant.Balance -= CalculateAmountWithVat(Price);
        }
        #endregion

        #region Heleper
        public decimal CalculateAmountWithVat(decimal amount)
        {
            return Math.Round((amount * TaxVat) * 100, 2);
        }
        private async Task<Tenant> GetTenant(int TenantId)
        {
            return await _Tenant.SingleAsync(t => t.Id == TenantId);

        }

        #endregion



    }
}
