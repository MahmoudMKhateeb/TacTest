using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Quartz;
using Abp.Timing;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Core.Invoices.Jobs;
using TACHYON.Invoices.Periods;
using TACHYON.Invoices.Groups;
using TACHYON.MultiTenancy;
using TACHYON.Shipping.ShippingRequests;
using System.Collections.Generic;
using Abp.Net.Mail;
using TACHYON.Notifications;
using Abp.Configuration;
using TACHYON.Configuration;
using Abp.Application.Features;
using TACHYON.Features;
using Abp.Domain.Uow;
using Microsoft.EntityFrameworkCore;
using TACHYON.Invoices.Balances;

namespace TACHYON.Invoices
{
    public class InvoiceManager : TACHYONDomainServiceBase
    {
        #region property
        private readonly IRepository<InvoicePeriod> _PeriodRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<InvoiceShippingRequests, long> _invoiceShippingRequestsRepository;
        private readonly IRepository<GroupPeriodInvoice, long> _GroupPeriodInvoiceRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Tenant> _Tenant;
        private readonly IEmailSender _emailSender;
        private readonly IAppNotifier _appNotifier;
        private readonly ISettingManager _settingManager;
        private readonly IQuartzScheduleJobManager _jobManager;
        private readonly IRepository<Invoice, long> _InvoiceRepository;
        private readonly IRepository<GroupPeriod, long> _GroupRepository;
        private readonly IFeatureChecker _featureChecker;
        private readonly BalanceManager _BalanceManager;

        private decimal TaxVat;
        #endregion
        public InvoiceManager(
            IRepository<InvoicePeriod> PeriodRepository,
            IRepository<Invoice, long> InvoiceRepository,
            IRepository<GroupPeriodInvoice, long> GroupPeriodInvoiceRepository,
            IQuartzScheduleJobManager JobManager,
            IEmailSender EmailSender,
            IAppNotifier AppNotifier,
            ISettingManager SettingManager,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IRepository<GroupPeriod, long> GroupRepository,
            IRepository<InvoiceShippingRequests, long> invoiceShippingRequestsRepository,
            IFeatureChecker featureChecker,
            IRepository<Tenant> tenant,
             BalanceManager BalanceManager,
             IUnitOfWorkManager unitOfWorkManager)
        {
            _PeriodRepository = PeriodRepository;
            _InvoiceRepository = InvoiceRepository;
             _GroupPeriodInvoiceRepository = GroupPeriodInvoiceRepository;
            _jobManager = JobManager;
            _emailSender = EmailSender;
            _appNotifier = AppNotifier;
            _settingManager = SettingManager;
            _shippingRequestRepository = shippingRequestRepository;
            _GroupRepository = GroupRepository;
            _featureChecker = featureChecker;
            _Tenant = tenant;
            _invoiceShippingRequestsRepository = invoiceShippingRequestsRepository;
            _BalanceManager = BalanceManager;
            _unitOfWorkManager = unitOfWorkManager;
        }

       // [UnitOfWork]
        public async void RunAllJobs()
        {
            
            TaxVat = GetTax();

            var Results = _PeriodRepository
                .GetAll()
                .WhereIf(true, p => p.Enabled && p.PeriodType != InvoicePeriodType.PayInAdvance && p.PeriodType != InvoicePeriodType.PayuponDelivery);

            foreach (var Period in Results)
            {
                await CreateTiggerAsync(Period);
            }
        }

        public async Task CreateTiggerAsync(InvoicePeriod Period)
        {
            string myJobKey = $"InvoiceJob.[{Enum.GetName(typeof(InvoicePeriodType), Period.PeriodType)}].[{Period.Id}]";



            await _jobManager.ScheduleAsync<InvoiceJob>(
                     job =>
                     {
                         job.WithIdentity($"InvoiceJob.[{Enum.GetName(typeof(InvoicePeriodType), Period.PeriodType)}].[{Period.Id}]")
                             .WithDescription("A job to simply write logs.")
                             .UsingJobData("PeriodType", (int)Period.PeriodType)
                             .UsingJobData("PeriodId", Period.Id)
                             .StoreDurably();

                     },
                     trigger =>
                     {
                         trigger.StartNow()
                         .WithIdentity($"InvoiceTrigger.[{Enum.GetName(typeof(InvoicePeriodType), Period.PeriodType)}].[{Period.Id}]")
                         .WithCronSchedule(Period.Cronexpression)
                         .ForJob(myJobKey);
                     });
        }

        public async Task RemoveTriggerAsync(InvoicePeriod Period)
        {
            string Key = $"InvoiceTrigger.[{Enum.GetName(typeof(InvoicePeriodType), Period.PeriodType)}].[{Period.Id}]";
            var TriggerKey = new TriggerKey(Key);
            await _jobManager.UnscheduleAsync(TriggerKey);
        }


        public async Task UpdateTriggerAsync(InvoicePeriod Period)
        {
            await RemoveTriggerAsync(Period);
            if (Period.Enabled & Period.PeriodType != InvoicePeriodType.PayInAdvance && Period.PeriodType != InvoicePeriodType.PayuponDelivery) await CreateTiggerAsync(Period);


        }

        public void GenerateInvoice(byte PeriodId)
        {
            var Tenants = _Tenant.GetAll()
                .Where(
                t => t.IsActive &&
                (byte.Parse(_featureChecker.GetValue(t.Id, AppFeatures.ShipperPeriods)) == PeriodId || byte.Parse(_featureChecker.GetValue(t.Id, AppFeatures.CarrierPeriods)) == PeriodId) &&
                (t.Edition.Name == TACHYONConsts.ShipperEdtionName || t.Edition.Name == TACHYONConsts.CarrierEdtionName));

            foreach (var Tenant in Tenants)
            {
                CollectRequestForShipper(Tenant, PeriodId);
                BuildCarrierGroup(Tenant, PeriodId);

            }

        }
        private void CollectRequestForShipper(Tenant Tenant, byte PeriodId)
        {       
            var Requests = _shippingRequestRepository
                .GetAllList(r =>r.TenantId== Tenant.Id && r.IsShipperHaveInvoice == false && r.IsPriceAccepted == true && r.IsPrePayed == false);
          if (Requests.Count()>0) GenerateShipperInvoice(Tenant, Requests, PeriodId);

        }

        private void BuildCarrierGroup(Tenant Tenant, byte PeriodId)
        {

            var Requests = _shippingRequestRepository
                .GetAllList(r => r.CarrierTenantId== Tenant.Id &&  r.IsCarrierHaveInvoice == false && r.IsPriceAccepted == true );
            if (Requests.Count() == 0) return;
            decimal Amount =(decimal)Requests.Sum(r => r.Price);
            TaxVat = GetTax();
            decimal VatAmount = (Amount * TaxVat / 100);
            decimal AmountWithTaxVat = VatAmount + Amount;
            var GroupPeriod = new GroupPeriod
            {
                TenantId = Tenant.Id,
                PeriodId = PeriodId,
                IsDemand = false,
                Amount = Amount,
                TaxVat = TaxVat,
                AmountWithTaxVat = AmountWithTaxVat,
                VatAmount= VatAmount,
                ShippingRequests = Requests.Select(
               r => new GroupShippingRequests()
               {
                   RequestId = r.Id
               }).ToList()
            };
            _GroupRepository.Insert(GroupPeriod);
           // Tenant.Balance +=AmountWithTaxVat;


            foreach (var request in Requests)
            {
                request.IsCarrierHaveInvoice = true;
            }
            _appNotifier.NewGroupPeriodsGenerated(GroupPeriod);
            //_emailSender.Send(
            //               to: user.EmailAddress,
            //               subject: "You have a new notification!",
            //               body: data.Message,
            //               isBodyHtml: true
            //           );

            /*Update For Both account Shipper and Carrier  */
        }


        private void GenerateShipperInvoice(Tenant Tenant, List<ShippingRequest> Requests, byte PeriodId)
        {
            decimal Amount = (decimal)Requests.Sum(r => r.Price);
            TaxVat = GetTax();
            decimal VatAmount = (Amount * TaxVat / 100);
            decimal AmountWithTaxVat = VatAmount + Amount;
            var PeriodType = (InvoicePeriodType)PeriodId;
            var Invoice = new Invoice
            {
                TenantId = Tenant.Id,
                PeriodId = PeriodId,
                DueDate = Clock.Now,
                IsPaid = PeriodType == InvoicePeriodType.PayInAdvance ? true: false,
                Amount = Amount,
                VatAmount = VatAmount,
                TaxVat = TaxVat,
                AmountWithTaxVat = AmountWithTaxVat,
                IsAccountReceivable = true,
                ShippingRequests = Requests.Select(
               r => new InvoiceShippingRequests()
               {
                   RequestId = r.Id
               }).ToList()
            };
            _InvoiceRepository.Insert(Invoice);

            foreach (var request in Requests)
            {
                request.IsShipperHaveInvoice = true;
            }

            if (PeriodType == InvoicePeriodType.PayInAdvance) {
                Tenant.Balance -= AmountWithTaxVat;
            }
            else
            {
                Tenant.CreditBalance -= AmountWithTaxVat;
            }


            _appNotifier.NewInvoiceShipperGenerated(Invoice);
        }

        public async Task GenerateCarrirInvoice(GroupPeriod Group)
        {
            var GroupRequest = Group.ShippingRequests.Select(r => r.RequestId).OfType<long>().ToArray();
            var Requests = _shippingRequestRepository.GetAllList(r => GroupRequest.Contains(r.Id));
            TaxVat = GetTax();
            decimal Amount = (decimal)Requests.Sum(r => r.Price);
            decimal VatAmount = (Amount * TaxVat / 100);
            decimal AmountWithTaxVat = VatAmount + Amount;
            var Invoice = new Invoice
            {
                TenantId = Group.TenantId,
                PeriodId = Group.PeriodId,
                DueDate = Clock.Now,
                IsPaid = false,
                Amount = Amount,
                VatAmount = VatAmount,
                TaxVat = TaxVat,
                AmountWithTaxVat = AmountWithTaxVat,
                IsAccountReceivable = false,
                ShippingRequests = Requests.Select(
               r => new InvoiceShippingRequests()
               {
                   RequestId = r.Id
               }).ToList()
            };
          
          var InvoiceId=   await  _InvoiceRepository.InsertAndGetIdAsync(Invoice);
            
           await _GroupPeriodInvoiceRepository.InsertAsync(new GroupPeriodInvoice { GroupId = Group.Id, InvoiceId = InvoiceId });
            foreach (var request in Requests)
            {
                request.IsShipperHaveInvoice = true;
            }

            Group.Tenant.Balance += AmountWithTaxVat;


           await _appNotifier.NewInvoiceShipperGenerated(Invoice);
        }

        public async Task GenertateInvoiceOutPeriod(ShippingRequest request)
        {
            var Tenant = await _Tenant.SingleAsync(t=>t.Id== request.TenantId);
            var PeriodType = (InvoicePeriodType)byte.Parse(_featureChecker.GetValue(request.TenantId, AppFeatures.ShipperPeriods));
            if (PeriodType == InvoicePeriodType.PayuponDelivery || PeriodType == InvoicePeriodType.PayInAdvance)
            {
                GenerateShipperInvoice(Tenant,new List<ShippingRequest>() { request }, (byte)PeriodType);
            }          
        }


        public async Task RemoveInvoiceFromRequest(long invoiceId)
        {
            var invoice = await GetInvoiceInfo(invoiceId);
            if (invoice == null) return;
            var Invoicerequests = _invoiceShippingRequestsRepository.
                GetAll().
                Where(r => r.InvoiceId == invoice.Id)
               .Select(r=> r.RequestId).OfType<long>().ToArray();

            var Requests = _shippingRequestRepository.GetAllList(r => Invoicerequests.Contains(r.Id));
            if (!IsCarrier(invoice.TenantId))
            {
                if (invoice.IsPaid) await _BalanceManager.AddBalanceToShipper(invoice.TenantId, -invoice.AmountWithTaxVat);
                foreach (var request in Requests)
                {
                    request.IsShipperHaveInvoice = false;
                }
            }
            else
            {
                if (invoice.IsPaid) await _BalanceManager.AddBalanceToCarrier(invoice.TenantId, -invoice.AmountWithTaxVat);
            }
           await _InvoiceRepository.DeleteAsync(invoice);
        }



        public async Task<Invoice> GetInvoiceInfo(long InvoiceId)
        {
            return await _InvoiceRepository
                               .GetAll()
                               .Include(i => i.InvoicePeriod)
                               .Include(i => i.Tenant)
                               .Include(i => i.ShippingRequests)
                                .ThenInclude(r => r.ShippingRequests)
                                 .ThenInclude(r => r.TrucksTypeFk)
                               .SingleAsync(i => i.Id == InvoiceId);
        }

        public bool IsCarrier(int TenantId)
        {
            return _featureChecker.IsEnabled(TenantId, AppFeatures.Carrier);
        }

        private decimal GetTax()
        {
            return _settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat);
        }
    }
}
