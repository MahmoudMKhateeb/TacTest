using Abp.Application.Features;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Repositories;
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
using TACHYON.DedicatedDynamicInvoices.DedicatedDynamicInvoiceItems;
using TACHYON.DedicatedDynamicInvoices.Dtos;
using TACHYON.DedicatedInvoices;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Invoices;
using TACHYON.MultiTenancy;
using TACHYON.PriceOffers;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;

namespace TACHYON.DedicatedDynamicInvoices
{
    [AbpAuthorize(AppPermissions.Pages_DedicatedDynamicInvoices)]
    [RequiresFeature(AppFeatures.TachyonDealer)]
    public class DedicatedDynamiceInvoicesAppService : TACHYONAppServiceBase
    {
        private readonly IRepository<DedicatedDynamicInvoice, long> _dedicatedInvoiceRepository;
        private readonly IRepository<DedicatedDynamicInvoiceItem, long> _dedicatedInvoiceItemRepository;
        private readonly ISettingManager _settingManager;
        private readonly IRepository<PriceOffer, long> _priceOfferRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly TenantManager _tenantManager;
        private readonly InvoiceManager _invoiceManager;
        public DedicatedDynamiceInvoicesAppService(IRepository<DedicatedDynamicInvoice, long> dedicatedInvoiceRepository, ISettingManager settingManager, IRepository<PriceOffer, long> priceOfferRepository, IRepository<ShippingRequest, long> shippingRequestRepository, TenantManager tenantManager, IRepository<DedicatedDynamicInvoiceItem, long> dedicatedInvoiceItemRepository, InvoiceManager invoiceManager)
        {
            _dedicatedInvoiceRepository = dedicatedInvoiceRepository;
            _settingManager = settingManager;
            _priceOfferRepository = priceOfferRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _tenantManager = tenantManager;
            _dedicatedInvoiceItemRepository = dedicatedInvoiceItemRepository;
            _invoiceManager = invoiceManager;
        }

        public async Task<LoadResult> GetAll(string filter)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            await DisableInvoiceDraftedFilter();
            var query = _dedicatedInvoiceRepository.GetAll()
                .ProjectTo<DedicatedDynamicInvoiceDto>(AutoMapperConfigurationProvider)
            .AsNoTracking();

            return await LoadResultAsync(query, filter);
        }
        //get D trucks by company

        public async Task CreateOrEdit(CreateOrEditDedicatedInvoiceDto input)
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
        [AbpAuthorize(AppPermissions.Pages_DedicatedDynamicInvoices_Update)]
        public async Task<CreateOrEditDedicatedInvoiceDto> GetDedicatedInvoiceForEdit(long id)
        {
            var invoice = await _dedicatedInvoiceRepository.GetAllIncluding(x => x.DedicatedDynamicInvoiceItems).FirstOrDefaultAsync(x => x.Id == id);
            return ObjectMapper.Map<CreateOrEditDedicatedInvoiceDto>(invoice);
        }

        [AbpAuthorize(AppPermissions.Pages_DedicatedDynamicInvoices_Delete)]
        public async Task Delete(long id)
        {
            var invoice = await _dedicatedInvoiceRepository.GetAllIncluding(x => x.DedicatedDynamicInvoiceItems).FirstOrDefaultAsync(x => x.Id == id);
            if (DedicatedInvoiceGenerated(invoice))
                throw new UserFriendlyException(L("DeleteNotAllowed"));

            await _dedicatedInvoiceRepository.DeleteAsync(invoice);
        }

        public async Task GenerateDedicatedInvoice(long id)
        {
            DisableTenancyFilters();
            await DisableInvoiceDraftedFilter();
            var invoice = await _dedicatedInvoiceRepository
                .GetAllIncluding(x => x.Tenant, x=>x.ShippingRequest)
                .Include(x=> x.DedicatedDynamicInvoiceItems)
                .ThenInclude(x=>x.DedicatedShippingRequestTruck)
                .ThenInclude(x=>x.Truck)
                .Include(x=>x.Invoice)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (invoice.InvoiceAccountType == InvoiceAccountType.AccountReceivable)
            {
                if (invoice.Invoice != null) throw new UserFriendlyException(L("InvoiceAlreadyGenerated"));
                if(invoice.DedicatedDynamicInvoiceItems.Any(x=>x.DedicatedShippingRequestTruck.InvoiceId != null))
                {
                    throw new UserFriendlyException(L(string.Format("InvoiceAlreadyGeneratedForTruck{0}",invoice.DedicatedDynamicInvoiceItems.First(x => x.DedicatedShippingRequestTruck.InvoiceId != null).DedicatedShippingRequestTruck.Truck.GetDisplayName())));
                }
                await _invoiceManager.GenerateDedicatedDynamicInvoice(invoice.Tenant, invoice);
            }
            else
            {
                if (invoice.SubmitInvoiceId != null) throw new UserFriendlyException(L("InvoiceAlreadyGenerated"));
                if (invoice.DedicatedDynamicInvoiceItems.Any(x => x.DedicatedShippingRequestTruck.SubmitInvoiceId != null))
                {
                    throw new UserFriendlyException(L(string.Format("InvoiceAlreadyGeneratedForTruck{0}",invoice.DedicatedDynamicInvoiceItems.First(x => x.DedicatedShippingRequestTruck.SubmitInvoiceId != null).DedicatedShippingRequestTruck.Truck.GetDisplayName())));
                }
                await _invoiceManager.GenerateSubmitDedicatedDynamicInvoice(invoice.Tenant, invoice);
            }
        }

        #region Helper
        [AbpAuthorize(AppPermissions.Pages_DedicatedDynamicInvoices_Create)]
        private async Task Create(CreateOrEditDedicatedInvoiceDto input)
        {
            var invoice = ObjectMapper.Map<DedicatedDynamicInvoice>(input);
            await _dedicatedInvoiceRepository.InsertAsync(invoice);
        }

        [AbpAuthorize(AppPermissions.Pages_DedicatedDynamicInvoices_Update)]
        private async Task Update(CreateOrEditDedicatedInvoiceDto input)
        {
            var invoice = await _dedicatedInvoiceRepository.GetAllIncluding(x => x.DedicatedDynamicInvoiceItems).SingleAsync(x => x.Id == input.Id.Value);
            if (DedicatedInvoiceGenerated(invoice))
            {
                throw new UserFriendlyException(L("UpdateNotAllowed"));
            }
            await RemoveDeletedItems(input, invoice);

            ObjectMapper.Map(input, invoice);
        }

        private static bool DedicatedInvoiceGenerated(DedicatedDynamicInvoice invoice)
        {
            return (invoice.InvoiceAccountType == Invoices.InvoiceAccountType.AccountReceivable && invoice.InvoiceId != null) ||
                            (invoice.InvoiceAccountType == Invoices.InvoiceAccountType.AccountPayable && invoice.SubmitInvoiceId != null);
        }

        private async Task RemoveDeletedItems(CreateOrEditDedicatedInvoiceDto input, DedicatedDynamicInvoice invoice)
        {
            foreach (var item in invoice.DedicatedDynamicInvoiceItems)
            {
                if (!input.DedicatedInvoiceItems.Any(x => x.Id == item.Id))
                {
                    await _dedicatedInvoiceItemRepository.DeleteAsync(item);
                }
            }
        }

        private async Task CalculateAmounts(CreateOrEditDedicatedInvoiceDto input)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            var price = await _priceOfferRepository.GetAll().Where(x => x.ShippingRequestId == input.ShippingRequestId &&
            x.Status == PriceOfferStatus.Accepted).FirstOrDefaultAsync();
            if (price == null) throw new UserFriendlyException(L("NoConfirmedOfferExists"));

            //var pricePerDay =await GetDedicatePricePerDay(input.ShippingRequestId,input.InvoiceAccountType, price);
            foreach (var item in input.DedicatedInvoiceItems)
            {
                var pricePerDay = GetTruckPricePerDay(input.InvoiceAccountType, item.AllNumberDays, price);
                item.PricePerDay = pricePerDay;
                item.ItemSubTotalAmount = item.NumberOfDays * item.PricePerDay;
                item.TaxVat = GetTaxVat();
                item.VatAmount = item.ItemSubTotalAmount * GetTaxVat() / 100;
                item.ItemTotalAmount = item.ItemSubTotalAmount + item.VatAmount;
            }

            input.SubTotalAmount = input.DedicatedInvoiceItems.Sum(x => x.ItemSubTotalAmount);
            input.VatAmount = input.SubTotalAmount * GetTaxVat() / 100;
            input.TotalAmount = input.SubTotalAmount + input.VatAmount;
        }

        public decimal GetTaxVat()
        {
            return _settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat);
        }

        private decimal GetTruckPricePerDay(InvoiceAccountType invoiceAccountType, int AllNumberOfDays, PriceOffer price)
        {
            if (AllNumberOfDays > 0) return (invoiceAccountType == InvoiceAccountType.AccountReceivable ? price.ItemSubTotalAmountWithCommission : price.ItemPrice) / AllNumberOfDays;
            return 0;
        }

        /// <summary>
        /// Helper for front
        /// </summary>
        /// <param name="ShippingRequestId"></param>
        /// <param name="invoiceAccountType"></param>
        /// <param name="AllNumberOfDays"></param>
        /// <returns></returns>
        public async Task<decimal> GetDedicatePricePerDay(long ShippingRequestId, InvoiceAccountType invoiceAccountType, int AllNumberOfDays)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            var price = await _priceOfferRepository.GetAll().Where(x => x.ShippingRequestId == ShippingRequestId &&
            x.Status == PriceOfferStatus.Accepted).FirstOrDefaultAsync();
            if (AllNumberOfDays > 0) return (invoiceAccountType == InvoiceAccountType.AccountReceivable ? price.ItemSubTotalAmountWithCommission : price.ItemPrice) / AllNumberOfDays;
            return 0;
        }

        public async Task<int> GetDefaultNumberOfDays(long ShippingRequestId)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            var shippingRequest = await _shippingRequestRepository.FirstOrDefaultAsync(x => x.Id == ShippingRequestId &&
            x.ShippingRequestFlag == ShippingRequestFlag.Dedicated);

            switch (shippingRequest.RentalDurationUnit)
            {
                case TimeUnit.Daily:
                    return shippingRequest.RentalDuration;
                case TimeUnit.Monthly:
                    return shippingRequest.RentalDuration * 26;
                case TimeUnit.Weekly:
                    return shippingRequest.RentalDuration * 7;
                default:
                    return 0;
            }
        }
        #endregion
    }
}
