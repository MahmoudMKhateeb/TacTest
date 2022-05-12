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
using TACHYON.Extension;
using TACHYON.Packing.PackingTypes;
using TACHYON.Vases.Dtos;
using TACHYON.Vases.Exporting;

namespace TACHYON.Vases
{
    [AbpAuthorize(AppPermissions.Pages_Administration_Vases)]
    public class VasesAppService : TACHYONAppServiceBase, IVasesAppService
    {
        private readonly IRepository<Vas> _vasRepository;
        private readonly IRepository<VasTranslation> _vasTranslationRepository;
        private readonly IVasesExcelExporter _vasesExcelExporter;

        public VasesAppService(IRepository<Vas> vasRepository,
            IVasesExcelExporter vasesExcelExporter,
            IRepository<VasTranslation> vasTranslationRepository)
        {
            _vasRepository = vasRepository;
            _vasesExcelExporter = vasesExcelExporter;
            _vasTranslationRepository = vasTranslationRepository;
        }

        public async Task<LoadResult> GetAll(GetAllVasesInput input)
        {
            var query = _vasRepository.GetAll()
                .ProjectTo<VasDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(query, input.LoadOptions);
        }

        public async Task<GetVasForViewDto> GetVasForView(int id)
        {
            var vas = await _vasRepository.GetAll().AsNoTracking()
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == id);

            return ObjectMapper.Map<GetVasForViewDto>(vas);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Vases_Edit)]
        public async Task<GetVasForEditOutput> GetVasForEdit(EntityDto input)
        {
            var vas = await _vasRepository.GetAll()
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            return ObjectMapper.Map<GetVasForEditOutput>(vas);
        }

        public async Task CreateOrEdit(CreateOrEditVasDto input)
        {
            // await CheckIfEmptyOrDuplicatedVasName(input);

            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }


        [AbpAuthorize(AppPermissions.Pages_Administration_Vases_Create)]
        protected virtual async Task Create(CreateOrEditVasDto input)
        {
            // TO DO Ignore VasTranslation List Mapping ----> Done

            var vas = ObjectMapper.Map<Vas>(input);
            await _vasRepository.InsertAndGetIdAsync(vas);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Vases_Edit)]
        protected virtual async Task Update(CreateOrEditVasDto input)
        {
            var vas = await _vasRepository.FirstOrDefaultAsync((input.Id.Value));

            if (vas.Key.ToLower().Contains(TACHYONConsts.OthersDisplayName) &&
                !input.Key.ToLower().Contains(TACHYONConsts.OthersDisplayName))
                throw new UserFriendlyException(L("OtherVasNameMustContainOther"));

            ObjectMapper.Map(input, vas);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Vases_Delete)]
        public async Task Delete(EntityDto input)
        {
            var vas = await _vasRepository.SingleAsync(x => x.Id == input.Id);

            if (vas.Key.ToLower().Contains(TACHYONConsts.OthersDisplayName))
                throw new UserFriendlyException(L("OtherVasNotRemovable"));

            await _vasRepository.DeleteAsync(vas);
        }

        public async Task<FileDto> GetVasesToExcel(GetAllVasesForExcelInput input)
        {
            var filteredVases = _vasRepository.GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Key.Contains(input.Filter))
                .WhereIf(input.HasAmountFilter.HasValue && input.HasAmountFilter > -1,
                    e => (input.HasAmountFilter == 1 && e.HasAmount) || (input.HasAmountFilter == 0 && !e.HasAmount))
                .WhereIf(input.HasCountFilter.HasValue && input.HasCountFilter > -1,
                    e => (input.HasCountFilter == 1 && e.HasCount) || (input.HasCountFilter == 0 && !e.HasCount));

            var query = (from o in filteredVases
                select new GetVasForViewDto()
                {
                    Vas = new VasDto { Key = o.Key, HasAmount = o.HasAmount, HasCount = o.HasCount, Id = o.Id }
                });

            var vasListDtos = await query.ToListAsync();

            return _vasesExcelExporter.ExportToFile(vasListDtos);
        }


        private async Task CheckIfEmptyOrDuplicatedVasName(CreateOrEditVasDto input)
        {
            //if (input.TranslationDtos.Any(x => x.Name.IsNullOrEmpty()))
            //{
            //    throw new UserFriendlyException(L("VasNameCannotBeEmpty"));
            //}

            //var anyItemNotValid = await _vasTranslationRepository
            //    .GetAll()
            //    .Where(x => input.TranslationDtos.Select(i => i.Name).Contains(x.Name))
            //    .FirstOrDefaultAsync();


            //if (anyItemNotValid != null)
            //{
            //    throw new UserFriendlyException(L("CannotInsertDuplicatedVasNameMessage"));
            //}
        }


        #region MultiLingual

        public async Task CreateOrEditTranslation(VasTranslationDto input)
        {
            var translation = await _vasTranslationRepository.FirstOrDefaultAsync(x => x.Id == input.Id);
            if (translation == null)
            {
                var newTranslation = ObjectMapper.Map<VasTranslation>(input);
                await _vasTranslationRepository.InsertAsync(newTranslation);
            }
            else
            {
                var duplication = await _vasTranslationRepository.FirstOrDefaultAsync(x =>
                    x.CoreId == translation.CoreId && x.Language.Contains(translation.Language) &&
                    x.Id != translation.Id);
                if (duplication != null)
                {
                    throw new UserFriendlyException(
                        "The translation for this language already exists, you can modify it");
                }

                ObjectMapper.Map(input, translation);
            }
        }

        public async Task<LoadResult> GetAllTranslations(GetAllTranslationsInput input)
        {
            var filteredPackingTypes = _vasTranslationRepository
                .GetAll()
                .Where(x => x.CoreId == Convert.ToInt32(input.CoreId))
                .ProjectTo<VasTranslationDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(filteredPackingTypes, input.LoadOptions);
        }


        public async Task DeleteTranslation(EntityDto input)
        {
            await _vasTranslationRepository.DeleteAsync(input.Id);
        }

        #endregion
    }
}