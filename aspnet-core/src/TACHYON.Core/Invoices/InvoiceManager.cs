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
using TACHYON.MultiTenancy;
using TACHYON.Shipping.ShippingRequests;
using System.Collections.Generic;
using Abp.Net.Mail;
using TACHYON.Notifications;

namespace TACHYON.Invoices
{
    public class InvoiceManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<InvoicePeriod> _PeriodRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IEmailSender _emailSender;
        private readonly IAppNotifier _appNotifier;

        private readonly IQuartzScheduleJobManager _jobManager;
        private TenantManager TenantManager { get; set; }
        private readonly IRepository<Invoice, long> _InvoiceRepository;

        public InvoiceManager(
            IRepository<InvoicePeriod> PeriodRepository,
            IRepository<Invoice, long> InvoiceRepository,
            IQuartzScheduleJobManager JobManager,
            IEmailSender EmailSender,
            IAppNotifier AppNotifier)
        {
            _PeriodRepository = PeriodRepository;
            _InvoiceRepository = InvoiceRepository;
            _jobManager = JobManager;
            _emailSender = EmailSender;
            _appNotifier = AppNotifier;
        }

        public async void RunAllJobs()
        {
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
            if (Period.Enabled) await CreateTiggerAsync(Period);


        }

        public void GenerateInvoice(int PeriodId)
        {

            IQueryable<IDictionary<string, object>> Tenants = TenantManager.Tenants
                .Where(t => t.IsActive && t.InvoicePeriodId == PeriodId && (t.Edition.Name == "Shipper" || t.Edition.Name == "Carrier"))
                .Select(t =>
                             new Dictionary<string, object>(){
                             {"EditionName", t.Edition.Name},
                             {"TenantId", t.Id}
                        })
                .OfType<IDictionary<string, object>>();

            foreach (var Tenant in Tenants)
            {
                BuildInvoice(Tenant, PeriodId);

            }

        }
        private void BuildInvoice(IDictionary<string, object> Tenant, int PeriodId)
        {
            var Requests = _shippingRequestRepository
                .GetAllList(r => r.InvoiceId != null && r.IsPriceAccepted == true && r.IsPrePayed == false);
            decimal? Amount = Requests.Sum(r => r.Price);
            decimal? TotalVat = 5;
            decimal? TotalSumExclVat = ((Amount * TotalVat) / 100) + Amount;
            var Invoice = new Invoice
            {
                TenantId = (int)Tenant["TenantId"],
                PeriodId = PeriodId,
                DueDate = Clock.Now,
                IsPaid = false,
                Amount = Amount,
                TotalVat = TotalVat,
                TotalSumExclVat = TotalSumExclVat,
                IsAccountReceivable = (string)Tenant["Shipper"] == "" ? true : false,
                ShippingRequests = Requests.Select(
               r => new InvoiceShippingRequests()
               {
                   RequestId = r.Id
               }).ToList()
            };
            _InvoiceRepository.Insert(Invoice);
            _appNotifier.NewInvoiceShipperGenerated(Invoice);
            //_emailSender.Send(
            //               to: user.EmailAddress,
            //               subject: "You have a new notification!",
            //               body: data.Message,
            //               isBodyHtml: true
            //           );

            /*Update For Both account Shipper and Carrier  */
        }


        private void InvoiceRejected(int InvoiceId)
        {

        }
    }
}
