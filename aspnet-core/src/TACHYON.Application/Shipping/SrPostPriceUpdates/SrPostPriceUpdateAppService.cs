using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Features;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Vases.Dtos;

namespace TACHYON.Shipping.SrPostPriceUpdates
{
    [AbpAuthorize(AppPermissions.Pages_SrPostPriceUpdate)]
    public class SrPostPriceUpdateAppService : TACHYONAppServiceBase
    {
        private readonly SrPostPriceUpdateManager _updateManager;
        private readonly IRepository<SrPostPriceUpdate,long> _updateRepository;

        public SrPostPriceUpdateAppService(
            SrPostPriceUpdateManager updateManager,
            IRepository<SrPostPriceUpdate, long> updateRepository)
        {
            _updateManager = updateManager;
            _updateRepository = updateRepository;
        }


        public async Task<PagedResultDto<SrPostPriceUpdateListDto>> GetAll(GetSrPostPriceUpdatesInput input)
        {
            var postPriceUpdates =  _updateRepository.GetAll().AsNoTracking()
                .Where(x => x.ShippingRequestId == input.ShippingRequestId)
                .OrderBy(input.Sorting ?? "Id DESC")
                // Note: ProjectTo used here to improve performance
                .ProjectTo<SrPostPriceUpdateListDto>(AutoMapperConfigurationProvider);
            
            var pageResult = await postPriceUpdates.PageBy(input).ToListAsync();

            var totalCount = await postPriceUpdates.CountAsync();

            return new PagedResultDto<SrPostPriceUpdateListDto>()
            {
                Items = pageResult, TotalCount = totalCount
            };
        }


        public async Task<ViewSrPostPriceUpdateDto> GetForView(long srUpdateId)
        {
            DisableTenancyFilters();
            var srPostPriceUpdate = await _updateRepository.GetAll().AsNoTracking()
                .Include(x => x.ShippingRequest).ThenInclude(x=> x.ShippingRequestVases)
                .ThenInclude(x=> x.VasFk).ThenInclude(x=> x.Translations)
                .WhereIf(await IsEnabledAsync(AppFeatures.Carrier),x => AbpSession.TenantId == x.ShippingRequest.CarrierTenantId)
                .WhereIf(await IsEnabledAsync(AppFeatures.Shipper),x => AbpSession.TenantId == x.ShippingRequest.TenantId)
                .SingleAsync(x => x.Id == srUpdateId);

            var updatedSrDto = JsonConvert.DeserializeObject<CreateOrEditShippingRequestDto>(srPostPriceUpdate.UpdateChanges);
            var shippingRequest = srPostPriceUpdate.ShippingRequest;
            var oldSrDto = ObjectMapper.Map<CreateOrEditShippingRequestDto>(shippingRequest);
            oldSrDto.ShippingRequestVasList = new List<CreateOrEditShippingRequestVasListDto>();
            foreach (var vas in shippingRequest.ShippingRequestVases)
            {
                var vasDto = ObjectMapper.Map<CreateOrEditShippingRequestVasListDto>(vas);

                vasDto.VasName = vas.VasFk.Translations.FirstOrDefault(x=>
                x.Language.Equals(CultureInfo.CurrentUICulture.Name))?.DisplayName ?? vas.VasFk.Name ?? vas.VasFk.Key;

                oldSrDto.ShippingRequestVasList.Add(vasDto);
            }

            var updatedShippingRequest = updatedSrDto.GetType().GetProperties()
                .ToDictionary(propertyInfo => propertyInfo.Name, propertyInfo => propertyInfo.GetValue(updatedSrDto));

            var oldShippingRequest = oldSrDto.GetType().GetProperties()
                .ToDictionary(propertyInfo => propertyInfo.Name, propertyInfo => propertyInfo.GetValue(oldSrDto));
            
            var changesResult = updatedShippingRequest.Where(x => x.Key != "ShippingRequestVasList" && (!x.Value?.Equals(oldShippingRequest[x.Key]) ?? oldShippingRequest[x.Key] != null))
                .Select(x=>
                {
                    return new SrUpdateChangeItem()
                    {
                        ChangeName = x.Key,
                        ChangeMsg = $"Changed From ({oldShippingRequest[x.Key]}) To ({x.Value})"
                    };
                }).ToList();
            
            
            var vasesList = updatedShippingRequest.FirstOrDefault(x => x.Key == "ShippingRequestVasList"
            && (!x.Value?.Equals(oldShippingRequest[x.Key]) ?? oldShippingRequest[x.Key] != null));

            if (vasesList.Value != null)
            {
                if (vasesList.Value is List<CreateOrEditShippingRequestVasListDto> list &&
                   oldShippingRequest[vasesList.Key] is List<CreateOrEditShippingRequestVasListDto> oldList)
                {
                    foreach (var oldItem in oldList)
                    {
                        var srUpdateChangeItem = new SrUpdateChangeItem();

                        if (list.All(x => x.Id != oldItem.Id))
                        {

                            srUpdateChangeItem.ChangeName = oldItem.VasName ?? oldItem.VasId.ToString();
                            srUpdateChangeItem.ChangeMsg = L("RemovedVas");

                            changesResult.Add(srUpdateChangeItem);
                            continue;
                        }

                        var item = list.FirstOrDefault(x => x.Id == oldItem.Id);

                        string message = "";

                        if (item.RequestMaxCount != oldItem.RequestMaxCount)
                        {
                            message += $"Max Count Changed From ({oldItem.RequestMaxCount}) To ({ item.RequestMaxCount})";
                        }
                          
                        if (item.RequestMaxAmount != oldItem.RequestMaxAmount)
                        {
                            if (!message.IsNullOrEmpty()) message += "\n";
                            message += $"Max Amount Changed From ({oldItem.RequestMaxAmount}) To ({ item.RequestMaxAmount})";
                        }
                          

                        if (item.NumberOfTrips != oldItem.NumberOfTrips)
                        {
                            if (!message.IsNullOrEmpty()) message += "\n";
                            message += $"Number Of Trips Changed From ({oldItem.NumberOfTrips}) To ({ item.NumberOfTrips})";
                        }

                        if (!message.IsNullOrEmpty())
                        {
                            srUpdateChangeItem.ChangeName = oldItem.VasName ?? oldItem.VasId.ToString();
                            srUpdateChangeItem.ChangeMsg = message;
                            changesResult.Add(srUpdateChangeItem);
                        }
                        

                    }
                }
            }


            var updateForViewDto = ObjectMapper.Map<ViewSrPostPriceUpdateDto>(srPostPriceUpdate);
            updateForViewDto.Changes = changesResult;
            return updateForViewDto ;
        }
        
        [AbpAuthorize(AppPermissions.Pages_SrPostPriceUpdate_CreateAction)]
        public async Task CreateUpdateAction(CreateSrPostPriceUpdateActionDto input)
        {
            DisableTenancyFilters();
            var srPostPriceUpdate = await _updateRepository.GetAll()
                .Select(x => new {x.Id, x.ShippingRequest.CarrierTenantId})
                .SingleAsync(x => x.Id == input.Id);
            
            if (srPostPriceUpdate.CarrierTenantId != AbpSession.TenantId)
                throw new AbpAuthorizationException(L("YouDoNotHavePermissionForThisAction"));
            
            await _updateManager.TakeAction(input);
        }

        [AbpAuthorize(AppPermissions.Pages_SrPostPriceUpdate_CreateOfferAction)]
        public async Task CreateOfferAction(CreateSrPostPriceUpdateOfferActionDto input)
        {
            DisableTenancyFilters();
            var srPostPriceUpdate = await _updateRepository.GetAll()
                .Select(x => new {x.Id, x.ShippingRequest.TenantId})
                .SingleAsync(x => x.Id == input.Id);

            if (srPostPriceUpdate.TenantId != AbpSession.TenantId)
                throw new AbpAuthorizationException(L("YouDoNotHavePermissionForThisAction"));
            
            
            await _updateManager.TakeOfferActionByShipper(input);
        }
        
    }
}