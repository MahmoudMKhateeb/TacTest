﻿using TACHYON.Trucks.TruckCategories.TransportTypes;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations
{
    [AbpAuthorize(AppPermissions.Pages_TransportTypesTranslations)]
    public class TransportTypesTranslationsAppService : TACHYONAppServiceBase, ITransportTypesTranslationsAppService
    {
        private readonly IRepository<TransportTypesTranslation> _transportTypesTranslationRepository;
        private readonly IRepository<TransportType, int> _lookup_transportTypeRepository;

        public TransportTypesTranslationsAppService(IRepository<TransportTypesTranslation> transportTypesTranslationRepository, IRepository<TransportType, int> lookup_transportTypeRepository)
        {
            _transportTypesTranslationRepository = transportTypesTranslationRepository;
            _lookup_transportTypeRepository = lookup_transportTypeRepository;

        }

        public async Task<PagedResultDto<GetTransportTypesTranslationForViewDto>> GetAll(GetAllTransportTypesTranslationsInput input)
        {

            var filteredTransportTypesTranslations = _transportTypesTranslationRepository.GetAll()
                        .Include(e => e.Core)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.TranslatedDisplayName.Contains(input.Filter) || e.Language.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TranslatedDisplayNameFilter), e => e.TranslatedDisplayName == input.TranslatedDisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LanguageFilter), e => e.Language == input.LanguageFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TransportTypeTranslatedDisplayNameFilter), e => e.Core != null && e.Core.DisplayName == input.TransportTypeTranslatedDisplayNameFilter);

            var pagedAndFilteredTransportTypesTranslations = filteredTransportTypesTranslations
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var transportTypesTranslations = from o in pagedAndFilteredTransportTypesTranslations
                                             join o1 in _lookup_transportTypeRepository.GetAll() on o.CoreId equals o1.Id into j1
                                             from s1 in j1.DefaultIfEmpty()

                                             select new GetTransportTypesTranslationForViewDto()
                                             {
                                                 TransportTypesTranslation = new TransportTypesTranslationDto
                                                 {
                                                     TranslatedDisplayName = o.TranslatedDisplayName,
                                                     Language = o.Language,
                                                     Id = o.Id
                                                 },
                                                 TransportTypeDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString()
                                             };

            var totalCount = await filteredTransportTypesTranslations.CountAsync();

            return new PagedResultDto<GetTransportTypesTranslationForViewDto>(
                totalCount,
                await transportTypesTranslations.ToListAsync()
            );
        }

        public async Task<GetTransportTypesTranslationForViewDto> GetTransportTypesTranslationForView(int id)
        {
            var transportTypesTranslation = await _transportTypesTranslationRepository.GetAsync(id);

            var output = new GetTransportTypesTranslationForViewDto { TransportTypesTranslation = ObjectMapper.Map<TransportTypesTranslationDto>(transportTypesTranslation) };

            if (output.TransportTypesTranslation.CoreId != null)
            {
                var _lookupTransportType = await _lookup_transportTypeRepository.FirstOrDefaultAsync((int)output.TransportTypesTranslation.CoreId);
                output.TransportTypeDisplayName = _lookupTransportType?.DisplayName?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_TransportTypesTranslations_Edit)]
        public async Task<GetTransportTypesTranslationForEditOutput> GetTransportTypesTranslationForEdit(EntityDto input)
        {
            var transportTypesTranslation = await _transportTypesTranslationRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTransportTypesTranslationForEditOutput { TransportTypesTranslation = ObjectMapper.Map<CreateOrEditTransportTypesTranslationDto>(transportTypesTranslation) };

            if (output.TransportTypesTranslation.CoreId != null)
            {
                var _lookupTransportType = await _lookup_transportTypeRepository.FirstOrDefaultAsync((int)output.TransportTypesTranslation.CoreId);
                output.TransportTypeDisplayName = _lookupTransportType?.DisplayName?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTransportTypesTranslationDto input)
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

        [AbpAuthorize(AppPermissions.Pages_TransportTypesTranslations_Create)]
        protected virtual async Task Create(CreateOrEditTransportTypesTranslationDto input)
        {
            var transportTypesTranslation = ObjectMapper.Map<TransportTypesTranslation>(input);

            await _transportTypesTranslationRepository.InsertAsync(transportTypesTranslation);
        }

        [AbpAuthorize(AppPermissions.Pages_TransportTypesTranslations_Edit)]
        protected virtual async Task Update(CreateOrEditTransportTypesTranslationDto input)
        {
            var transportTypesTranslation = await _transportTypesTranslationRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, transportTypesTranslation);
        }

        [AbpAuthorize(AppPermissions.Pages_TransportTypesTranslations_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _transportTypesTranslationRepository.DeleteAsync(input.Id);
        }
        [AbpAuthorize(AppPermissions.Pages_TransportTypesTranslations)]
        public async Task<List<TransportTypesTranslationTransportTypeLookupTableDto>> GetAllTransportTypeForTableDropdown()
        {
            return await _lookup_transportTypeRepository.GetAll()
                .Select(transportType => new TransportTypesTranslationTransportTypeLookupTableDto
                {
                    Id = transportType.Id,
                    TranslatedDisplayName = transportType == null || transportType.DisplayName == null ? "" : transportType.DisplayName.ToString()
                }).ToListAsync();
        }

    }
}