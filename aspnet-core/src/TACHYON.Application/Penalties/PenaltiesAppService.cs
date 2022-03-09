using Abp.Authorization;
using Abp.Domain.Repositories;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Common;
using TACHYON.Features;
using TACHYON.Penalties.Dto;

namespace TACHYON.Penalties
{
    [AbpAuthorize()]
   public class PenaltiesAppService : TACHYONAppServiceBase , IPenaltiesAppService
    {
        private readonly IRepository<Penalty> _penaltyRepository;
        public PenaltiesAppService(IRepository<Penalty> penaltyRepository)
        {
            _penaltyRepository = penaltyRepository;
        }
        public async Task<LoadResult> GetAll(LoadOptionsInput input) 
        {
            var query = _penaltyRepository
                           .GetAll()
                           .ProjectTo<GetAllPenaltiesDto>(AutoMapperConfigurationProvider)
                           .AsNoTracking();
            if (!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                DisableTenancyFilters();
            }
            return await LoadResultAsync(query, input.LoadOptions);
        }
    }
}
