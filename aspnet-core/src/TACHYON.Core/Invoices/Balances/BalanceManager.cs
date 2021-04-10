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
using System.Linq;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Authorization.Users;
using Abp.UI;

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
        private readonly IRepository<InvoiceProforma,long> _InvoicesProformarepository;
        private readonly UserManager _userManager;


        public BalanceManager(
            ISettingManager settingManager,
            IRepository<Tenant> Tenant, 
            IFeatureChecker featureChecker,
            IAppNotifier appNotifier,
            IEmailTemplateProvider emailTemplateProvider,
             IEmailSender emailSender,
             IRepository<InvoiceProforma, long> InvoicesProformarepository,
             UserManager userManager)
        {
            _settingManager = settingManager;
            _Tenant = Tenant;
            _featureChecker = featureChecker;
            TaxVat =  _settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat);
            _appNotifier = appNotifier;
            _emailTemplateProvider = emailTemplateProvider;
            _emailSender = emailSender;
            _InvoicesProformarepository = InvoicesProformarepository;
            _userManager = userManager;
        }
        #region Shipper

        /// <summary>
        /// When shipper rejected the shipping request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task ChangeShipperBalanceWhenRequestRejected(ShippingRequest request)
        {
            var PeriodType = (InvoicePeriodType)byte.Parse(await _featureChecker.GetValueAsync(request.TenantId, AppFeatures.ShipperPeriods));

            var Tenant = await GetTenant(request.TenantId);

            if (PeriodType == InvoicePeriodType.PayInAdvance)
            {
                Tenant.ReservedBalance -= request.Price.Value;// CalculateAmountWithVat((decimal)request.Price).TotalAmount;
                var InvoiceProformare = await _InvoicesProformarepository.SingleAsync(i => i.TenantId == request.TenantId && i.RequestId== request.Id);
                if (InvoiceProformare != null)
                {
                    await _InvoicesProformarepository.DeleteAsync(InvoiceProformare);
                }
            }
            else
            {
                Tenant.CreditBalance += request.Price.Value;// CalculateAmountWithVat((decimal)request.Price).TotalAmount;

            }
        }
        /// <summary>
        /// Check if shipper can create new shipping  if the credit balance greater or equal credit limit
        /// </summary>
        /// <param name="ShipperTenantId"></param>
        /// <returns></returns>
        public async Task<bool> ShipperCanCreateRequest(int ShipperTenantId)
        {
            var PeriodType = (InvoicePeriodType)byte.Parse(await _featureChecker.GetValueAsync(ShipperTenantId, AppFeatures.ShipperPeriods));
            if (PeriodType == InvoicePeriodType.PayInAdvance) return true;
            var Tenant = await GetTenant(ShipperTenantId);
            decimal CreditLimit = decimal.Parse(await _featureChecker.GetValueAsync(ShipperTenantId, AppFeatures.ShipperCreditLimit)) * -1;
            return Tenant.CreditBalance >= CreditLimit;
        }
        /// <summary>
        /// Check if the credit balance for shipper can accept the shipping request or not.
        /// </summary>
        /// <param name="ShipperTenantId"></param>
        /// <param name="Price"></param>
        /// <returns></returns>
        public async Task ShipperCanAcceptPrice(int TenantId,decimal Price,long shippingRequestId)
        {
           
            var PeriodType = (InvoicePeriodType)byte.Parse(await _featureChecker.GetValueAsync(TenantId, AppFeatures.ShipperPeriods));
            var Tenant = await GetTenant(TenantId);
            if (PeriodType == InvoicePeriodType.PayInAdvance ) 
            {
                if (!await CheckShipperCanPaidFromBalance(TenantId,Price)) throw new UserFriendlyException(L("ThereIsNoBalanceToPaidThisShippiment"));
                await ShipperWhenCanAcceptPrice(TenantId,Price, shippingRequestId);
            }
            else
            {
                decimal CreditLimit = decimal.Parse(await _featureChecker.GetValueAsync(TenantId, AppFeatures.ShipperCreditLimit)) * -1;
                decimal CreditBalance = Tenant.CreditBalance - Price;
                if (!(CreditBalance > CreditLimit)) throw new UserFriendlyException(L("YouDoNoHaveEnoughCredit<nYourCreditCard"));
            }
        }
        /// <summary>
        /// Shipper after accept price
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task ShipperWhenCanAcceptPrice(int TenantId, decimal Price,long shippingRequestId)
        {
            var PeriodType = (InvoicePeriodType)byte.Parse(await _featureChecker.GetValueAsync(TenantId, AppFeatures.ShipperPeriods));
            var Tenant = await GetTenant(TenantId);
           // var InvoiceAmount =  this.CalculateAmountWithVat((decimal)request.Price);
            
            if (PeriodType == InvoicePeriodType.PayInAdvance)
            {
             await   _InvoicesProformarepository.InsertAsync(new InvoiceProforma // Generate Invoice proforma when the shipper billing interval is pay in advance
             {
                  TenantId= Tenant.Id,
                  Amount= Price,
                 /* Amount = InvoiceAmount.Amount,
                  VatAmount= InvoiceAmount.VatAmount,
                  TaxVat= InvoiceAmount.TaxVat,
                  TotalAmount= InvoiceAmount.TotalAmount,*/
                  RequestId = shippingRequestId
             });
                Tenant.ReservedBalance += Price;// InvoiceAmount.TotalAmount;
            }
            else
            {
                Tenant.CreditBalance -= Price;// InvoiceAmount.TotalAmount;
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
               
                decimal ShipperCreditLimit = decimal.Parse(await _featureChecker.GetValueAsync(Tenant.Id, AppFeatures.ShipperCreditLimit));
                var percentge =(int) Math.Ceiling((CurrentBalance / ShipperCreditLimit) * 100);
                if (percentge>70)
                {
                  var user = await _userManager.GetAdminByTenantIdAsync(Tenant.Id);
                  await  _appNotifier.ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage(Tenant.Id, percentge);
                  await  _emailSender.SendAsync(user.EmailAddress, L("EmailSubjectShipperCreditLimit"), _emailTemplateProvider.ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage(Tenant.Id, percentge), true);
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

        public async Task RemoveBalanceFromCarrier(int CarrierTenantId, decimal Price)
        {
            var Tenant = await GetTenant(CarrierTenantId);
            Tenant.Balance -= Price; //CalculateAmountWithVat(Price).TotalAmount;
        }
        #endregion

        #region Heleper
        //public InvoiceAmount CalculateAmountWithVat(decimal amount)
        //{
        //    decimal VatAmount = Math.Round((amount * TaxVat) * 100, 2);
        //    return new InvoiceAmount(TaxVat, amount, VatAmount, amount + VatAmount);
           
        //}
        private async Task<Tenant> GetTenant(int TenantId)
        {
            return await _Tenant.SingleAsync(t => t.Id == TenantId);

        }

        #endregion



    }
}
