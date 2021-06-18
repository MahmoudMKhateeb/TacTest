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
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Invoices.PaymentMethods;

namespace TACHYON.Invoices
{
    public class InvoiceManager : TACHYONDomainServiceBase
    {
        #region property
        private readonly IRepository<InvoicePeriod> _periodRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTrip;
        private readonly IRepository<InvoicePaymentMethod> _invoicePaymentMethodRepository;
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
        private readonly TransactionManager _transactionManager;

        #endregion
        public InvoiceManager(
            IRepository<InvoicePeriod> PeriodRepository,
            IRepository<Invoice, long> InvoiceRepository,
            IRepository<GroupPeriodInvoice, long> GroupPeriodInvoiceRepository,
            IQuartzScheduleJobManager JobManager,
            IEmailSender EmailSender,
            IAppNotifier AppNotifier,
            ISettingManager SettingManager,
            IRepository<GroupPeriod, long> GroupRepository,
            IFeatureChecker featureChecker,
            IRepository<Tenant> tenant,
             BalanceManager BalanceManager,
             IUnitOfWorkManager unitOfWorkManager,
             TransactionManager transactionManager, IRepository<ShippingRequestTrip> shippingRequestTrip, IRepository<InvoicePaymentMethod> invoicePaymentMethodRepository)
        {
            _periodRepository = PeriodRepository;
            _InvoiceRepository = InvoiceRepository;
            _GroupPeriodInvoiceRepository = GroupPeriodInvoiceRepository;
            _jobManager = JobManager;
            _emailSender = EmailSender;
            _appNotifier = AppNotifier;
            _settingManager = SettingManager;
            _GroupRepository = GroupRepository;
            _featureChecker = featureChecker;
            _Tenant = tenant;
            _BalanceManager = BalanceManager;
            _unitOfWorkManager = unitOfWorkManager;
            _transactionManager = transactionManager;
            _shippingRequestTrip = shippingRequestTrip;
            _invoicePaymentMethodRepository = invoicePaymentMethodRepository;
        }


        public async void RunAllJobs()
        {
            
            var Results = _periodRepository
                .GetAll()
                .WhereIf(true, p => p.Enabled && p.PeriodType != InvoicePeriodType.PayInAdvance && p.PeriodType != InvoicePeriodType.PayuponDelivery);

            foreach (var Period in Results)
            {
                await CreateTiggerAsync(Period);
            }
        }
        /// <summary>
        /// Create job worker for each period in invoice
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task CreateTiggerAsync(InvoicePeriod Period)
        {
            string myJobKey = $"InvoiceJob.[{Enum.GetName(typeof(InvoicePeriodType), Period.PeriodType)}].[{Period.Id}]";
            string TriggerKey = $"InvoiceTrigger.[{Enum.GetName(typeof(InvoicePeriodType), Period.PeriodType)}].[{Period.Id}]";
            try
            {
                await _jobManager.ScheduleAsync<InvoiceJob>(
                         job =>
                         {
                             job.WithIdentity(myJobKey)
                                 .WithDescription("A job to simply write logs.")
                                 .UsingJobData("PeriodType", (int)Period.PeriodType)
                                 .UsingJobData("PeriodId", Period.Id)
                                 .StoreDurably();

                         },
                         trigger =>
                         {
                             trigger.StartNow()
                             .WithIdentity(TriggerKey)
                             .UsingJobData("PeriodType", (int)Period.PeriodType)
                             .UsingJobData("PeriodId", Period.Id)
                             .WithCronSchedule(Period.Cronexpression)
                             .ForJob(myJobKey);
                         });
            }
            catch
            { // If Exists before just update the Schedule
                await _jobManager.RescheduleAsync(new TriggerKey(TriggerKey),
                         trigger =>
                         {
                             trigger.StartNow()
                             .WithIdentity(TriggerKey)
                             .UsingJobData("PeriodType", (int)Period.PeriodType)
                             .UsingJobData("PeriodId", Period.Id)
                             .WithCronSchedule(Period.Cronexpression)
                             .ForJob(myJobKey);
                         });
            }

        }

        public async Task RemoveTriggerAsync(InvoicePeriod Period)
        {
            var TriggerKey = new TriggerKey($"InvoiceTrigger.[{Enum.GetName(typeof(InvoicePeriodType), Period.PeriodType)}].[{Period.Id}]");
            await _jobManager.UnscheduleAsync(TriggerKey);
        }


        public async Task UpdateTriggerAsync(InvoicePeriod Period)
        {
            if (Period.Enabled & Period.PeriodType == InvoicePeriodType.PayInAdvance || Period.PeriodType == InvoicePeriodType.PayuponDelivery)
                
                await RemoveTriggerAsync(Period);
            else
            {
                await CreateTiggerAsync(Period);
            }

        }
        /// <summary>
        /// Generate invoices for shipper and submitinvoices for carrirer by period
        /// </summary>
        /// <param name="PeriodId"></param>
        public void GenerateInvoice(int PeriodId)
        {
            var Period = _periodRepository.FirstOrDefault(x => x.Id == PeriodId);
            var Tenants = _Tenant.GetAll()
                .Where(
                t => t.IsActive &&
                (int.Parse(_featureChecker.GetValue(t.Id, AppFeatures.ShipperPeriods)) == PeriodId || int.Parse(_featureChecker.GetValue(t.Id, AppFeatures.CarrierPeriods)) == PeriodId) &&
                (t.Edition.Name == AppConsts.ShipperEditionName || t.Edition.Name == AppConsts.CarrierEditionName));

            foreach (var Tenant in Tenants)
            {
                CollectTripsForShipper(Tenant, Period);
                //BuildCarrierSubmitInvoice(Tenant, PeriodId);

            }
        }

        /// <summary>
        /// Collect all shipping request for shipper in the period interval
        /// </summary>
        /// <param name="Tenant"></param>
        /// <param name="PeriodId"></param>
        private void CollectTripsForShipper(Tenant Tenant, InvoicePeriod period)
        {       
            var Trips = _shippingRequestTrip
                .GetAllList(r =>r.ShippingRequestFk.TenantId== Tenant.Id && r.IsShipperHaveInvoice == false && r.Status== Shipping.Trips.ShippingRequestTripStatus.Delivered && r.ShippingRequestFk.IsPrePayed == false);
          if (Trips.Count()>0) GenerateShipperInvoice(Tenant, Trips, period);

        }
        /// <summary>
        /// Generate submit invoices for carrirer in period intreval
        /// </summary>
        /// <param name="Tenant"></param>
        /// <param name="PeriodId"></param>
        //private void BuildCarrierSubmitInvoice(Tenant Tenant, int PeriodId)
        //{

        //    var Trips = _shippingRequestTrip
        //        .GetAllList(r => r.ShippingRequestFk.CarrierTenantId== Tenant.Id &&  r.IsCarrierHaveInvoice == false && r.Status == Shipping.Trips.ShippingRequestTripStatus.Delivered);
        //    if (Trips.Count() == 0) return;
        //    decimal Amount =(decimal)Requests.Sum(r => r.Price);
        //    var GroupPeriod = new GroupPeriod
        //    {
        //        TenantId = Tenant.Id,
        //        PeriodId = PeriodId,
        //        Amount = Amount,
        //       // TaxVat = TaxVat,
        //       // AmountWithTaxVat = AmountWithTaxVat,
        //       // VatAmount= VatAmount,
        //        ShippingRequests = Requests.Select(
        //       r => new GroupShippingRequests()
        //       {
        //           RequestId = r.Id
        //       }).ToList()
        //    };
        //    _GroupRepository.Insert(GroupPeriod);
        //   // Tenant.Balance +=AmountWithTaxVat;


        //    foreach (var request in Requests)
        //    {
        //        request.IsCarrierHaveInvoice = true;
        //    }
        //    _appNotifier.NewSubmitInvoiceGenerated(GroupPeriod);
        //    //_emailSender.Send(
        //    //               to: user.EmailAddress,
        //    //               subject: "You have a new notification!",
        //    //               body: data.Message,
        //    //               isBodyHtml: true
        //    //           );

        //    /*Update For Both account Shipper and Carrier  */
        //}
        /// <summary>
        /// Generate invoices for shipper
        /// </summary>
        /// <param name="Tenant"></param>
        /// <param name="Requests"></param>
        /// <param name="PeriodId"></param>
        private async Task GenerateShipperInvoice(Tenant Tenant, List<ShippingRequestTrip> Trips, InvoicePeriod period)
        {
            decimal TotalAmount = (decimal)Trips.Sum(r => r.TotalAmountWithCommission + r.ShippingRequestTripVases.Sum(v=>v.TotalAmountWithCommission));
            decimal VatAmount= (decimal)Trips.Sum(r => r.VatAmountWithCommission + r.ShippingRequestTripVases.Sum(v => v.VatAmountWithCommission));
            decimal SubTotalAmount = (decimal)Trips.Sum(r => r.SubTotalAmountWithCommission + r.ShippingRequestTripVases.Sum(v => v.SubTotalAmountWithCommission));
            DateTime DueDate= Clock.Now;

            if (period.PeriodType != InvoicePeriodType.PayInAdvance)
            {
                var PaymentType = await _invoicePaymentMethodRepository.FirstOrDefaultAsync(x => x.Id == int.Parse(_featureChecker.GetValue(Tenant.Id, AppFeatures.InvoicePaymentMethod)));
                if (PaymentType.PaymentType == PaymentMethod.InvoicePaymentType.Days)
                {
                    DueDate = Clock.Now.AddDays(PaymentType.InvoiceDueDateDays);
                }
            }



            var Invoice = new Invoice
            {
                TenantId = Tenant.Id,
                PeriodId = period.Id,
                DueDate = DueDate,
                IsPaid = period.PeriodType== InvoicePeriodType.PayInAdvance,
                TotalAmount = TotalAmount,
                VatAmount = VatAmount,
                SubTotalAmount = SubTotalAmount,
                TaxVat = Trips.FirstOrDefault().TaxVat.Value,
                AccountType = InvoiceAccountType.AccountReceivable,
                Trips = Trips.Select(
               r => new InvoiceTrip()
               {
                   TripId = r.Id
               }).ToList()
            };
            Invoice.Id=await  _InvoiceRepository.InsertAndGetIdAsync(Invoice);

            foreach (var trip in Trips)
            {
                trip.IsShipperHaveInvoice = true;
            }

            if (period.PeriodType == InvoicePeriodType.PayInAdvance) {
                Tenant.Balance -= TotalAmount;
                Tenant.ReservedBalance -= TotalAmount;

            }
            else
            {
                Tenant.CreditBalance -= TotalAmount;
            }



            await _BalanceManager.CheckShipperOverLimit(Tenant);
            await _appNotifier.NewInvoiceShipperGenerated(Invoice);
        }
/// <summary>
/// Generate invoice for carrirer after the host accepted the submit invoice
/// </summary>
/// <param name="Group"></param>
/// <returns></returns>
        //public async Task GenerateCarrirInvoice(GroupPeriod Group)
        //{
        //    var GroupRequest = Group.ShippingRequests.Select(r => r.RequestId) ;
        //    var Requests = _shippingRequestRepository.GetAllList(r => GroupRequest.Contains(r.Id));

        //    //var Invoice = new Invoice
        //    //{
        //    //    TenantId = Group.TenantId,
        //    //    PeriodId = Group.PeriodId,
        //    //    DueDate = Clock.Now,
        //    //    IsPaid = false,
        //    //    TotalAmount = Group.Amount,
        //    //    //VatAmount = Group.VatAmount,
        //    //    TaxVat = Group.TaxVat,
        //    //    //AmountWithTaxVat = Group.AmountWithTaxVat,
        //    //    AccountType = InvoiceAccountType.AccountPayable,
        //    //    ShippingRequests = Requests.Select(
        //    //   r => new InvoiceShippingRequests()
        //    //   {
        //    //       RequestId = r.Id
        //    //   }).ToList()
        //    //};
          
        // // var InvoiceId=   await  _InvoiceRepository.InsertAndGetIdAsync(Invoice);
            
        //   //await _GroupPeriodInvoiceRepository.InsertAsync(new GroupPeriodInvoice { GroupId = Group.Id, InvoiceId = InvoiceId });
        //   // foreach (var request in Requests)
        //   // {
        //   //     request.IsShipperHaveInvoice = true;
        //   // }

        //    Group.Tenant.Balance += Group.AmountWithTaxVat;
        //}
        /// <summary>
        /// When the shipper billing interval  after delivry run this method to generate invoice
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task GenertateInvoiceWhenShipmintDelivery(ShippingRequestTrip trip)
        {
            var Tenant = trip.ShippingRequestFk.Tenant;
            InvoicePeriod Period=default;
            ///If the shipemnt pay in advance get the period entity for pay in advance else get from the features
            if (trip.ShippingRequestFk.IsPrePayed.HasValue && trip.ShippingRequestFk.IsPrePayed.Value)
            {
                Period = await _periodRepository.FirstOrDefaultAsync(x => x.PeriodType== InvoicePeriodType.PayInAdvance);

            }
            else
            {
                 Period = await _periodRepository.FirstOrDefaultAsync(x => x.Id == int.Parse(_featureChecker.GetValue(Tenant.Id, AppFeatures.ShipperPeriods)));

            }

            if (Period.PeriodType == InvoicePeriodType.PayuponDelivery || Period.PeriodType== InvoicePeriodType.PayInAdvance)
            {
               await GenerateShipperInvoice(Tenant,new List<ShippingRequestTrip>() { trip }, Period);
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
        
            if (!IsCarrier(invoice.TenantId))
            {
                if (invoice.IsPaid) await _BalanceManager.AddBalanceToShipper(invoice.TenantId, -invoice.TotalAmount);
                invoice.Trips.ToList().ForEach(t =>
                {
                    t.ShippingRequestTripFK.IsShipperHaveInvoice = false;
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
                               .Include(i => i.Trips)
                                .ThenInclude(t=>t.ShippingRequestTripFK)
                               .FirstOrDefaultAsync(i => i.Id == InvoiceId);
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
