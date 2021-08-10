using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Vases.Exporting;
using TACHYON.Vases.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;

namespace TACHYON.Vases
{
    [AbpAuthorize(AppPermissions.Pages_Administration_Vases)]
    public class VasesAppService : TACHYONAppServiceBase, IVasesAppService
    {
        private readonly IRepository<Vas> _vasRepository;
        private readonly IRepository<VasTranslation> _vasTranslationRepository;
        private readonly IVasesExcelExporter _vasesExcelExporter;

        public VasesAppService(IRepository<Vas> vasRepository, IVasesExcelExporter vasesExcelExporter, IRepository<VasTranslation> vasTranslationRepository)
        {
            _vasRepository = vasRepository;
            _vasesExcelExporter = vasesExcelExporter;
            _vasTranslationRepository = vasTranslationRepository;
        }

        public async Task<PagedResultDto<GetVasForViewDto>> GetAll(GetAllVasesInput input)
        {

            var vases = _vasRepository.GetAll()
                .Include(x=> x.Translations)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    e => false || e.Name.Contains(input.Filter) || e.DisplayName.Contains(input.Filter))
                        .WhereIf(input.HasAmountFilter.HasValue && input.HasAmountFilter > -1,
                    e => (input.HasAmountFilter == 1 && e.HasAmount) || (input.HasAmountFilter == 0 && !e.HasAmount))
                        .WhereIf(input.HasCountFilter.HasValue && input.HasCountFilter > -1,
                    e => (input.HasCountFilter == 1 && e.HasCount) || (input.HasCountFilter == 0 && !e.HasCount))
                .OrderBy(input.Filter??"Id asc");

            var pageResult = await vases.PageBy(input).ToListAsync();
            var totalCount = await vases.CountAsync();

            return new PagedResultDto<GetVasForViewDto>()
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<GetVasForViewDto>>(pageResult)
            };
        }

        public async Task<GetVasForViewDto> GetVasForView(int id)
        {
            var vas = await _vasRepository.GetAll().AsNoTracking()
                .Include(x=> x.Translations)
                .FirstOrDefaultAsync(x=> x.Id == id);

            return ObjectMapper.Map<GetVasForViewDto>(vas);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Vases_Edit)]
        public async Task<GetVasForEditOutput> GetVasForEdit(EntityDto input)
        {
            var vas = await _vasRepository.GetAll()
                .Include(x=> x.Translations)
                .FirstOrDefaultAsync(x=> x.Id == input.Id);

            return ObjectMapper.Map<GetVasForEditOutput>(vas);
        }

        public async Task CreateOrEdit(CreateOrEditVasDto input)
        {
            await CheckIfEmptyOrDuplicatedVasName(input);

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

            using (var unitOfWork = UnitOfWorkManager.Begin())
            {
               var coreId =  await _vasRepository.InsertAndGetIdAsync(vas);

               var vasTranslations = ObjectMapper.Map<List<VasTranslation>>(input.TranslationDtos);
                
                foreach (var vasTranslation in vasTranslations)
                {
                    vasTranslation.CoreId = coreId;
                    await _vasTranslationRepository.InsertAsync(vasTranslation);
                }

                await unitOfWork.CompleteAsync();
            }

        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Vases_Edit)]
        protected virtual async Task Update(CreateOrEditVasDto input)
        {
            var vas = await _vasRepository.FirstOrDefaultAsync((input.Id.Value));
            
            vas.Translations.Clear();

            ObjectMapper.Map(input, vas);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Vases_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _vasRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetVasesToExcel(GetAllVasesForExcelInput input)
        {
            
            var filteredVases = _vasRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.DisplayName.Contains(input.Filter))
                        .WhereIf(input.HasAmountFilter.HasValue && input.HasAmountFilter > -1, e => (input.HasAmountFilter == 1 && e.HasAmount) || (input.HasAmountFilter == 0 && !e.HasAmount))
                        .WhereIf(input.HasCountFilter.HasValue && input.HasCountFilter > -1, e => (input.HasCountFilter == 1 && e.HasCount) || (input.HasCountFilter == 0 && !e.HasCount));

            var query = (from o in filteredVases
                         select new GetVasForViewDto()
                         {
                             Vas = new VasDto
                             {
                                 Name = o.Name,
                                 HasAmount = o.HasAmount,
                                 HasCount = o.HasCount,
                                 Id = o.Id
                             }
                         });

            var vasListDtos = await query.ToListAsync();

            return _vasesExcelExporter.ExportToFile(vasListDtos);
        }


        private async Task CheckIfEmptyOrDuplicatedVasName(CreateOrEditVasDto input)
        {

            if (input.TranslationDtos.Any(x=> x.Name.IsNullOrEmpty()))
            {
                throw new UserFriendlyException(L("VasNameCannotBeEmpty"));
            }

            var anyItemNotValid = await  _vasTranslationRepository
                .GetAll()
                .Where(x=> input.TranslationDtos.Select(i=> i.Name).Contains( x.Name))
                .FirstOrDefaultAsync();
           
            
            if (anyItemNotValid != null)
            {
                throw new UserFriendlyException(L("CannotInsertDuplicatedVasNameMessage"));
            }
        }
    }
}