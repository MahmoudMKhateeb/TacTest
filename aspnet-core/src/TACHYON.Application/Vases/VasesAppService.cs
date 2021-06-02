using System;
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
        private readonly IVasesExcelExporter _vasesExcelExporter;

        public VasesAppService(IRepository<Vas> vasRepository, IVasesExcelExporter vasesExcelExporter)
        {
            _vasRepository = vasRepository;
            _vasesExcelExporter = vasesExcelExporter;

        }

        public async Task<PagedResultDto<GetVasForViewDto>> GetAll(GetAllVasesInput input)
        {

            var filteredVases = _vasRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.DisplayName.Contains(input.Filter))
                        .WhereIf(input.HasAmountFilter.HasValue && input.HasAmountFilter > -1, e => (input.HasAmountFilter == 1 && e.HasAmount) || (input.HasAmountFilter == 0 && !e.HasAmount))
                        .WhereIf(input.HasCountFilter.HasValue && input.HasCountFilter > -1, e => (input.HasCountFilter == 1 && e.HasCount) || (input.HasCountFilter == 0 && !e.HasCount));

            var pagedAndFilteredVases = filteredVases
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var vases = from o in pagedAndFilteredVases
                        select new GetVasForViewDto()
                        {
                            Vas = new VasDto
                            {
                                Name = o.Name,
                                HasAmount = o.HasAmount,
                                HasCount = o.HasCount,
                                Id = o.Id,
                                CreationTime= o.CreationTime
                            }
                        };

            var totalCount = await filteredVases.CountAsync();

            return new PagedResultDto<GetVasForViewDto>(
                totalCount,
                await vases.ToListAsync()
            );
        }

        public async Task<GetVasForViewDto> GetVasForView(int id)
        {
            var vas = await _vasRepository.GetAsync(id);

            var output = new GetVasForViewDto { Vas = ObjectMapper.Map<VasDto>(vas) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Vases_Edit)]
        public async Task<GetVasForEditOutput> GetVasForEdit(EntityDto input)
        {
            var vas = await _vasRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetVasForEditOutput { Vas = ObjectMapper.Map<CreateOrEditVasDto>(vas) };

            return output;
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
            var vas = ObjectMapper.Map<Vas>(input);

            await _vasRepository.InsertAsync(vas);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Vases_Edit)]
        protected virtual async Task Update(CreateOrEditVasDto input)
        {
            var vas = await _vasRepository.FirstOrDefaultAsync((int)input.Id);
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
            var item =await _vasRepository.FirstOrDefaultAsync(x => x.Name.ToLower() == input.Name.ToLower() && x.Id!=input.Id);
            if (item != null)
            {
                throw new UserFriendlyException(L("CannotInsertDuplicatedVasNameMessage"));
            }
        }
    }
}