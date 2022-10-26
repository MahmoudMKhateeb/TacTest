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
using TACHYON.Configuration;
using TACHYON.DedicatedDynamicInvoices.DedicatedDynamicInvoiceItems;
using TACHYON.DedicatedDynamicInvoices.Dtos;
using TACHYON.DedicatedInvoices;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.MultiTenancy;
using TACHYON.PriceOffers;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;

namespace TACHYON.DedicatedDynamicInvoices
{
    [AbpAuthorize]
    [RequiresFeature(AppFeatures.TachyonDealer)]
    public class DedicatedDynamiceInvoicesAppService : TACHYONAppServiceBase
    {
        private readonly IRepository<DedicatedDynamicInvoice, long> _dedicatedInvoiceRepository;
        private readonly IRepository<DedicatedDynamicInvoiceItem, long> _dedicatedInvoiceItemRepository;
        private readonly ISettingManager _settingManager;
        private readonly IRepository<PriceOffer, long> _priceOfferRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly TenantManager _tenantManager;
        public DedicatedDynamiceInvoicesAppService(IRepository<DedicatedDynamicInvoice, long> dedicatedInvoiceRepository, ISettingManager settingManager, IRepository<PriceOffer, long> priceOfferRepository, IRepository<ShippingRequest, long> shippingRequestRepository, TenantManager tenantManager, IRepository<DedicatedDynamicInvoiceItem, long> dedicatedInvoiceItemRepository)
        {
            _dedicatedInvoiceRepository = dedicatedInvoiceRepository;
            _settingManager = settingManager;
            _priceOfferRepository = priceOfferRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _tenantManager = tenantManager;
            _dedicatedInvoiceItemRepository = dedicatedInvoiceItemRepository;
        }

        public async Task<LoadResult> GetAll(string filter)
        {
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

        public async Task<CreateOrEditDedicatedInvoiceDto> GetDedicatedInvoiceForEdit(long id)
        {
            var invoice = await _dedicatedInvoiceRepository.GetAllIncluding(x => x.DedicatedDynamicInvoiceItems).FirstOrDefaultAsync(x=>x.Id == id);
            return ObjectMapper.Map<CreateOrEditDedicatedInvoiceDto>(invoice);
        }

        #region Helper
        private async Task Create(CreateOrEditDedicatedInvoiceDto input)
        {
            var invoice = ObjectMapper.Map<DedicatedDynamicInvoice>(input);
            await _dedicatedInvoiceRepository.InsertAsync(invoice);
        }

        private async Task Update(CreateOrEditDedicatedInvoiceDto input)
        {
            var invoice =await _dedicatedInvoiceRepository.GetAllIncluding(x=>x.DedicatedDynamicInvoiceItems).SingleAsync(x=>x.Id == input.Id.Value);
            await RemoveDeletedItems(input, invoice);

            ObjectMapper.Map(input, invoice);
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
            var pricePerDay =await GetDedicatePricePerDay(input.ShippingRequestId, input.TenantId);
            foreach (var item in input.DedicatedInvoiceItems)
            {
                item.PricePerDay = pricePerDay;
                item.ItemSubTotalAmount = item.NumberOfDays * item.PricePerDay;
                item.TaxVat = GetTaxVat();
                item.VatAmount = item.ItemSubTotalAmount * GetTaxVat()/100;
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

        public async Task<decimal> GetDedicatePricePerDay(long ShippingRequestId, int tenantId)
        {
            var edition = _tenantManager.GetById(tenantId).EditionId;
            var price =await _priceOfferRepository.GetAll().Where(x => x.ShippingRequestId == ShippingRequestId &&
            x.Status == PriceOfferStatus.Accepted).FirstOrDefaultAsync();

            if (price == null) throw new UserFriendlyException(L("NoConfirmedOfferExists"));

            var shippingRequest = await _shippingRequestRepository.FirstOrDefaultAsync(x => x.Id == ShippingRequestId &&
            x.ShippingRequestFlag == ShippingRequestFlag.Dedicated);
            int AllDays = GetNumberOfDays(shippingRequest);

            if (AllDays > 0) return (edition == ShipperEditionId ?price.TotalAmountWithCommission :price.ItemPrice) / AllDays;
            return 0;
        }

        private int GetNumberOfDays(ShippingRequest shippingRequest)
        {
            switch (shippingRequest.RentalDurationUnit)
            {
                case TimeUnit.Daily:
                    return shippingRequest.RentalDuration;
                case TimeUnit.Monthly:
                    return shippingRequest.RentalDuration * 26;
                case TimeUnit.Weekly:
                    return shippingRequest.RentalDuration * 6;
                default:
                    return 0;
            }
        }
        #endregion
    }
}
