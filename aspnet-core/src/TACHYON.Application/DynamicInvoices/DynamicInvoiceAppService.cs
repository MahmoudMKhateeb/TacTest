using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Configuration;
using TACHYON.DynamicInvoices.Dto;
using TACHYON.DynamicInvoices.DynamicInvoiceItems;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.DynamicInvoices
{

    [AbpAuthorize(AppPermissions.Pages_DynamicInvoices)]
    public class DynamicInvoiceAppService : TACHYONAppServiceBase, IDynamicInvoiceAppService
    {
        private readonly IRepository<DynamicInvoice,long> _dynamicInvoiceRepository;
        private readonly IRepository<DynamicInvoiceItem,long> _dynamicInvoiceItemRepository;
        private readonly IRepository<ShippingRequestTrip> _tripRepository;

        public DynamicInvoiceAppService(
            IRepository<DynamicInvoice,long> dynamicInvoiceRepository,
            IRepository<DynamicInvoiceItem, long> dynamicInvoiceItemRepository,
            IRepository<ShippingRequestTrip> tripRepository)
        {
            _dynamicInvoiceRepository = dynamicInvoiceRepository;
            _dynamicInvoiceItemRepository = dynamicInvoiceItemRepository;
            _tripRepository = tripRepository;
        }

        public async Task<PagedResultDto<DynamicInvoiceListDto>> GetAll(GetDynamicInvoicesInput input)
        {
            DisableTenancyFilters();
            
            var dynamicInvoices = _dynamicInvoiceRepository.GetAll()
                .WhereIf(!input.Filter.IsNullOrEmpty(),
                    x => x.DebitTenant.Name.Contains(input.Filter) || x.DebitTenant.companyName.Contains(input.Filter))
                .WhereIf(!input.Filter.IsNullOrEmpty(),
                    x => x.CreditTenant.Name.Contains(input.Filter) ||
                         x.CreditTenant.companyName.Contains(input.Filter))
                .OrderBy(input.Sorting ?? "Id Desc")
                .ProjectTo<DynamicInvoiceListDto>(AutoMapperConfigurationProvider);


            var pageResult = await dynamicInvoices.PageBy(input).ToListAsync();

            return new PagedResultDto<DynamicInvoiceListDto>()
            {
                Items = pageResult, TotalCount = await dynamicInvoices.CountAsync(),
            };
        }

        public async Task<DynamicInvoiceForViewDto> GetForView(long dynamicInvoiceId)
        {
            DisableTenancyFilters();
            
            var dynamicInvoice = await _dynamicInvoiceRepository.GetAllIncluding(x => x.Items)
                .ProjectTo<DynamicInvoiceForViewDto>(AutoMapperConfigurationProvider).SingleAsync(x=> x.Id == dynamicInvoiceId);

            return dynamicInvoice;
        }

        
        public async Task CreateOrEdit(CreateOrEditDynamicInvoiceDto input)
        {
            DisableTenancyFilters();
            
            if (input.Id.HasValue)
            {
                await Update(input);
                return;
            }

            await Create(input);
        }
        
        [AbpAuthorize(AppPermissions.Pages_DynamicInvoices_Create)]
        protected virtual async Task Create(CreateOrEditDynamicInvoiceDto input)
        {
            var createdDynamicInvoice = ObjectMapper.Map<DynamicInvoice>(input);

            var taxVat = await SettingManager.GetSettingValueAsync<decimal>(AppSettings.HostManagement.TaxVat);
            createdDynamicInvoice.SubTotalAmount = createdDynamicInvoice.Items.Sum(x => x.Price);
            createdDynamicInvoice.VatAmount = createdDynamicInvoice.SubTotalAmount * taxVat;
            createdDynamicInvoice.TotalAmount = createdDynamicInvoice.SubTotalAmount + createdDynamicInvoice.VatAmount;
            
            
            var dynamicInvoiceId = await _dynamicInvoiceRepository.InsertAndGetIdAsync(createdDynamicInvoice);

            var itemsList = (from item in input.Items
                from trip in _tripRepository.GetAll().Where(x=> x.WaybillNumber == item.WaybillNumber).DefaultIfEmpty()
                select new {item.WaybillNumber, TripId = trip?.Id}).ToList();

            foreach (var item in input.Items)
            {
                var createdItem = ObjectMapper.Map<DynamicInvoiceItem>(item);
                createdItem.DynamicInvoiceId = dynamicInvoiceId;
                if (item.WaybillNumber.HasValue) 
                    createdItem.TripId = itemsList.FirstOrDefault(x=> x.WaybillNumber == item.WaybillNumber)?.TripId
                                         ?? throw new UserFriendlyException(L("TripWithWaybillNumberNotFound", item.WaybillNumber)); 
                await _dynamicInvoiceItemRepository.InsertAsync(createdItem);
            }
        }
        
        [AbpAuthorize(AppPermissions.Pages_DynamicInvoices_Update)]
        protected virtual async Task Update(CreateOrEditDynamicInvoiceDto input)
        {
            // Important Note: in case of update dynamic invoice, the waybill number is not mapped 
            // that's mean you can't change trip id of Dynamic invoice
            
            if (!input.Id.HasValue) throw new UserFriendlyException(L("IdCanNotBeEmpty"));
            
            var dynamicInvoice = await _dynamicInvoiceRepository.GetAllIncluding(x=> x.Items)
                .SingleAsync(x => x.Id == input.Id);

            var deletedItems = dynamicInvoice.Items
                .Where(x => input.Items.All(i => i.Id != x.Id));
            
            foreach (DynamicInvoiceItem deletedItem in deletedItems)
                await _dynamicInvoiceItemRepository.DeleteAsync(deletedItem);

            ObjectMapper.Map(input, dynamicInvoice);
            
            var taxVat = await SettingManager.GetSettingValueAsync<decimal>(AppSettings.HostManagement.TaxVat);
            dynamicInvoice.SubTotalAmount = dynamicInvoice.Items.Sum(x => x.Price);
            dynamicInvoice.VatAmount = dynamicInvoice.SubTotalAmount * taxVat;
            dynamicInvoice.TotalAmount = dynamicInvoice.SubTotalAmount + dynamicInvoice.VatAmount;

        }

        [AbpAuthorize(AppPermissions.Pages_DynamicInvoices_Delete)]
        public async Task Delete(long dynamicInvoiceId)
        {
            var isDynamicInvoiceExist = await _dynamicInvoiceRepository.GetAll().AnyAsync(x => x.Id == dynamicInvoiceId);

            if (!isDynamicInvoiceExist) throw new EntityNotFoundException(L("NotFound"));

            await _dynamicInvoiceRepository.DeleteAsync(dynamicInvoiceId);
        }

    }
}