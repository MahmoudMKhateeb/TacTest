

using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
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


        public TruckStatusesAppService(IRepository<TruckStatus, long> truckStatusRepository, IRepository<TruckStatusesTranslation> truckStatusTranslationRepository)
        {
            _truckStatusRepository = truckStatusRepository;
            _truckStatusTranslationRepository = truckStatusTranslationRepository;
        }

        public async Task<PagedResultDto<GetTruckStatusForViewDto>> GetAll(GetAllTruckStatusesInput input)
        {

            var truckStatuses = _truckStatusRepository.GetAll()
                .AsNoTracking()
                .Include(x=> x.Translations)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    e => false || e.DisplayName.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),
                    e => e.DisplayName == input.DisplayNameFilter)
                .OrderBy(input.Sorting ?? "id asc");

            var pageResult = await truckStatuses.PageBy(input).ToListAsync();

            var totalCount = await truckStatuses.CountAsync();

            var items = pageResult.Select(x => new GetTruckStatusForViewDto()
            {
                TruckStatus = new TruckStatusDto()
                {
                    DisplayName = x.DisplayName,
                    Id = x.Id,
                    TruckStatusesTranslation = x.Translations
                        .Select(t=> new TruckStatusesTranslationDto()
                    {
                            CoreId = t.CoreId,
                            Id = t.Id,
                            Language = t.Language,
                            TranslatedDisplayName = t.TranslatedDisplayName

                    }).ToList()
                }
            }).ToList();

            return new PagedResultDto<GetTruckStatusForViewDto>()
            {
                Items = ObjectMapper.Map<List<GetTruckStatusForViewDto>>(pageResult),
                TotalCount = totalCount
            };
        }

        public async Task<GetTruckStatusForViewDto> GetTruckStatusForView(long id)
        {
            var truckStatus = await _truckStatusRepository.GetAll().AsNoTracking()
                .Include(x=> x.Translations)
                .FirstOrDefaultAsync(x=> x.Id == id);

            var output = new GetTruckStatusForViewDto
            {
                TruckStatus = ObjectMapper.Map<TruckStatusDto>(truckStatus)
            };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TruckStatuses_Edit)]
        public async Task<GetTruckStatusForEditOutput> GetTruckStatusForEdit(EntityDto<long> input)
        {
            var truckStatus = await _truckStatusRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTruckStatusForEditOutput { TruckStatus = ObjectMapper.Map<CreateOrEditTruckStatusDto>(truckStatus) };

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
            // TODO Ignore Map Translations List

            var truckStatus = ObjectMapper.Map<TruckStatus>(input);

            using (var unitOfWork = UnitOfWorkManager.Begin())
            {
               var coreId=  await _truckStatusRepository.InsertAndGetIdAsync(truckStatus);

                var truckStatusTranslations =
                    ObjectMapper.Map<List<TruckStatusesTranslation>>(input.TruckStatusTranslation);

                foreach (var tst in truckStatusTranslations)
                {
                    tst.CoreId = coreId;
                    await _truckStatusTranslationRepository.InsertAsync(tst);
                }

                await unitOfWork.CompleteAsync();
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TruckStatuses_Edit)]
        protected virtual async Task Update(CreateOrEditTruckStatusDto input)
        {
            var truckStatus = await _truckStatusRepository.FirstOrDefaultAsync(input.Id.Value);

            truckStatus.Translations.Clear();

            ObjectMapper.Map(input, truckStatus);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TruckStatuses_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _truckStatusRepository.DeleteAsync(input.Id);
        }
    }
}