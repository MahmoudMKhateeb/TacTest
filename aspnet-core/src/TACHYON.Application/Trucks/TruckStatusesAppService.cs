using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Trucks.Dtos;
using TACHYON.Trucks.TruckStatusesTranslations;
using TACHYON.Trucks.TruckStatusesTranslations.Dtos;

namespace TACHYON.Trucks
{
    [AbpAuthorize(AppPermissions.Pages_Administration_TruckStatuses)]
    public class TruckStatusesAppService : TACHYONAppServiceBase, ITruckStatusesAppService
    {
        private readonly IRepository<TruckStatus, long> _truckStatusRepository;
        private readonly IRepository<TruckStatusesTranslation> _truckStatusTranslationRepository;

        //! Don't Forget Translation Permissions

        public TruckStatusesAppService(IRepository<TruckStatus, long> truckStatusRepository,
            IRepository<TruckStatusesTranslation> truckStatusTranslationRepository)
        {
            _truckStatusRepository = truckStatusRepository;
            _truckStatusTranslationRepository = truckStatusTranslationRepository;
        }

        public async Task<LoadResult> GetAll(GetAllTruckStatusesInput input)
        {
            var truckStatuses = _truckStatusRepository.GetAll()
                .AsNoTracking().ProjectTo<TruckStatusDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(truckStatuses, input.LoadOptions);
        }

        public async Task<GetTruckStatusForViewDto> GetTruckStatusForView(long id)
        {
            var truckStatus = await _truckStatusRepository.GetAll().AsNoTracking()
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == id);

            var output = new GetTruckStatusForViewDto { TruckStatus = ObjectMapper.Map<TruckStatusDto>(truckStatus) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TruckStatuses_Edit)]
        public async Task<GetTruckStatusForEditOutput> GetTruckStatusForEdit(EntityDto<long> input)
        {
            var truckStatus = await _truckStatusRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTruckStatusForEditOutput
            {
                TruckStatus = ObjectMapper.Map<CreateOrEditTruckStatusDto>(truckStatus)
            };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTruckStatusDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Administration_TruckStatuses_Create)]
        protected virtual async Task Create(CreateOrEditTruckStatusDto input)
        {
            var truckStatus = ObjectMapper.Map<TruckStatus>(input);

            truckStatus.CreatorUserId = AbpSession.UserId;
            await _truckStatusRepository.InsertAndGetIdAsync(truckStatus);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TruckStatuses_Edit)]
        protected virtual async Task Update(CreateOrEditTruckStatusDto input)
        {
            var truckStatus = await _truckStatusRepository.FirstOrDefaultAsync(input.Id.Value);

            ObjectMapper.Map(input, truckStatus);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TruckStatuses_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            var deletedEntity = await _truckStatusRepository.FirstOrDefaultAsync(input.Id);

            if (deletedEntity == null)
                throw new UserFriendlyException(L("TruckStatusNotFound"));

            deletedEntity.DeleterUserId = AbpSession.UserId;
            deletedEntity.DeletionTime = DateTime.Now;

            await _truckStatusRepository.DeleteAsync(deletedEntity);
        }

        public async Task<LoadResult> GetAllTranslations(GetAllTruckStatusesTranslationsInput input)
        {
            var filteredTruckStatusTranslations = _truckStatusTranslationRepository
                .GetAll().AsNoTracking()
                .Where(x => x.CoreId == input.CoreId)
                .ProjectTo<TruckStatusesTranslationDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(filteredTruckStatusTranslations, input.LoadOptions);
        }

        public async Task CreateOrEditTranslation(CreateOrEditTruckStatusesTranslationDto input)
        {
            if (!input.Id.HasValue)
            {
                var d = await _truckStatusTranslationRepository
                    .GetAll()
                    .Where(x => x.CoreId == input.CoreId)
                    .Where(x => x.TranslatedDisplayName == input.TranslatedDisplayName)
                    .Where(x => x.Language.Contains(input.Language))
                    .FirstOrDefaultAsync();
                if (d != null)
                {
                    throw new UserFriendlyException(L("TranslationDuplicated"));
                }

                var createdTranslation = ObjectMapper.Map<TruckStatusesTranslation>(input);
                await _truckStatusTranslationRepository.InsertAsync(createdTranslation);
            }
            else
            {
                var updatedTranslation = await _truckStatusTranslationRepository.SingleAsync(x => x.Id == input.Id);
                ObjectMapper.Map(input, updatedTranslation);
            }
        }

        public async Task DeleteTranslation(EntityDto input)
        {
            var deletedTranslation = await _truckStatusTranslationRepository
                .FirstOrDefaultAsync(input.Id);

            if (deletedTranslation == null)
                throw new UserFriendlyException(L("TruckStatusTranslationNotFound"));

            deletedTranslation.DeletionTime = DateTime.Now;
            deletedTranslation.DeleterUserId = AbpSession.UserId;

            await _truckStatusTranslationRepository.DeleteAsync(deletedTranslation);
        }
    }
}