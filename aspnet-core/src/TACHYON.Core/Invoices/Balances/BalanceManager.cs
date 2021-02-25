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


        public async Task ChangeShipperBalanceWhenRequestRejected(ShippingRequest request)
        {
            var PeriodType = (InvoicePeriodType)byte.Parse(await _featureChecker.GetValueAsync(request.TenantId, AppFeatures.ShipperPeriods));

            var Tenant = await GetTenant(request.TenantId);

            if (PeriodType == InvoicePeriodType.PayInAdvance)
            {
                Tenant.ReservedBalance -= CalculateAmountWithVat((decimal)request.Price).TotalAmount;
                var InvoiceProformare = await _InvoicesProformarepository.SingleAsync(i => i.TenantId == request.TenantId && i.RequestId== request.Id);
                if (InvoiceProformare != null)
                {
                    await _InvoicesProformarepository.DeleteAsync(InvoiceProformare);
                }
            }
            else
            {
                Tenant.CreditBalance += CalculateAmountWithVat((decimal)request.Price).TotalAmount;

            }
        }
        public async Task<bool> ShipperCanCreateRequest(int ShipperTenantId)
        {
            var PeriodType = (InvoicePeriodType)byte.Parse(await _featureChecker.GetValueAsync(ShipperTenantId, AppFeatures.ShipperPeriods));
            if (PeriodType == InvoicePeriodType.PayInAdvance) return true;
            var Tenant = await GetTenant(ShipperTenantId);
            decimal CreditLimit = decimal.Parse(await _featureChecker.GetValueAsync(ShipperTenantId, AppFeatures.ShipperCreditLimit)) * -1;
            return Tenant.CreditBalance > CreditLimit;
        }

        public async Task<bool> ShipperCanAcceptPrice(int ShipperTenantId, decimal Price)
        {
            var PeriodType = (InvoicePeriodType)byte.Parse(await _featureChecker.GetValueAsync(ShipperTenantId, AppFeatures.ShipperPeriods));
            var Tenant = await GetTenant(ShipperTenantId);
            if (PeriodType == InvoicePeriodType.PayInAdvance) 
            {
                return await CheckShipperCanPaidFromBalance(ShipperTenantId, Price);
            }
            else
            {
                decimal CreditLimit = decimal.Parse(await _featureChecker.GetValueAsync(ShipperTenantId, AppFeatures.ShipperCreditLimit)) * -1;
                decimal CreditBalance = Tenant.CreditBalance - Price;
                return (CreditBalance > CreditLimit);
            }
        }

        public async Task ShipperWhenCanAcceptPrice(ShippingRequest request)
        {
            var PeriodType = (InvoicePeriodType)byte.Parse(await _featureChecker.GetValueAsync(request.TenantId, AppFeatures.ShipperPeriods));
            var Tenant = await GetTenant(request.TenantId);
            var InvoiceAmount = this.CalculateAmountWithVat((decimal)request.Price);
            
            if (PeriodType == InvoicePeriodType.PayInAdvance)
            {
             await   _InvoicesProformarepository.InsertAsync(new InvoiceProforma
                {
                  TenantId= Tenant.Id,
                  Amount = InvoiceAmount.Amount,
                  VatAmount= InvoiceAmount.VatAmount,
                  TaxVat= InvoiceAmount.TaxVat,
                  TotalAmount= InvoiceAmount.TotalAmount,
                  RequestId= request.Id
             });
                Tenant.ReservedBalance += InvoiceAmount.TotalAmount;
            }
            else
            {
                Tenant.CreditBalance -= InvoiceAmount.TotalAmount;
            }
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
                  var user = await _userManager.GetAdminByTenantIdAsync(Tenant.Id);
                  await  _appNotifier.ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage(Tenant.Id, percentge);
                  await  _emailSender.SendAsync(user.EmailAddress, L("EmailSubjectShipperCreditLimit"), _emailTemplateProvider.ShipperNotfiyWhenCreditLimitGreaterOrEqualXPercentage(Tenant.Id, percentge), true);
                }

            }
        }
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

        public async Task<bool> CheckShipperCanPaidFromBalance(int ShipperTenantId, decimal Amount)
        {
            var Tenant = await GetTenant(ShipperTenantId);
            //Amount += _InvoicesProformarepository.GetAll().Where(i => i.TenantId == ShipperTenantId).Sum(i => i.Amount);
            var Balance = Tenant.Balance - Tenant.ReservedBalance;
            return Balance >= Amount;
        }
        #endregion

        #region Carrier
        public async Task AddBalanceToCarrier(int CarrierTenantId, decimal Price)
        {
            var Tenant = await GetTenant(CarrierTenantId);
            Tenant.Balance += CalculateAmountWithVat(Price).TotalAmount;
        }

        public async Task RemoveBalanceToCarrier(int CarrierTenantId, decimal Price)
        {
            var Tenant = await GetTenant(CarrierTenantId);
            Tenant.Balance -= CalculateAmountWithVat(Price).TotalAmount;
        }
        #endregion

        #region Heleper
        public InvoiceAmount CalculateAmountWithVat(decimal amount)
        {
            decimal VatAmount = Math.Round((amount * TaxVat) * 100, 2);
            return new InvoiceAmount(TaxVat, amount, VatAmount, amount + VatAmount);
           
        }
        private async Task<Tenant> GetTenant(int TenantId)
        {
            return await _Tenant.SingleAsync(t => t.Id == TenantId);

        }

        #endregion



    }
}
