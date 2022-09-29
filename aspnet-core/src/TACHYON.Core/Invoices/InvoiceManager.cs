using Abp.Application.Features;
using Abp.Collections.Extensions;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Net.Mail;
using Abp.Quartz;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Configuration;
using TACHYON.Core.Invoices.Jobs;
using TACHYON.Dto;
using TACHYON.DynamicInvoices;
using TACHYON.Features;
using TACHYON.Invoices.Balances;
using TACHYON.Invoices.Groups;
using TACHYON.Invoices.PaymentMethods;
using TACHYON.Invoices.Periods;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.Invoices.Transactions;
using TACHYON.MultiTenancy;
using TACHYON.Notifications;
using TACHYON.Penalties;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;

namespace TACHYON.Invoices
{
    public class InvoiceManager : TACHYONDomainServiceBase
    {
        #region property

        private readonly IRepository<InvoicePeriod> _periodRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTrip;
        private readonly IRepository<InvoicePaymentMethod> _invoicePaymentMethodRepository;
        private readonly IRepository<GroupPeriodInvoice, long> _groupPeriodInvoiceRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Tenant> _tenant;
        private readonly IEmailSender _emailSender;
        private readonly IAppNotifier _appNotifier;
        private readonly ISettingManager _settingManager;
        private readonly IQuartzScheduleJobManager _jobManager;
        private readonly IRepository<Invoice, long> _invoiceRepository;
        private readonly IRepository<SubmitInvoice, long> _submitInvoiceRepository;

        private readonly IRepository<GroupPeriod, long> _groupRepository;
        private readonly IFeatureChecker _featureChecker;
        private readonly BalanceManager _balanceManager;
        private readonly TransactionManager _transactionManager;
        private readonly IRepository<Penalty> _penaltyRepository;

        #endregion

        public InvoiceManager(
            IRepository<InvoicePeriod> periodRepository,
            IRepository<Invoice, long> invoiceRepository,
            IRepository<GroupPeriodInvoice, long> groupPeriodInvoiceRepository,
            IQuartzScheduleJobManager jobManager,
            IEmailSender emailSender,
            IAppNotifier appNotifier,
            ISettingManager settingManager,
            IRepository<GroupPeriod, long> groupRepository,
            IFeatureChecker featureChecker,
            IRepository<Tenant> tenant,
            BalanceManager balanceManager,
            IUnitOfWorkManager unitOfWorkManager,
            TransactionManager transactionManager,
            IRepository<ShippingRequestTrip> shippingRequestTrip,
            IRepository<InvoicePaymentMethod> invoicePaymentMethodRepository,
            IRepository<SubmitInvoice, long> submitInvoiceRepository,
            IRepository<Penalty> penaltyRepository)
        {
            _periodRepository = periodRepository;
            _invoiceRepository = invoiceRepository;
            _groupPeriodInvoiceRepository = groupPeriodInvoiceRepository;
            _jobManager = jobManager;
            _emailSender = emailSender;
            _appNotifier = appNotifier;
            _settingManager = settingManager;
            _groupRepository = groupRepository;
            _featureChecker = featureChecker;
            _tenant = tenant;
            _balanceManager = balanceManager;
            _unitOfWorkManager = unitOfWorkManager;
            _transactionManager = transactionManager;
            _shippingRequestTrip = shippingRequestTrip;
            _invoicePaymentMethodRepository = invoicePaymentMethodRepository;
            _submitInvoiceRepository = submitInvoiceRepository;
            _penaltyRepository = penaltyRepository;
        }


        public async void RunAllJobs()
        {
            var results = _periodRepository
                .GetAll()
                .WhereIf(true,
                    p => p.Enabled && p.PeriodType != InvoicePeriodType.PayInAdvance &&
                         p.PeriodType != InvoicePeriodType.PayuponDelivery);

            foreach (var period in results)
            {
                await CreateTriggerAsync(period);
            }
        }

        /// <summary>
        /// Create job worker for each period in invoice
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        public async Task CreateTriggerAsync(InvoicePeriod period)
        {
            string myJobKey =
                $"InvoiceJob.[{Enum.GetName(typeof(InvoicePeriodType), period.PeriodType)}].[{period.Id}]";
            string triggerKey =
                $"InvoiceTrigger.[{Enum.GetName(typeof(InvoicePeriodType), period.PeriodType)}].[{period.Id}]";
            try
            {
                await _jobManager.ScheduleAsync<InvoiceJob>(
                    job =>
                    {
                        job.WithIdentity(myJobKey)
                            .WithDescription("A job to simply write logs.")
                            .UsingJobData("PeriodType", (int)period.PeriodType)
                            .UsingJobData("PeriodId", period.Id)
                            .StoreDurably();
                    },
                    trigger =>
                    {
                        trigger.StartNow()
                            .WithIdentity(triggerKey)
                            .UsingJobData("PeriodType", (int)period.PeriodType)
                            .UsingJobData("PeriodId", period.Id)
                            .WithCronSchedule(period.Cronexpression)
                            .ForJob(myJobKey);
                    });
            }
            catch
            {
                // If Exists before just update the Schedule
                await _jobManager.RescheduleAsync(new TriggerKey(triggerKey),
                    trigger =>
                    {
                        trigger.StartNow()
                            .WithIdentity(triggerKey)
                            .UsingJobData("PeriodType", (int)period.PeriodType)
                            .UsingJobData("PeriodId", period.Id)
                            .WithCronSchedule(period.Cronexpression)
                            .ForJob(myJobKey);
                    });
            }
        }

        public async Task RemoveTriggerAsync(InvoicePeriod period)
        {
            var triggerKey =
                new TriggerKey(
                    $"InvoiceTrigger.[{Enum.GetName(typeof(InvoicePeriodType), period.PeriodType)}].[{period.Id}]");
            await _jobManager.UnscheduleAsync(triggerKey);
        }


        public async Task UpdateTriggerAsync(InvoicePeriod period)
        {
            if (period.Enabled & period.PeriodType == InvoicePeriodType.PayInAdvance ||
                period.PeriodType == InvoicePeriodType.PayuponDelivery)

                await RemoveTriggerAsync(period);
            else
            {
                await CreateTriggerAsync(period);
            }
        }

        /// <summary>
        /// Generate invoices for shipper and submitinvoices for carrirer by period
        /// </summary>
        /// <param name="periodId"></param>
        public async Task GenerateInvoice(int periodId)
        {
            // get all tenants with this period
            List<Tenant> tenants = GetTenantByFeatures(periodId);

            var period = await _periodRepository.FirstOrDefaultAsync(x => x.Id == periodId);


            foreach (var tenant in tenants)
            {
                if (await _featureChecker.IsEnabledAsync(AppFeatures.Pay))
                {
                    await CollectTripsForShipper(tenant, period);
                    await PenaltyCollectorForPayTenants(tenant, period);
                }
                if (await _featureChecker.IsEnabledAsync(AppFeatures.Receipt))
                {
                    await BuildCarrierSubmitInvoice(tenant, period);
                    await PenaltyCollectorForReceiptTenants(tenant, period);


                }

            }
        }

        private List<Tenant> GetTenantByFeatures(int periodId)
        {
            List<Tenant> tenantsList = new List<Tenant>();
            var tenants = _tenant.GetAll()
                .Where(
                    t => t.IsActive && (t.Edition.Id == ShipperEditionId || t.Edition.Id == CarrierEditionId))
                .ToList();
            //todo fix this please 
            foreach (var tenant in tenants)
            {
                int value;
                if (tenant.EditionId == ShipperEditionId)
                {
                    value = int.Parse(_featureChecker.GetValue(tenant.Id, AppFeatures.ShipperPeriods));
                }
                else
                {
                    value = int.Parse(_featureChecker.GetValue(tenant.Id, AppFeatures.CarrierPeriods));
                }

                if (value == periodId)
                {
                    tenantsList.Add(tenant);
                }
            }

            return tenantsList;
        }


        /// <summary>
        /// Collect all shipping request for shipper in the period interval
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="PeriodId"></param>
        private async Task CollectTripsForShipper(Tenant tenant, InvoicePeriod period)
        {
            var trips = _shippingRequestTrip.GetAll()
                .Include(trip => trip.ShippingRequestTripVases)
                .Include(trip => trip.ShippingRequestFk)
                .Where(trip => trip.ShippingRequestFk.TenantId == tenant.Id)
                .Where(trip => !trip.IsShipperHaveInvoice)
                .Where(trip => trip.Status == Shipping.Trips.ShippingRequestTripStatus.Delivered
                || trip.InvoiceStatus == InvoiceTripStatus.CanBeInvoiced)
                .ToList();

            foreach (ShippingRequestTrip trip in trips.ToList())
            {
                var shipperId = trip.ShippingRequestFk.TenantId;
                var carrierId = trip.ShippingRequestFk.CarrierTenantId;

                if (!await _featureChecker.IsEnabledAsync(shipperId, AppFeatures.Saas))
                {
                    continue;
                }

                var relatedCarrierId =
                    int.Parse(await _featureChecker.GetValueAsync(shipperId, AppFeatures.SaasRelatedCarrier));
                if (carrierId == relatedCarrierId)
                {
                    trips.Remove(trip);
                }

            }

            if (trips.Any())
                await GenerateShipperInvoice(tenant, trips, period);
        }

        private async Task PenaltyCollectorForPayTenants(Tenant tenant, InvoicePeriod period)
        {
            DisableTenancyFilters();
            var penalties = await _penaltyRepository.GetAll()
                 .Include(x => x.ShippingRequestTripFK)
                 .ThenInclude(x => x.ShippingRequestFk)
                 .Where(x => x.TenantId == tenant.Id) // source company
                 .Where(x => !x.InvoiceId.HasValue &&
                 x.Status != PenaltyStatus.Canceled &&
                 x.Status != PenaltyStatus.Draft)
                 .ToListAsync();

            if (penalties.Any())
                await GeneratePenaltyInvoice(tenant, penalties, period);
        }


        private async Task PenaltyCollectorForReceiptTenants(Tenant tenant, InvoicePeriod period)
        {
            DisableTenancyFilters();
            var penalties = await _penaltyRepository.GetAll()
                 .Include(x => x.ShippingRequestTripFK)
                 .ThenInclude(x => x.ShippingRequestFk)
                 .Where(x => x.DestinationTenantId == tenant.Id) // destination  company
                 .Where(x => !x.SubmitInvoiceId.HasValue && x.Status != PenaltyStatus.Canceled &&
                 x.Status != PenaltyStatus.Draft).ToListAsync();

            if (penalties.Any())
                await GeneratePenaltySubmitInvoice(tenant, penalties, period);
        }



        /// <summary>
        /// Generate submit invoices for carrirer in period intreval
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="PeriodId"></param>
        private async Task BuildCarrierSubmitInvoice(Tenant tenant, InvoicePeriod period)
        {
            var trips = _shippingRequestTrip
                .GetAll()
                .Include(v => v.ShippingRequestTripVases)
                .Include(v => v.ShippingRequestFk)
                .Where(x => x.ShippingRequestFk.CarrierTenantId == tenant.Id
                            && (x.Status == Shipping.Trips.ShippingRequestTripStatus.Delivered || x.InvoiceStatus == InvoiceTripStatus.CanBeInvoiced)
                            && !x.IsCarrierHaveInvoice)
                .ToList();

            foreach (ShippingRequestTrip trip in trips.ToList())
            {
                var shipperId = trip.ShippingRequestFk.TenantId;
                var carrierId = trip.ShippingRequestFk.CarrierTenantId;
                if (carrierId == shipperId) // saas
                {
                    trips.Remove(trip);
                }


                //related carriers 
                int relatedCarrierId;

                int.TryParse(await _featureChecker.GetValueAsync(shipperId, AppFeatures.SaasRelatedCarrier), out relatedCarrierId);

                if (carrierId == relatedCarrierId)
                {
                    trips.Remove(trip);
                }


                //saas SR 
                if (trip.ShippingRequestFk.IsSaas())
                {
                    trips.Remove(trip);
                }
            }

            if (trips.Count == 0) return;
            decimal totalAmount =
                (decimal)trips.Sum(r => r.TotalAmount + r.ShippingRequestTripVases.Sum(v => v.TotalAmount));
            decimal vatAmount = (decimal)trips.Sum(r => r.VatAmount + r.ShippingRequestTripVases.Sum(v => v.VatAmount));
            decimal subTotalAmount = (decimal)trips.Sum(r =>
                r.SubTotalAmount + r.ShippingRequestTripVases.Sum(v => v.SubTotalAmount));

            var submitInvoice = new SubmitInvoice
            {
                TenantId = tenant.Id,
                PeriodId = period.Id,
                TotalAmount = totalAmount,
                VatAmount = vatAmount,
                SubTotalAmount = subTotalAmount,
                TaxVat = trips.FirstOrDefault(x => x.TaxVat.HasValue).TaxVat.Value,
                Channel = InvoiceChannel.Trip,
                Trips = trips.Select(
                    r => new SubmitInvoiceTrip() { TripId = r.Id }).ToList()
            };
            submitInvoice.Id = await _submitInvoiceRepository.InsertAndGetIdAsync(submitInvoice);


            foreach (var trip in trips)
            {
                trip.IsCarrierHaveInvoice = true;
            }
            //await  _appNotifier.NewSubmitInvoiceGenerated(submitInvoice);
        }

        /// <summary>
        /// Generate invoices for shipper
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="trips"></param>
        /// <param name="period"></param>
        public async Task GenerateShipperInvoice(Tenant tenant,
            List<ShippingRequestTrip> trips,
            InvoicePeriod period)
        {
            //not saas trips
            var notSaasTrips = trips.Where(x => !x.ShippingRequestFk.IsSaas());

            decimal notSaasTotalAmount = (decimal)notSaasTrips.Sum(r => r.TotalAmountWithCommission + r.ShippingRequestTripVases.Sum(v => v.TotalAmountWithCommission));
            decimal notSaasVatAmount = (decimal)notSaasTrips.Sum(r => r.VatAmountWithCommission + r.ShippingRequestTripVases.Sum(v => v.VatAmountWithCommission));
            decimal notSaaSubTotalAmount = (decimal)notSaasTrips.Sum(r => r.SubTotalAmountWithCommission + r.ShippingRequestTripVases.Sum(v => v.SubTotalAmountWithCommission));

            //saas trips
            var saasTrips = trips.Where(x => x.ShippingRequestFk.IsSaas());

            decimal SaasTotalAmount = (decimal)saasTrips.Sum(r => r.TotalAmountWithCommission);
            decimal SaasVatAmount = (decimal)saasTrips.Sum(r => r.VatAmountWithCommission);
            decimal SaasSubTotalAmount = (decimal)saasTrips.Sum(r => r.SubTotalAmountWithCommission);

            var totalAmount = notSaasTotalAmount + SaasTotalAmount;
            var vatAmount = notSaasVatAmount + SaasVatAmount;
            var subTotalAmount = notSaaSubTotalAmount + SaasSubTotalAmount;




            DateTime dueDate = Clock.Now;

            if (period.PeriodType != InvoicePeriodType.PayInAdvance)
            {
                var paymentType = await _invoicePaymentMethodRepository.FirstOrDefaultAsync(x =>
                    x.Id == int.Parse(_featureChecker.GetValue(tenant.Id, AppFeatures.InvoicePaymentMethod)));
                if (paymentType.PaymentType == PaymentMethod.InvoicePaymentType.Days)
                {
                    dueDate = Clock.Now.AddDays(paymentType.InvoiceDueDateDays);
                }
            }
            var invoice = new Invoice
            {
                TenantId = tenant.Id,
                PeriodId = period.Id,
                DueDate = dueDate,
                IsPaid = period.PeriodType == InvoicePeriodType.PayInAdvance,
                TotalAmount = totalAmount,
                VatAmount = vatAmount,
                SubTotalAmount = subTotalAmount,
                TaxVat = trips.Where(x => x.TaxVat.HasValue).FirstOrDefault().TaxVat.Value,
                AccountType = InvoiceAccountType.AccountReceivable,
                Channel = InvoiceChannel.Trip,
                Status = InvoiceStatus.Drafted,
                Trips = trips.Select(r => new InvoiceTrip() { TripId = r.Id }).ToList(),
            };
            invoice.Id = await _invoiceRepository.InsertAndGetIdAsync(invoice);

            foreach (var trip in trips)
            {
                trip.IsShipperHaveInvoice = true;
            }
            
        }

        // ToDo: Add Permission for Tms/Host only
        public async Task ConfirmInvoice(long invoiceId)
        {
            DisableTenancyFilters();

            Invoice invoice = await _invoiceRepository.GetAll().Include(x => x.InvoicePeriodsFK)
                .Include(x => x.Tenant)
                .FirstOrDefaultAsync(x => x.Id == invoiceId);
            
            // todo add message here
            if (invoice is null) throw new UserFriendlyException(L("InvoiceNotFound"));

            Tenant tenant = invoice.Tenant;
            InvoicePeriod period = invoice.InvoicePeriodsFK;
            decimal totalAmount = invoice.TotalAmount;

            if (period.PeriodType == InvoicePeriodType.PayInAdvance)
            {
                tenant.Balance -= totalAmount;
                tenant.ReservedBalance -= totalAmount;
            }
            else
            {
                tenant.CreditBalance -= totalAmount;
            }


            await _balanceManager.CheckShipperOverLimit(tenant);
            invoice.Status = InvoiceStatus.Confirmed;
            await _appNotifier.NewInvoiceShipperGenerated(invoice);
        }

        public async Task CorrectShipperInvoice(int invoiceId)
        {

            var invoice = (await _invoiceRepository.GetAll()
                .Include(x=> x.InvoicePeriodsFK)
                .Include(x=> x.Tenant)

                .Include(x=> x.Trips)
                .ThenInclude(x=> x.ShippingRequestTripFK)
                .ThenInclude(x=> x.ShippingRequestTripVases)

                .Include(x => x.Trips)
                .ThenInclude(x => x.ShippingRequestTripFK)
                .ThenInclude(x=> x.ShippingRequestFk)
                                                .Where(x => x.Id == invoiceId)
                                                .FirstOrDefaultAsync());
            var trips =   invoice.Trips.Select(x => x.ShippingRequestTripFK);


            //not saas trips
            var notSaasTrips = trips.Where(x => !x.ShippingRequestFk.IsSaas());

            decimal notSaasTotalAmount = (decimal)notSaasTrips.Sum(r => r.TotalAmountWithCommission + r.ShippingRequestTripVases.Sum(v => v.TotalAmountWithCommission));
            decimal notSaasVatAmount = (decimal)notSaasTrips.Sum(r => r.VatAmountWithCommission + r.ShippingRequestTripVases.Sum(v => v.VatAmountWithCommission));
            decimal notSaaSubTotalAmount = (decimal)notSaasTrips.Sum(r => r.SubTotalAmountWithCommission + r.ShippingRequestTripVases.Sum(v => v.SubTotalAmountWithCommission));

            //saas trips
            var saasTrips = trips.Where(x => x.ShippingRequestFk.IsSaas());

            decimal SaasTotalAmount = (decimal)saasTrips.Sum(r => r.TotalAmountWithCommission);
            decimal SaasVatAmount = (decimal)saasTrips.Sum(r => r.VatAmountWithCommission);
            decimal SaasSubTotalAmount = (decimal)saasTrips.Sum(r => r.SubTotalAmountWithCommission);

            var totalAmount = notSaasTotalAmount + SaasTotalAmount;
            var vatAmount = notSaasVatAmount + SaasVatAmount;
            var subTotalAmount = notSaaSubTotalAmount + SaasSubTotalAmount;




            DateTime dueDate = Clock.Now;

            if (invoice.InvoicePeriodsFK.PeriodType != InvoicePeriodType.PayInAdvance)
            {
                var paymentType = await _invoicePaymentMethodRepository.FirstOrDefaultAsync(x =>
                    x.Id == int.Parse(_featureChecker.GetValue(invoice.TenantId, AppFeatures.InvoicePaymentMethod)));
                if (paymentType.PaymentType == PaymentMethod.InvoicePaymentType.Days)
                {
                    dueDate = Clock.Now.AddDays(paymentType.InvoiceDueDateDays);
                }
            }


            if (invoice.InvoicePeriodsFK.PeriodType == InvoicePeriodType.PayInAdvance)
            {
                invoice.Tenant.Balance += totalAmount;
                invoice.Tenant.ReservedBalance += totalAmount;
            }
            else
            {
                invoice.Tenant.CreditBalance += totalAmount;
            }

            invoice.IsPaid = invoice.InvoicePeriodsFK.PeriodType == InvoicePeriodType.PayInAdvance;
            invoice.TotalAmount = totalAmount;
            invoice.VatAmount = vatAmount;
            invoice.SubTotalAmount = subTotalAmount;
            //invoice.TaxVat = trips.Where(x => x.TaxVat.HasValue).FirstOrDefault().TaxVat.Value;
           // invoice.AccountType = InvoiceAccountType.AccountReceivable;
           // invoice.Channel = InvoiceChannel.Trip;
           // invoice.Trips = trips.Select(r => new InvoiceTrip() { TripId = r.Id }).ToList();
            
           // invoice.Id = await _invoiceRepository.InsertAndGetIdAsync(invoice);

            foreach (var trip in trips)
            {
                trip.IsShipperHaveInvoice = true;
            }

            if (invoice.InvoicePeriodsFK.PeriodType == InvoicePeriodType.PayInAdvance)
            {
                invoice.Tenant.Balance -= totalAmount;
                invoice.Tenant.ReservedBalance -= totalAmount;
            }
            else
            {
                invoice.Tenant.CreditBalance -= totalAmount;
            }

        }


        public async Task GeneratePenaltyInvoice(Tenant tenant, List<Penalty> penalties, InvoicePeriod period)
        {
            decimal totalAmount = penalties.Sum(r => r.TotalAmount);
            decimal vatAmount = penalties.Sum(r => r.VatPostCommestion);
            decimal subTotalAmount = penalties.Sum(r => r.AmountPostCommestion);

            DateTime dueDate = Clock.Now;

            if (period.PeriodType != InvoicePeriodType.PayInAdvance)
            {
                var paymentType = await _invoicePaymentMethodRepository.FirstOrDefaultAsync(x =>
                    x.Id == int.Parse(_featureChecker.GetValue(tenant.Id, AppFeatures.InvoicePaymentMethod)));
                if (paymentType.PaymentType == PaymentMethod.InvoicePaymentType.Days)
                {
                    dueDate = Clock.Now.AddDays(paymentType.InvoiceDueDateDays);
                }
            }
            var invoice = new Invoice
            {
                TenantId = tenant.Id,
                PeriodId = period.Id,
                DueDate = dueDate,
                IsPaid = period.PeriodType == InvoicePeriodType.PayInAdvance,
                TotalAmount = totalAmount,
                VatAmount = vatAmount,
                SubTotalAmount = subTotalAmount,
                AccountType = InvoiceAccountType.AccountReceivable,
                Channel = InvoiceChannel.Penalty,
                Penalties = penalties
            };
            await _invoiceRepository.InsertAsync(invoice);
             invoice.Id = await _invoiceRepository.InsertAndGetIdAsync(invoice);

            if (period.PeriodType == InvoicePeriodType.PayInAdvance)
            {
                tenant.Balance -= totalAmount;
                tenant.ReservedBalance -= totalAmount;
            }
            else
            {
                tenant.CreditBalance -= totalAmount;
            }


            await _balanceManager.CheckShipperOverLimit(tenant);
        }

        public async Task GenerateDynamicInvoice(Tenant tenant, DynamicInvoice dynamicInvoice)
        {
            InvoicePeriod period = await _periodRepository.FirstOrDefaultAsync(x =>
                x.Id == int.Parse(_featureChecker.GetValue(tenant.Id, AppFeatures.ShipperPeriods))); 

            decimal subTotalAmount = dynamicInvoice.Items.Sum(r => r.Price);
            var tax = GetTax();

            decimal vatAmount = subTotalAmount * tax/100;//dynamicInvoice.Items.Sum(r => r.va);
            decimal totalAmount = subTotalAmount + vatAmount;

            DateTime dueDate = Clock.Now;

            var invoice = new Invoice
            {
                TenantId = tenant.Id,
                PeriodId = period.Id,
                DueDate = dueDate,
                IsPaid = false,
                TotalAmount = totalAmount,
                VatAmount = vatAmount,
                SubTotalAmount = subTotalAmount,
                AccountType = InvoiceAccountType.AccountReceivable,
                Channel = InvoiceChannel.DynamicInvoice,
                Note=dynamicInvoice.Notes
            };
            //await _invoiceRepository.InsertAsync(invoice);
            invoice.Id = await _invoiceRepository.InsertAndGetIdAsync(invoice);
            dynamicInvoice.InvoiceId=invoice.Id;
            if (period.PeriodType == InvoicePeriodType.PayInAdvance)
            {
                tenant.Balance -= totalAmount;
                tenant.ReservedBalance -= totalAmount;
            }
            else
            {
                tenant.CreditBalance -= totalAmount;
            }


            await _balanceManager.CheckShipperOverLimit(tenant);
        }

        public async Task GenerateSubmitDynamicInvoice(Tenant tenant, DynamicInvoice dynamicInvoice)
        {

            InvoicePeriod period = await _periodRepository.FirstOrDefaultAsync(x =>
                 x.Id == int.Parse(_featureChecker.GetValue(tenant.Id, AppFeatures.ShipperPeriods)));

            decimal subTotalAmount = dynamicInvoice.Items.Sum(r => r.Price);
            var tax = GetTax();

            decimal vatAmount = subTotalAmount * tax / 100;//dynamicInvoice.Items.Sum(r => r.va);
            decimal totalAmount = subTotalAmount + vatAmount;

            DateTime dueDate = Clock.Now;

            var submitInvoice = new SubmitInvoice
            {
                TenantId = tenant.Id,
                PeriodId = period.Id,
                TotalAmount = totalAmount,
                VatAmount = vatAmount,
                SubTotalAmount = subTotalAmount,
                TaxVat = tax,
                Channel = InvoiceChannel.DynamicInvoice,
                
            };
            submitInvoice.Id = await _submitInvoiceRepository.InsertAndGetIdAsync(submitInvoice);
            dynamicInvoice.SubmitInvoiceId=submitInvoice.Id;

        }
        public async Task GeneratePenaltySubmitInvoice(Tenant tenant, List<Penalty> penalties, InvoicePeriod period)
        {
            decimal vatAmount = penalties.Sum(r => r.VatAmount);
            decimal subTotalAmount = penalties.Sum(r => r.AmountPreCommestion);
            decimal totalAmount = penalties.Sum(r => r.AmountPreCommestion + r.VatAmount);


            var submitInvoice = new SubmitInvoice
            {
                TenantId = tenant.Id,
                PeriodId = period.Id,
                TotalAmount = totalAmount,
                VatAmount = vatAmount,
                SubTotalAmount = subTotalAmount,
                TaxVat = penalties.Where(x => x.TaxVat > 0).FirstOrDefault().TaxVat,
                Channel = InvoiceChannel.Penalty,
                Penalties = penalties
            };
            submitInvoice.Id = await _submitInvoiceRepository.InsertAndGetIdAsync(submitInvoice);



        }

        /// <summary>
        /// When the shipper billing interval  after delivry run this method to generate invoice
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task GenertateInvoiceWhenShipmintDelivery(ShippingRequestTrip trip)
        {
            var tenant = trip.ShippingRequestFk.Tenant;
            InvoicePeriod period = default;
            ///If the shipemnt pay in advance get the period entity for pay in advance else get from the features
            if (trip.ShippingRequestFk.IsPrePayed.HasValue && trip.ShippingRequestFk.IsPrePayed.Value)
                period = await _periodRepository.FirstOrDefaultAsync(
                    x => x.PeriodType == InvoicePeriodType.PayInAdvance);
            else
                period = await _periodRepository.FirstOrDefaultAsync(x =>
                    x.Id == int.Parse(_featureChecker.GetValue(tenant.Id, AppFeatures.ShipperPeriods)));

            if (period.PeriodType == InvoicePeriodType.PayuponDelivery ||
                period.PeriodType == InvoicePeriodType.PayInAdvance)
                await GenerateShipperInvoice(tenant, new List<ShippingRequestTrip>() { trip }, period);
        }


        public async Task GenertateInvoiceOnDeman(Tenant tenant, List<int> tripIdsList)
        {
            InvoicePeriod period = await _periodRepository.FirstOrDefaultAsync(x =>
                x.Id == int.Parse(_featureChecker.GetValue(tenant.Id, AppFeatures.ShipperPeriods)));


            if (period.PeriodType != InvoicePeriodType.PayuponDelivery &&
                period.PeriodType != InvoicePeriodType.PayInAdvance)
            {
                var trips = _shippingRequestTrip.GetAll()
                    .Include(x => x.ShippingRequestFk)
                    .Include(x => x.ShippingRequestTripVases)
                    .Where(x => x.ShippingRequestFk.TenantId == tenant.Id &&
                    tripIdsList.Contains(x.Id) &&
                    !x.IsShipperHaveInvoice &&
                    //waybills.Contains(x.Id) &&
                    (x.Status == Shipping.Trips.ShippingRequestTripStatus.Delivered ||
                    x.InvoiceStatus == InvoiceTripStatus.CanBeInvoiced));
                if (trips != null && trips.Count() > 0)
                {
                    await GenerateShipperInvoice(tenant, trips.ToList(), period);
                }
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

            if (!IsTenantCarrier(invoice.TenantId))
            {
                if (invoice.IsPaid) await _balanceManager.AddBalanceToShipper(invoice.TenantId, -invoice.TotalAmount);
                invoice.Trips.ToList().ForEach(t =>
                {
                    t.ShippingRequestTripFK.IsShipperHaveInvoice = false;
                });
            }
            else
            {
                if (invoice.IsPaid) await _balanceManager.AddBalanceToCarrier(invoice.TenantId, -invoice.TotalAmount);
            }

            await _invoiceRepository.DeleteAsync(invoice);
        }


        public async Task<Invoice> GetInvoiceInfo(long invoiceId)
        {
            return await _invoiceRepository
                .GetAll()
                .Include(i => i.Trips)
                .ThenInclude(t => t.ShippingRequestTripFK)
                .FirstOrDefaultAsync(i => i.Id == invoiceId);
        }

        public bool IsTenantCarrier(int tenantId)
        {
            return _featureChecker.IsEnabled(tenantId, AppFeatures.Carrier);
        }

        private decimal GetTax()
        {
            return _settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat);
        }
    }
}