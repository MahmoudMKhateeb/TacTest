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
using TACHYON.Invoices.Transactions;

namespace TACHYON.Invoices
{
    public class InvoiceManager : TACHYONDomainServiceBase
    {
        #region property
        private readonly IRepository<InvoicePeriod> _PeriodRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<InvoiceShippingRequests, long> _invoiceShippingRequestsRepository;
        private readonly IRepository<GroupPeriodInvoice, long> _GroupPeriodInvoiceRepository;
        private readonly IRepository<InvoiceProforma, long> _invoiceProformaRepository;

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
        private readonly TransactionManager _transactionManager;
       
        private decimal TaxVat;
        #endregion
        public InvoiceManager(
            IRepository<InvoicePeriod> PeriodRepository,
            IRepository<Invoice, long> InvoiceRepository,
            IRepository<GroupPeriodInvoice, long> GroupPeriodInvoiceRepository,
            IRepository<InvoiceProforma, long> invoiceProformaRepository,
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
             IUnitOfWorkManager unitOfWorkManager,
             TransactionManager transactionManager)
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
            _transactionManager = transactionManager;
            _invoiceProformaRepository = invoiceProformaRepository;
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
        /// <summary>
        /// Generate invoices for shipper and submitinvoices for carrirer by period
        /// </summary>
        /// <param name="PeriodId"></param>
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
                BuildCarrierSubmitInvoice(Tenant, PeriodId);

            }

        }

        /// <summary>
        /// Collect all shipping request for shipper in the period interval
        /// </summary>
        /// <param name="Tenant"></param>
        /// <param name="PeriodId"></param>
        private void CollectRequestForShipper(Tenant Tenant, byte PeriodId)
        {       
            var Requests = _shippingRequestRepository
                .GetAllList(r =>r.TenantId== Tenant.Id && r.IsShipperHaveInvoice == false && r.Status ==  ShippingRequestStatus.PostPrice && r.IsPrePayed == false);
          if (Requests.Count()>0) GenerateShipperInvoice(Tenant, Requests, PeriodId);

        }
        /// <summary>
        /// Generate submit invoices for carrirer in period intreval
        /// </summary>
        /// <param name="Tenant"></param>
        /// <param name="PeriodId"></param>
        private void BuildCarrierSubmitInvoice(Tenant Tenant, byte PeriodId)
        {

            var Requests = _shippingRequestRepository
                .GetAllList(r => r.CarrierTenantId== Tenant.Id &&  r.IsCarrierHaveInvoice == false && r.Status == ShippingRequestStatus.PostPrice);
            if (Requests.Count() == 0) return;
            decimal Amount =(decimal)Requests.Sum(r => r.Price);
            //TaxVat = GetTax();
            //decimal VatAmount = (Amount * TaxVat / 100);
            //decimal AmountWithTaxVat = VatAmount + Amount;
            var GroupPeriod = new GroupPeriod
            {
                TenantId = Tenant.Id,
                PeriodId = PeriodId,
                Amount = Amount,
               // TaxVat = TaxVat,
               // AmountWithTaxVat = AmountWithTaxVat,
               // VatAmount= VatAmount,
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
            _appNotifier.NewSubmitInvoiceGenerated(GroupPeriod);
            //_emailSender.Send(
            //               to: user.EmailAddress,
            //               subject: "You have a new notification!",
            //               body: data.Message,
            //               isBodyHtml: true
            //           );

            /*Update For Both account Shipper and Carrier  */
        }
        /// <summary>
        /// Generate invoices for shipper
        /// </summary>
        /// <param name="Tenant"></param>
        /// <param name="Requests"></param>
        /// <param name="PeriodId"></param>
        private async void GenerateShipperInvoice(Tenant Tenant, List<ShippingRequest> Requests, byte PeriodId)
        {
            decimal TotalAmount = (decimal)Requests.Sum(r => r.Price);
            decimal VatAmount= (decimal)Requests.Sum(r => r.Price);
            decimal SubTotalAmount = (decimal)Requests.Sum(r => r.Price);

            var PeriodType = (InvoicePeriodType)PeriodId;
            var Invoice = new Invoice
            {
                TenantId = Tenant.Id,
                PeriodId = PeriodId,
                DueDate = Clock.Now,
                IsPaid = PeriodType == InvoicePeriodType.PayInAdvance ? true : false,
                TotalAmount = TotalAmount,
                VatAmount = VatAmount,
                SubTotalAmount = SubTotalAmount,
                TaxVat = Requests.FirstOrDefault().VatSetting,
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
                Tenant.Balance -= TotalAmount;
                Tenant.ReservedBalance -= TotalAmount;
                var invoiceProformas =await _invoiceProformaRepository.SingleAsync(i => i.TenantId == Tenant.Id && i.RequestId== Requests[0].Id);
            if (invoiceProformas !=null)   await _invoiceProformaRepository.DeleteAsync(invoiceProformas);
            }
            else
            {
                Tenant.CreditBalance -= TotalAmount;
            }


          await  _appNotifier.NewInvoiceShipperGenerated(Invoice);
          await  _BalanceManager.CheckShipperOverLimit(Tenant);
        }
/// <summary>
/// Generate invoice for carrirer after the host accepted the submit invoice
/// </summary>
/// <param name="Group"></param>
/// <returns></returns>
        public async Task GenerateCarrirInvoice(GroupPeriod Group)
        {
            var GroupRequest = Group.ShippingRequests.Select(r => r.RequestId) ;
            var Requests = _shippingRequestRepository.GetAllList(r => GroupRequest.Contains(r.Id));

            var Invoice = new Invoice
            {
                TenantId = Group.TenantId,
                PeriodId = Group.PeriodId,
                DueDate = Clock.Now,
                IsPaid = false,
                TotalAmount = Group.Amount,
                //VatAmount = Group.VatAmount,
                TaxVat = Group.TaxVat,
                //AmountWithTaxVat = Group.AmountWithTaxVat,
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

            Group.Tenant.Balance += Group.AmountWithTaxVat;
        }
        /// <summary>
        /// When the shipper billing interval  after delivry run this method to generate invoice
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task GenertateInvoiceWhenShipmintDelivery(ShippingRequest request)
        {
            var Tenant = await _Tenant.SingleAsync(t=>t.Id== request.TenantId);
            var PeriodType = (InvoicePeriodType)byte.Parse(_featureChecker.GetValue(request.TenantId, AppFeatures.ShipperPeriods));
            if (PeriodType == InvoicePeriodType.PayuponDelivery || PeriodType == InvoicePeriodType.PayInAdvance)
            {
                GenerateShipperInvoice(Tenant,new List<ShippingRequest>() { request }, (byte)PeriodType);
            }          
        }

        /// <summary>
        /// remove invoice from shipping request when delete invoice
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public async Task RemoveInvoiceFromRequest(long invoiceId)
        {
            var invoice = await GetInvoiceInfo(invoiceId);
            if (invoice == null) return;
            var Invoicerequests = _invoiceShippingRequestsRepository.
                GetAll().
                Where(r => r.InvoiceId == invoice.Id)
               .Select(r=> r.RequestId);

            var Requests = _shippingRequestRepository.GetAllList(r => Invoicerequests.Contains(r.Id));
            if (!IsCarrier(invoice.TenantId))
            {
                if (invoice.IsPaid) await _BalanceManager.AddBalanceToShipper(invoice.TenantId, -invoice.TotalAmount);
                Requests.ForEach(request =>
                {
                    request.IsShipperHaveInvoice = false;
                });

            }
            else
            {
                if (invoice.IsPaid) await _BalanceManager.AddBalanceToCarrier(invoice.TenantId, -invoice.TotalAmount);
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
