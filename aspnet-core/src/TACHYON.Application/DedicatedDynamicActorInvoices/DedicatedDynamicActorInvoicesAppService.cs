using Abp.Application.Features;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Timing;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Configuration;
using TACHYON.DedicatedDynamicActorInvoices.DedicatedDynamicActorInvoiceItems;
using TACHYON.DedicatedDynamicInvoices;
using TACHYON.DedicatedDynamicInvoices.Dtos;
using TACHYON.DedicatedInvoices;
using TACHYON.DedidcatedDynamicActorInvoices;
using TACHYON.DedidcatedDynamicActorInvoices.Dtos;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Invoices;
using TACHYON.Invoices.ActorInvoices;
using TACHYON.PriceOffers;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.DedicatedDynamicActorInvoices
{
    [RequiresFeature(AppFeatures.ShipperClients, AppFeatures.CarrierClients)]
    public class DedicatedDynamicActorInvoicesAppService: TACHYONAppServiceBase, IDedicatedDynamiceActorInvoicesAppService
    {
        private readonly IRepository<DedicatedDynamicActorInvoice, long> _dedicatedActorInvoiceRepository;
        private readonly IRepository<DedicatedDynamicActorInvoiceItem, long> _dedicatedActorInvoiceItemRepository;
        private readonly ISettingManager _settingManager;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly ActorInvoicesManager _actorInvoicesManager;
        private readonly DedicatedDynamicInvoiceManager _dedicatedDynamicInvoiceManager;



        public DedicatedDynamicActorInvoicesAppService(IRepository<DedicatedDynamicActorInvoice, long> dedicatedActorInvoiceRepository,
            IRepository<DedicatedDynamicActorInvoiceItem, long> dedicatedActorInvoiceItemRepository,
            ISettingManager settingManager,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            ActorInvoicesManager actorInvoicesManager, DedicatedDynamicInvoiceManager dedicatedDynamicInvoiceManager)
        {
            _dedicatedActorInvoiceRepository = dedicatedActorInvoiceRepository;
            _dedicatedActorInvoiceItemRepository = dedicatedActorInvoiceItemRepository;
            _settingManager = settingManager;
            _shippingRequestRepository = shippingRequestRepository;
            _actorInvoicesManager = actorInvoicesManager;
            _dedicatedDynamicInvoiceManager = dedicatedDynamicInvoiceManager;
        }
        [AbpAuthorize(AppPermissions.Pages_DedicatedDynamicActorInvoices)]
        public async Task<LoadResult> GetAll(string filter)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            await DisableInvoiceDraftedFilter();
            var query = _dedicatedActorInvoiceRepository.GetAll()
                .ProjectTo<DedicatedDynamicActorInvoiceDto>(AutoMapperConfigurationProvider)
            .AsNoTracking();

            return await LoadResultAsync(query, filter);
        }

        public async Task CreateOrEdit(CreateOrEditDedicatedActorInvoiceDto input)
        {
            await CalculateAmounts(input);
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }
        [AbpAuthorize(AppPermissions.Pages_DedicatedDynamicActorInvoices_Update)]
        public async Task<CreateOrEditDedicatedActorInvoiceDto> GetDedicatedInvoiceForEdit(long id)
        {
            var invoice = await _dedicatedActorInvoiceRepository.GetAllIncluding(x => x.DedicatedDynamicActorInvoiceItems).FirstOrDefaultAsync(x => x.Id == id);
            return ObjectMapper.Map<CreateOrEditDedicatedActorInvoiceDto>(invoice);
        }

        [AbpAuthorize(AppPermissions.Pages_DedicatedDynamicActorInvoices_Delete)]
        public async Task Delete(long id)
        {
            var invoice = await _dedicatedActorInvoiceRepository.GetAllIncluding(x => x.DedicatedDynamicActorInvoiceItems).FirstOrDefaultAsync(x => x.Id == id);
            if (DedicatedInvoiceGenerated(invoice))
                throw new UserFriendlyException(L("DeleteNotAllowed"));

            await _dedicatedActorInvoiceRepository.DeleteAsync(invoice);
        }

        public async Task GenerateDedicatedInvoice(long id)
        {
            DisableTenancyFilters();
            await DisableInvoiceDraftedFilter();
            var dedicatedInvoice = await _dedicatedActorInvoiceRepository
                .GetAllIncluding(x => x.Tenant, x => x.ShippingRequest)
                .Include(x => x.DedicatedDynamicActorInvoiceItems)
                .ThenInclude(x => x.DedicatedShippingRequestTruck)
                .ThenInclude(x => x.Truck)
                .Include(x => x.ActorInvoice)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dedicatedInvoice.InvoiceAccountType == InvoiceAccountType.AccountReceivable)
            {
                if (dedicatedInvoice.ActorInvoice != null) throw new UserFriendlyException(L("InvoiceAlreadyGenerated"));
                if (dedicatedInvoice.DedicatedDynamicActorInvoiceItems.Any(x => x.DedicatedShippingRequestTruck.ActorInvoiceId != null))
                {
                    throw new UserFriendlyException(L(string.Format("InvoiceAlreadyGeneratedForTruck{0}", dedicatedInvoice.DedicatedDynamicActorInvoiceItems.First(x => x.DedicatedShippingRequestTruck.ActorInvoiceId != null).DedicatedShippingRequestTruck.Truck.GetDisplayName())));
                }
                await _actorInvoicesManager.GenerateDedicatedDynamicActorInvoice(dedicatedInvoice);
            }
            else
            {
                if (dedicatedInvoice.ActorSubmitInvoice != null) throw new UserFriendlyException(L("InvoiceAlreadyGenerated"));
                if (dedicatedInvoice.DedicatedDynamicActorInvoiceItems.Any(x => x.DedicatedShippingRequestTruck.ActorSubmitInvoiceId != null))
                {
                    throw new UserFriendlyException(L(string.Format("InvoiceAlreadyGeneratedForTruck{0}", dedicatedInvoice.DedicatedDynamicActorInvoiceItems.First(x => x.DedicatedShippingRequestTruck.ActorSubmitInvoiceId != null).DedicatedShippingRequestTruck.Truck.GetDisplayName())));
                }
                //await _invoiceManager.GenerateSubmitDedicatedDynamicInvoice(invoice.Tenant, invoice);
                await _actorInvoicesManager.GenerateDedicatedDynamicActorSubmitInvoice(dedicatedInvoice);
            }
        }

        public async Task<decimal> GetDedicatePricePerDay(long ShippingRequestId, InvoiceAccountType invoiceAccountType, int AllNumberOfDays)
        {
            var requestActorPrices = await _shippingRequestRepository.GetAll()
                .Select(x => new {
                    id = x.Id,
                    shipperPrice = x.ActorShipperPrice.SubTotalAmountWithCommission,
                    carrierPrice = x.ActorCarrierPrice != null ? x.ActorCarrierPrice.SubTotalAmount : null
                })
                .FirstOrDefaultAsync(y => y.id == ShippingRequestId);

            return GetTruckPricePerDay(invoiceAccountType, AllNumberOfDays, requestActorPrices.shipperPrice, requestActorPrices.carrierPrice);
        }

        public async Task<int> GetDefaultNumberOfDays(long ShippingRequestId)
        {
            return await _dedicatedDynamicInvoiceManager._getDefaultNumberOfDays(ShippingRequestId);
        }
        #region DropDowns

        public async Task<List<SelectItemDto>> GetDedicatedRequestsByActor(int actorId)
        {
            DisableTenancyFilters();
            return await _shippingRequestRepository.GetAll()
                .Where(x => x.TenantId == AbpSession.TenantId &&
                (x.ShipperActorId == actorId || x.CarrierActorId == actorId) &&
            x.Status != ShippingRequestStatus.PrePrice &&
            x.ShippingRequestFlag == ShippingRequestFlag.Dedicated)
                .Select(x => new SelectItemDto
                {
                    DisplayName = x.ReferenceNumber,
                    Id = x.Id.ToString()
                }).ToListAsync();
        }
        #endregion

        #region Helper
        [AbpAuthorize(AppPermissions.Pages_DedicatedDynamicActorInvoices_Create)]
        private async Task Create(CreateOrEditDedicatedActorInvoiceDto input)
        {
            var invoice = ObjectMapper.Map<DedicatedDynamicActorInvoice>(input);
            await _dedicatedActorInvoiceRepository.InsertAsync(invoice);
        }

        [AbpAuthorize(AppPermissions.Pages_DedicatedDynamicActorInvoices_Update)]
        private async Task Update(CreateOrEditDedicatedActorInvoiceDto input)
        {
            var invoice = await _dedicatedActorInvoiceRepository.GetAllIncluding(x => x.DedicatedDynamicActorInvoiceItems).SingleAsync(x => x.Id == input.Id.Value);
            if (DedicatedInvoiceGenerated(invoice))
            {
                throw new UserFriendlyException(L("UpdateNotAllowed"));
            }
            await RemoveDeletedItems(input, invoice);

            ObjectMapper.Map(input, invoice);
        }

        private async Task CalculateAmounts(CreateOrEditDedicatedActorInvoiceDto input)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();

            var requestActorPrices = await _shippingRequestRepository.GetAll()
                .Select(x=> new {id=x.Id, shipperPrice =x.ActorShipperPrice , 
                    carrierPrice = x.ActorCarrierPrice!=null ?x.ActorCarrierPrice :null })
                .FirstOrDefaultAsync(y => y.id == input.ShippingRequestId);

            if (requestActorPrices == null) throw new UserFriendlyException(L("NoConfirmedOfferExistsForClients"));

            //var pricePerDay =await GetDedicatePricePerDay(input.ShippingRequestId,input.InvoiceAccountType, price);
            foreach (var item in input.DedicatedActorInvoiceItems)
            {
                if(item.WorkingDayType == DedicatedDynamicInvocies.WorkingDayType.Normal)
                {
                    var pricePerDay = GetTruckPricePerDay(input.InvoiceAccountType, item.AllNumberDays, requestActorPrices.shipperPrice.SubTotalAmountWithCommission, requestActorPrices.carrierPrice?.SubTotalAmount );
                    item.PricePerDay = pricePerDay;
                }

                item.ItemSubTotalAmount = item.NumberOfDays * item.PricePerDay;
                item.TaxVat = GetTaxVat();
                item.VatAmount = item.ItemSubTotalAmount * GetTaxVat() / 100;
                item.ItemTotalAmount = item.ItemSubTotalAmount + item.VatAmount;
            }

            input.SubTotalAmount = input.DedicatedActorInvoiceItems.Sum(x => x.ItemSubTotalAmount);
            input.VatAmount = input.SubTotalAmount * GetTaxVat() / 100;
            input.TotalAmount = input.SubTotalAmount + input.VatAmount;
        }

        private decimal GetTruckPricePerDay(InvoiceAccountType invoiceAccountType, int AllNumberOfDays, decimal? shipperPrice, decimal? carrierPrice)
        {
            if (invoiceAccountType == InvoiceAccountType.AccountReceivable && shipperPrice == null) throw new UserFriendlyException(L("ShipperActorPriceDoesn'tExists"));
            if (invoiceAccountType == InvoiceAccountType.AccountPayable && carrierPrice == null) throw new UserFriendlyException(L("CarrierActorPriceDoesn'tExists"));

            if (AllNumberOfDays > 0) return (invoiceAccountType == InvoiceAccountType.AccountReceivable ? shipperPrice.Value : carrierPrice.Value) / AllNumberOfDays;
            return 0;
        }



        public decimal GetTaxVat()
        {
            return _settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat);
        }


        private static bool DedicatedInvoiceGenerated(DedicatedDynamicActorInvoice invoice)
        {
            return (invoice.InvoiceAccountType == Invoices.InvoiceAccountType.AccountReceivable && invoice.ActorInvoiceId != null) ||
                            (invoice.InvoiceAccountType == Invoices.InvoiceAccountType.AccountPayable && invoice.ActorInvoiceId != null);
        }

        private async Task RemoveDeletedItems(CreateOrEditDedicatedActorInvoiceDto input, DedicatedDynamicActorInvoice invoice)
        {
            foreach (var item in invoice.DedicatedDynamicActorInvoiceItems)
            {
                if (!input.DedicatedActorInvoiceItems.Any(x => x.Id == item.Id))
                {
                    await _dedicatedActorInvoiceItemRepository.DeleteAsync(item);
                }
            }
        }


        //private async Task GenerateDedicatedDynamicActorInvoice(Tenant tenant, DedicatedDynamicInvoice dedicatedDynamicInvoice)
        //{
        //    decimal subTotalAmount = dedicatedDynamicInvoice.SubTotalAmount;
        //    var tax = 15;

        //    decimal vatAmount = dedicatedDynamicInvoice.VatAmount;
        //    decimal totalAmount = dedicatedDynamicInvoice.TotalAmount;

        //    DateTime dueDate = Clock.Now;

        //    var invoice = new Invoice
        //    {
        //        TenantId = tenant.Id,
        //        PeriodId = period.Id,
        //        DueDate = dueDate,
        //        IsPaid = false,
        //        TotalAmount = totalAmount,
        //        VatAmount = vatAmount,
        //        SubTotalAmount = subTotalAmount,
        //        AccountType = InvoiceAccountType.AccountReceivable,
        //        Channel = InvoiceChannel.Dedicated,
        //        Status = InvoiceStatus.Drafted,
        //        Note = dedicatedDynamicInvoice.Notes
        //    };
        //    //await _invoiceRepository.InsertAsync(invoice);
        //    invoice.Id = await _invoiceRepository.InsertAndGetIdAsync(invoice);
        //    dedicatedDynamicInvoice.InvoiceId = invoice.Id;
        //    //dedicatedDynamicInvoice.ShippingRequest.IsShipperHaveInvoice = true;
        //    dedicatedDynamicInvoice.DedicatedDynamicInvoiceItems.ForEach(x =>
        //    x.DedicatedShippingRequestTruck.InvoiceId = invoice.Id);
        //}
            #endregion
    }
}
