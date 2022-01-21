using Abp.Application.Features;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Net.Mail;
using Abp.UI;
using System;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.Configuration;
using TACHYON.Features;
using TACHYON.Invoices.Periods;
using TACHYON.MultiTenancy;
using TACHYON.Net.Emailing;
using TACHYON.Notifications;
using TACHYON.PriceOffers;

namespace TACHYON.Invoices.Balances
{
    public class BalanceManager : TACHYONDomainServiceBase
    {
        private decimal TaxVat;
        private readonly ISettingManager _settingManager;
        private readonly IRepository<Tenant> _Tenant;
        private readonly IFeatureChecker _featureChecker;
        private readonly IAppNotifier _appNotifier;
        private readonly IEmailTemplateProvider _emailTemplateProvider;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<InvoiceProforma, long> _InvoicesProformarepository;
        private readonly IRepository<InvoicePeriod> _invoicePeriodRepository;
        private readonly UserManager _userManager;


        public BalanceManager(
            ISettingManager settingManager,
            IRepository<Tenant> Tenant,
            IFeatureChecker featureChecker,
            IAppNotifier appNotifier,
            IEmailTemplateProvider emailTemplateProvider,
            IEmailSender emailSender,
            IRepository<InvoiceProforma, long> InvoicesProformarepository,
            UserManager userManager,
            IRepository<InvoicePeriod> invoicePeriodRepository)
        {
            _settingManager = settingManager;
            _Tenant = Tenant;
            _featureChecker = featureChecker;
            TaxVat = _settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat);
            _appNotifier = appNotifier;
            _emailTemplateProvider = emailTemplateProvider;
            _emailSender = emailSender;
            _InvoicesProformarepository = InvoicesProformarepository;
            _userManager = userManager;
            _invoicePeriodRepository = invoicePeriodRepository;
        }

        #region Shipper

        /// <summary>
        ///  Check if the credit balance for shipper can accept the shipping request or not.
        /// </summary>
        /// <param name="offer"> price offer entity</param>
        /// <returns></returns>
        public async Task ShipperCanAcceptOffer(PriceOffer offer)
        {
            InvoicePeriodType periodType = await GetTenantPeriodType(offer.ShippingRequestFk.TenantId);
            var tenant = offer.ShippingRequestFk.Tenant;
            if (periodType == InvoicePeriodType.PayInAdvance)
            {
                if (!await CheckShipperCanPaidFromBalance(offer.ShippingRequestFk.TenantId, offer.TotalAmount))
                    throw new UserFriendlyException(L("NoEnoughBalance"));
                await ShipperWhenCanAcceptPrice(offer, periodType);
            }
            else
            {
                decimal creditLimit =
                    decimal.Parse(await _featureChecker.GetValueAsync(offer.ShippingRequestFk.TenantId,
                        AppFeatures.ShipperCreditLimit)) * -1;
                decimal creditBalance = tenant.CreditBalance - offer.TotalAmount;
                if (!(creditBalance > creditLimit))
                    throw new UserFriendlyException(L("YouDoNotHaveEnoughCreditInYourCreditCard"));
            }
        }

        private async Task<InvoicePeriodType> GetTenantPeriodType(int tenantId)
        {
            byte shipperPeriodId =
                byte.Parse(await _featureChecker.GetValueAsync(tenantId, AppFeatures.ShipperPeriods));
            var preiod = await _invoicePeriodRepository.GetAsync(shipperPeriodId);
            var periodType = preiod.PeriodType;
            return periodType;
        }

        /// <summary>
        /// If shipper can accept offer then create invoices proformare
        /// </summary>
        /// <param name="offer"></param>
        /// <param name="periodType"></param>
        /// <returns></returns>
        private async Task ShipperWhenCanAcceptPrice(PriceOffer offer, InvoicePeriodType periodType)
        {
            var Tenant = offer.ShippingRequestFk.Tenant;

            if (periodType == InvoicePeriodType.PayInAdvance)
            {
                offer.ShippingRequestFk.IsPrePayed = true;
                await _InvoicesProformarepository.InsertAsync(
                    new InvoiceProforma // Generate Invoice proforma when the shipper billing interval is pay in advance
                    {
                        TenantId = Tenant.Id,
                        Amount = offer.SubTotalAmountWithCommission,
                        TotalAmount = offer.TotalAmountWithCommission,
                        VatAmount = offer.VatAmountWithCommission,
                        TaxVat = offer.TaxVat,
                        RequestId = offer.ShippingRequestId
                    });
                Tenant.ReservedBalance += offer.TotalAmount;
            }
            else
            {
                Tenant.CreditBalance -= offer.TotalAmount;
            }
        }

        /// <summary>
        /// Check if the shipper credit limit over the limit or not
        /// </summary>
        /// <param name="Tenant"></param>
        /// <returns></returns>
        public async Task CheckShipperOverLimit(Tenant Tenant)
        {
            if (Tenant.CreditBalance < 0)
            {
                decimal CurrentBalance = Tenant.CreditBalance * -1;

                decimal ShipperCreditLimit =
                    decimal.Parse(await _featureChecker.GetValueAsync(Tenant.Id, AppFeatures.ShipperCreditLimit));
                var percentge = (int)Math.Ceiling((CurrentBalance / ShipperCreditLimit) * 100);
                if (percentge > 70)
                {
                    var user = await _userManager.GetAdminByTenantIdAsync(Tenant.Id);
                    await _appNotifier.ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage(Tenant.Id, percentge);
                    await _emailSender.SendAsync(user.EmailAddress, L("EmailSubjectShipperCreditLimit"),
                        _emailTemplateProvider.ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage(Tenant.Id,
                            percentge), true);
                }
            }
        }

        /// <summary>
        /// Add balance to shipper when recharge blanace
        /// </summary>
        /// <param name="ShipperTenantId"></param>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public async Task AddBalanceToShipper(int ShipperTenantId, decimal Amount)
        {
            var Tenant = await GetTenant(ShipperTenantId);
            Tenant.Balance += Amount;
        }

        public async Task AddCreditBalanceToShipper(int ShipperTenantId, decimal Amount)
        {
            var Tenant = await GetTenant(ShipperTenantId);
            Tenant.CreditBalance += Amount;
        }

        /// <summary>
        /// Check if the pay in adnavce can shipper pay to shipping request price or not
        /// </summary>
        /// <param name="ShipperTenantId"></param>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public async Task<bool> CheckShipperCanPaidFromBalance(int ShipperTenantId, decimal Amount)
        {
            var Tenant = await GetTenant(ShipperTenantId);
            var Balance = Tenant.Balance - Tenant.ReservedBalance;
            return Balance >= Amount;
        }

        #endregion

        #region Carrier

        public async Task AddBalanceToCarrier(int CarrierTenantId, decimal Price)
        {
            var Tenant = await GetTenant(CarrierTenantId);
            Tenant.Balance += Price; //CalculateAmountWithVat(Price).TotalAmount;
        }


        private async Task<Tenant> GetTenant(int TenantId)
        {
            return await _Tenant.SingleAsync(t => t.Id == TenantId);
        }

        #endregion
    }
}