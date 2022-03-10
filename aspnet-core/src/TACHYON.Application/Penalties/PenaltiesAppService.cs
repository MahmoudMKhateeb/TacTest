using Abp.Authorization;
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
using TACHYON.Common;
using TACHYON.Features;
using TACHYON.MultiTenancy;
using TACHYON.Penalties.Dto;

namespace TACHYON.Penalties
{
   public class PenaltiesAppService : TACHYONAppServiceBase , IPenaltiesAppService
    {
        private readonly IRepository<Penalty> _penaltyRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        public PenaltiesAppService(IRepository<Penalty> penaltyRepository, IRepository<Tenant> tenantRepository)
        {
            _penaltyRepository = penaltyRepository;
            _tenantRepository = tenantRepository;
        }

        #region MainFunctions
        public async Task<LoadResult> GetAll(LoadOptionsInput input)
        {
            var query = _penaltyRepository
                           .GetAll()
                           .Include(x => x.Tenant)
                           .ProjectTo<GetAllPenaltiesDto>(AutoMapperConfigurationProvider)
                           .AsNoTracking();
            if (!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                DisableTenancyFilters();
            }
            return await LoadResultAsync(query, input.LoadOptions);
        }
        public async Task CreateOrEdit(CreateOrEditPenaltyDto input) 
        {
            if (!input.Id.HasValue)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }
        public async Task<CreateOrEditPenaltyDto> GetPenaltyForEditDto(long Id) 
        {
            var penalty = await _penaltyRepository.FirstOrDefaultAsync(x=>x.Id == Id);

            if (penalty == null)
                throw new UserFriendlyException("ThePenaltyDoseNotFounded");

            return ObjectMapper.Map<CreateOrEditPenaltyDto>(penalty);
        }
        #endregion

        #region Lookups
        public async Task<List<GetAllCompanyForDropDownDto>> GetAllCompanyForDropDown()
        {
            return await _tenantRepository.GetAll()
                .Where(x=>x.EditionId == ShipperEditionId || x.EditionId == CarrierEditionId)
                .Select(x => new GetAllCompanyForDropDownDto { Id = x.Id, DisplayName = x.TenancyName }).ToListAsync();
        }
        #endregion

        #region Helper
        private async Task Create(CreateOrEditPenaltyDto model) 
        {
            var peanlty = ObjectMapper.Map<Penalty>(model);
            await _penaltyRepository.InsertAsync(peanlty);
        }
        private async Task Update(CreateOrEditPenaltyDto model) 
        {
            var pnealty = await _penaltyRepository.FirstOrDefaultAsync(x => x.Id == model.Id.Value);
            ObjectMapper.Map(model, pnealty);
        }
        #endregion

    }
}
