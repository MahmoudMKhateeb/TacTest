using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Trucks.TruckStatusesTranslations.Dtos;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Authorization;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Trucks.TruckStatusesTranslations
{
    [AbpAuthorize(AppPermissions.Pages_TruckStatusesTranslations)]
    public class TruckStatusesTranslationsAppService : TACHYONAppServiceBase, ITruckStatusesTranslationsAppService
    {
        private readonly IRepository<TruckStatusesTranslation> _truckStatusesTranslationRepository;
        private readonly IRepository<TruckStatus, long> _lookup_truckStatusRepository;

        public TruckStatusesTranslationsAppService(
            IRepository<TruckStatusesTranslation> truckStatusesTranslationRepository,
            IRepository<TruckStatus, long> lookup_truckStatusRepository)
        {
            _truckStatusesTranslationRepository = truckStatusesTranslationRepository;
            _lookup_truckStatusRepository = lookup_truckStatusRepository;
        }

        public async Task<LoadResult> GetAll(GetAllTruckStatusesTranslationsInput input)
        {
            var truckStatusTranslations = _truckStatusesTranslationRepository.GetAll()
                .Where(x => x.CoreId == input.CoreId)
                .AsNoTracking().ProjectTo<GetTruckStatusesTranslationForViewDto>
                    (AutoMapperConfigurationProvider);

            return await LoadResultAsync(truckStatusTranslations, input.LoadOptions);
        }

        public async Task<GetTruckStatusesTranslationForViewDto> GetTruckStatusesTranslationForView(int id)
        {
            var truckStatusesTranslation = await _truckStatusesTranslationRepository.GetAsync(id);

            var output = new GetTruckStatusesTranslationForViewDto
            {
                TruckStatusesTranslation = ObjectMapper.Map<TruckStatusesTranslationDto>(truckStatusesTranslation)
            };

            if (output.TruckStatusesTranslation.CoreId != null)
            {
                var _lookupTruckStatus =
                    await _lookup_truckStatusRepository.FirstOrDefaultAsync(
                        (long)output.TruckStatusesTranslation.CoreId);
                output.TruckStatusDisplayName = _lookupTruckStatus?.DisplayName?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_TruckStatusesTranslations_Edit)]
        public async Task<GetTruckStatusesTranslationForEditOutput> GetTruckStatusesTranslationForEdit(EntityDto input)
        {
            var truckStatusesTranslation = await _truckStatusesTranslationRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTruckStatusesTranslationForEditOutput
            {
                TruckStatusesTranslation =
                    ObjectMapper.Map<CreateOrEditTruckStatusesTranslationDto>(truckStatusesTranslation)
            };

            if (output.TruckStatusesTranslation.CoreId != null)
            {
                var _lookupTruckStatus =
                    await _lookup_truckStatusRepository.FirstOrDefaultAsync(
                        (long)output.TruckStatusesTranslation.CoreId);
                output.TruckStatusDisplayName = _lookupTruckStatus?.DisplayName?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTruckStatusesTranslationDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_TruckStatusesTranslations_Create)]
        protected virtual async Task Create(CreateOrEditTruckStatusesTranslationDto input)
        {
            var truckStatusesTranslation = ObjectMapper.Map<TruckStatusesTranslation>(input);

            await _truckStatusesTranslationRepository.InsertAsync(truckStatusesTranslation);
        }

        [AbpAuthorize(AppPermissions.Pages_TruckStatusesTranslations_Edit)]
        protected virtual async Task Update(CreateOrEditTruckStatusesTranslationDto input)
        {
            var truckStatusesTranslation = await _truckStatusesTranslationRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, truckStatusesTranslation);
        }

        [AbpAuthorize(AppPermissions.Pages_TruckStatusesTranslations_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _truckStatusesTranslationRepository.DeleteAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_TruckStatusesTranslations)]
        public async Task<List<TruckStatusesTranslationTruckStatusLookupTableDto>> GetAllTruckStatusForTableDropdown()
        {
            return await _lookup_truckStatusRepository.GetAll()
                .Select(truckStatus => new TruckStatusesTranslationTruckStatusLookupTableDto
                {
                    Id = truckStatus.Id,
                    DisplayName = truckStatus == null || truckStatus.DisplayName == null
                        ? ""
                        : truckStatus.DisplayName.ToString()
                }).ToListAsync();
        }
    }
}