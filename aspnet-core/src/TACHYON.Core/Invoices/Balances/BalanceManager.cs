using Abp.Application.Features;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Net.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Configuration;
using TACHYON.Features;
using TACHYON.Invoices.Periods;
using TACHYON.MultiTenancy;
using TACHYON.Net.Emailing;
using TACHYON.Notifications;

namespace TACHYON.Invoices.Balances
{
  public  class BalanceManager : TACHYONDomainServiceBase
    {
        private decimal TaxVat;
        private readonly ISettingManager _settingManager;
        private readonly IRepository<Tenant> _Tenant;
        private readonly IFeatureChecker _featureChecker;
        private readonly IAppNotifier _appNotifier;
        private readonly IEmailTemplateProvider _emailTemplateProvider;
        private readonly IEmailSender _emailSender;

        public BalanceManager(
            ISettingManager settingManager,
            IRepository<Tenant> Tenant, 
            IFeatureChecker featureChecker,
            IAppNotifier appNotifier,
            IEmailTemplateProvider emailTemplateProvider,
             IEmailSender emailSender)
        {
            _settingManager = settingManager;
            _Tenant = Tenant;
            _featureChecker = featureChecker;
            TaxVat =  _settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat);
            _appNotifier = appNotifier;
            _emailTemplateProvider = emailTemplateProvider;
            _emailSender = emailSender;
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

        public async Task CheckShipperOverLimit(Tenant Tenant)
        {
            if (Tenant.CreditBalance < 0)
            {
                decimal CurrentBalance = Tenant.CreditBalance * -1;
               
                decimal ShipperCreditLimit = decimal.Parse(await _featureChecker.GetValueAsync(Tenant.Id, AppFeatures.ShipperCreditLimit));
                var percentge =(int) Math.Ceiling((CurrentBalance / ShipperCreditLimit) * 100);
                if (percentge>70)
                {
                  await  _appNotifier.ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage(Tenant.Id, percentge);
                  await  _emailSender.SendAsync("abdullah",L("EmailSubjectShipperCreditLimit"), _emailTemplateProvider.ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage(Tenant.Id, percentge), true);
                }

            }
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
