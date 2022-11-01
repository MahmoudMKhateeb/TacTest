using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Common;
using TACHYON.PricePackages.Dto.TmsPricePackages;
using TACHYON.PricePackages.TmsPricePackages;

namespace TACHYON.PricePackages
{
    [AbpAuthorize(AppPermissions.Pages_TmsPricePackages)]
    public class TmsPricePackageAppService : TACHYONAppServiceBase, ITmsPricePackageAppService
    {
        private readonly IRepository<TmsPricePackage> _tmsPricePackageRepository;
        private readonly ITmsPricePackageManager _tmsPricePackageManager;

        public TmsPricePackageAppService(
            IRepository<TmsPricePackage> tmsPricePackageRepository,
            ITmsPricePackageManager tmsPricePackageManager)
        {
            _tmsPricePackageRepository = tmsPricePackageRepository;
            _tmsPricePackageManager = tmsPricePackageManager;
        }


        public async Task<LoadResult> GetAll(LoadOptionsInput input)
        {
            var pricePackages = _tmsPricePackageRepository.GetAll().AsNoTracking()
                .ProjectTo<TmsPricePackageListDto>(AutoMapperConfigurationProvider);
            
            return await LoadResultAsync(pricePackages,input.LoadOptions);
        }

        public async Task<TmsPricePackageForViewDto> GetForView(int pricePackageId)
        {
            var pricePackage = await _tmsPricePackageRepository.GetAll().AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == pricePackageId);

            if (pricePackage == null) throw new EntityNotFoundException(L("NotFound"));
            
            return ObjectMapper.Map<TmsPricePackageForViewDto>(pricePackage);
        }

        
        public async Task CreateOrEdit(CreateOrEditTmsPricePackageDto input)
        {
           // TODO: Add validation/pre-execution here if required

           if (input.Id.HasValue)
           {
               await Update(input);
               return;
           }

           await Create(input);
        }

        [AbpAuthorize(AppPermissions.Pages_TmsPricePackages_Create)]
        protected virtual async Task Create(CreateOrEditTmsPricePackageDto input)
        {
            var createdTmsPricePackage = ObjectMapper.Map<TmsPricePackage>(input);
            
            createdTmsPricePackage.CommissionType = PricePackageCommissionType.Value;
            await _tmsPricePackageRepository.InsertAsync(createdTmsPricePackage);
        }

        [AbpAuthorize(AppPermissions.Pages_TmsPricePackages_Update)]
        protected virtual async Task Update(CreateOrEditTmsPricePackageDto input)
        {
            if (!input.Id.HasValue) return;
            
            var updatedTmsPricePackage = await _tmsPricePackageRepository.FirstOrDefaultAsync(input.Id.Value);

            ObjectMapper.Map(input, updatedTmsPricePackage);
        }
        
        [AbpAuthorize(AppPermissions.Pages_TmsPricePackages_Update)]
        public async Task<CreateOrEditTmsPricePackageDto> GetForEdit(int pricePackageId)
        {
            var tmsPricePackage = await _tmsPricePackageRepository.GetAll().AsNoTracking()
                .FirstOrDefaultAsync(x=> x.Id == pricePackageId);
            
            if (tmsPricePackage == null) throw new EntityNotFoundException(L("NotFound"));
            
            return ObjectMapper.Map<CreateOrEditTmsPricePackageDto>(tmsPricePackage);
        }

        [AbpAuthorize(AppPermissions.Pages_TmsPricePackages_Delete)]
        public async Task Delete(int pricePackageId)
        {
            var isExist = await _tmsPricePackageRepository.GetAll().AnyAsync(x => x.Id == pricePackageId);

            if (!isExist) throw new EntityNotFoundException(L("NotFound"));

            await _tmsPricePackageRepository.DeleteAsync(x => x.Id == pricePackageId);
        }

        public async Task<TmsPricePackageForViewDto> GetMatchingPricePackage(long shippingRequestId)
        {
            var tmsPricePackage = await _tmsPricePackageManager.GetMatchingPricePackage(shippingRequestId);
            return ObjectMapper.Map<TmsPricePackageForViewDto>(tmsPricePackage);
        }

        public async Task ChangeActivateStatus(int pricePackageId,bool isActive)
        {
            var isExist = await _tmsPricePackageRepository.GetAll().AnyAsync(x => x.Id == pricePackageId);
            if (!isExist) throw new UserFriendlyException(L("NotFound"));
            
            _tmsPricePackageRepository.Update(pricePackageId, x => x.IsActive = isActive);
        }
        
        public async Task<LoadResult> GetAllForDropdown(GetTmsPricePackagesInput input)
        {
            var tmsPricePackages = _tmsPricePackageRepository.GetAll()
                .AsNoTracking().Where(x=> x.IsActive && x.ShipperId == input.ShipperId)
                .WhereIf(input.ProposalId.HasValue,x=> !x.ProposalId.HasValue || x.ProposalId == input.ProposalId)
                .ProjectTo<TmsPricePackageSelectItemDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(tmsPricePackages, input.LoadOptions);
        }
    }
}