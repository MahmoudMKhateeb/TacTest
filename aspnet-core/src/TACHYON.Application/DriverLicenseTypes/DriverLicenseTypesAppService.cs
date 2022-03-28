

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
using TACHYON.DriverLicenseTypes.Dtos;
using TACHYON.Dto;

namespace TACHYON.DriverLicenseTypes
{
    public class DriverLicenseTypesAppService : TACHYONAppServiceBase, IDriverLicenseTypesAppService
    {
        private readonly IRepository<DriverLicenseType> _driverLicenseTypeRepository;
        private readonly IRepository<DriverLicenseTypeTranslation> _driverLicenseTypeTranslationRepository;

        public DriverLicenseTypesAppService(IRepository<DriverLicenseType> driverLicenseTypeRepository, IRepository<DriverLicenseTypeTranslation> driverLicenseTypeTranslationRepository)
        {
            _driverLicenseTypeRepository = driverLicenseTypeRepository;
            _driverLicenseTypeTranslationRepository = driverLicenseTypeTranslationRepository;
        }

        public async Task<LoadResult> GetAll(GetAllDriverLicenseTypesInput input)
        {

            var filteredDriverLicenseTypes = _driverLicenseTypeRepository.GetAll()
                        .ProjectTo<DriverLicenseTypeDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(filteredDriverLicenseTypes, input.LoadOptions);
        }

        [AbpAuthorize(AppPermissions.Pages_DriverLicenseTypes_Edit)]
        public async Task<GetDriverLicenseTypeForEditOutput> GetDriverLicenseTypeForEdit(EntityDto input)
        {
            var driverLicenseType = await _driverLicenseTypeRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetDriverLicenseTypeForEditOutput { DriverLicenseType = ObjectMapper.Map<CreateOrEditDriverLicenseTypeDto>(driverLicenseType) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditDriverLicenseTypeDto input)
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

        [AbpAuthorize(AppPermissions.Pages_DriverLicenseTypes_Create)]
        protected virtual async Task Create(CreateOrEditDriverLicenseTypeDto input)
        {
            var driverLicenseType = ObjectMapper.Map<DriverLicenseType>(input);



            await _driverLicenseTypeRepository.InsertAsync(driverLicenseType);
        }

        [AbpAuthorize(AppPermissions.Pages_DriverLicenseTypes_Edit)]
        protected virtual async Task Update(CreateOrEditDriverLicenseTypeDto input)
        {
            var driverLicenseType = await _driverLicenseTypeRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, driverLicenseType);
        }

        [AbpAuthorize(AppPermissions.Pages_DriverLicenseTypes_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _driverLicenseTypeRepository.DeleteAsync(input.Id);
        }


        public async Task<List<GetLicenseTypeForDropDownOutput>> GetForDropDownList()
        {
            DisableTenancyFilters();
            var list = await _driverLicenseTypeRepository
                .GetAll()
                .Include(x => x.Translations)
                .ToListAsync();
            return ObjectMapper.Map<List<GetLicenseTypeForDropDownOutput>>(list);
        }


        #region MultiLingual

        public async Task CreateOrEditTranslation(DriverLicenseTypeTranslationDto input)
        {

            var translation = await _driverLicenseTypeTranslationRepository.FirstOrDefaultAsync(x => x.Id == input.Id);
            if (translation == null)
            {
                var newTranslation = ObjectMapper.Map<DriverLicenseTypeTranslation>(input);
                await _driverLicenseTypeTranslationRepository.InsertAsync(newTranslation);
            }
            else
            {
                var duplication = await _driverLicenseTypeTranslationRepository.FirstOrDefaultAsync(x => x.CoreId == translation.CoreId && x.Language.Contains(translation.Language) && x.Id != translation.Id);
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
            var filteredPackingTypes = _driverLicenseTypeTranslationRepository
                .GetAll()
                .Where(x => x.CoreId == Convert.ToInt32(input.CoreId))
                .ProjectTo<DriverLicenseTypeTranslationDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(filteredPackingTypes, input.LoadOptions);
        }


        public async Task DeleteTranslation(EntityDto input)
        {
            await _driverLicenseTypeTranslationRepository.DeleteAsync(input.Id);
        }

        #endregion
    }
}