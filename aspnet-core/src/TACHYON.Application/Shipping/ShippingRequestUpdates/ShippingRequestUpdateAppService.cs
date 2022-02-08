using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Shipping.ShippingRequestUpdate;

namespace TACHYON.Shipping.ShippingRequestUpdates
{
    [AbpAuthorize()] // todo Add permission here
    public class ShippingRequestUpdateAppService : TACHYONAppServiceBase
    {
        private readonly IRepository<ShippingRequestUpdate,Guid> _srUpdateRepository;

        public ShippingRequestUpdateAppService(IRepository<ShippingRequestUpdate, Guid> srUpdateRepository)
        {
            _srUpdateRepository = srUpdateRepository;
        }


        public async Task<PagedResultDto<ShippingRequestUpdateListDto>> GetAll(GetAllSrUpdateInputDto input)
        {
            var srUpdates = _srUpdateRepository.GetAll()
                .Where(x => x.PriceOfferId == input.PriceOfferId
                            && x.ShippingRequestId == input.ShippingRequestId)
                .AsNoTracking().OrderBy(input.Sorting ?? "CreationTime asc");

            var pageResult = await srUpdates.PageBy(input).ToListAsync();

            var totalCount = await srUpdates.CountAsync();

            return new PagedResultDto<ShippingRequestUpdateListDto>()
            {
                Items = ObjectMapper.Map<List<ShippingRequestUpdateListDto>>(pageResult), TotalCount = totalCount
            };
        }
        
    }
}