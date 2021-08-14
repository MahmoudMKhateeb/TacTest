using TACHYON.Vases;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
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
using Abp.Application.Features;
using TACHYON.Features;

namespace TACHYON.Vases
{
    [AbpAuthorize(AppPermissions.Pages_VasPrices)]
    [RequiresFeature(AppFeatures.Carrier)]
    public class VasPricesAppService : TACHYONAppServiceBase, IVasPricesAppService
    {
        private readonly IRepository<VasPrice> _vasPriceRepository;
        private readonly IVasPricesExcelExporter _vasPricesExcelExporter;
        private readonly IRepository<Vas, int> _lookup_vasRepository;


        public VasPricesAppService(IRepository<VasPrice> vasPriceRepository, IVasPricesExcelExporter vasPricesExcelExporter, IRepository<Vas, int> lookup_vasRepository)
        {
            _vasPriceRepository = vasPriceRepository;
            _vasPricesExcelExporter = vasPricesExcelExporter;
            _lookup_vasRepository = lookup_vasRepository;

        }


        public async Task<PagedResultDto<GetVasPriceForViewDto>> GetAllVASs(GetAllVasPricesInput input)
        {

            var filteredVASs = _lookup_vasRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) )
                        .WhereIf(!string.IsNullOrWhiteSpace(input.VasNameFilter), e => false || e.Name.Contains(input.VasNameFilter) );

            var vasPrices = from o in filteredVASs
                            join o1 in _vasPriceRepository.GetAll()
                            on o.Id equals o1.VasId into j1
                            from s1 in j1.DefaultIfEmpty()


                            select new GetVasPriceForViewDto()
                            {
                                VasPrice = new VasPriceDto
                                {
                                    Price = s1.Price,
                                    MaxAmount = s1.MaxAmount,
                                    MaxCount = s1.MaxCount,
                                    Id = s1.Id,
                                    VasId = o.Id,
                                },
                                VasName = o.Translations.FirstOrDefault(t=> t.Language.Contains(CurrentLanguage)) != null ? o.Translations.FirstOrDefault(t=> t.Language.Contains(CurrentLanguage)).DisplayName : o.Name,
                                HasCount = o == null ? false : o.HasCount,
                                HasAmount = o == null ? false : o.HasAmount
                            };

            var filteredVASPrices = vasPrices
                        .WhereIf(input.MinPriceFilter != null, e => e.VasPrice.Price >= input.MinPriceFilter)
                        .WhereIf(input.MaxPriceFilter != null, e => e.VasPrice.Price <= input.MaxPriceFilter)
                        .WhereIf(input.MinAmountFilter != null, e => e.VasPrice.MaxAmount >= input.MinAmountFilter)
                        .WhereIf(input.MaxAmountFilter != null, e => e.VasPrice.MaxAmount <= input.MaxAmountFilter)
                        .WhereIf(input.MinCountFilter != null, e => e.VasPrice.MaxCount >= input.MinCountFilter)
                        .WhereIf(input.MaxCountFilter != null, e => e.VasPrice.MaxCount <= input.MaxCountFilter);
                        

            var pagedAndFilteredVASPrices = filteredVASPrices
                .OrderBy(input.Sorting ?? "vasPrice.vasId asc")
                .PageBy(input);

            var totalCount = await filteredVASPrices.CountAsync();

            return new PagedResultDto<GetVasPriceForViewDto>(
                totalCount,
                await pagedAndFilteredVASPrices.ToListAsync()
            );
        }


        public async Task<PagedResultDto<GetVasPriceForViewDto>> GetAll(GetAllVasPricesInput input)
        {

            var filteredVasPrices = _vasPriceRepository.GetAll()
                        .Include(e => e.VasFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false)
                        .WhereIf(input.MinPriceFilter != null, e => e.Price >= input.MinPriceFilter)
                        .WhereIf(input.MaxPriceFilter != null, e => e.Price <= input.MaxPriceFilter)
                        .WhereIf(input.MinAmountFilter != null, e => e.MaxAmount >= input.MinAmountFilter)
                        .WhereIf(input.MaxAmountFilter != null, e => e.MaxAmount <= input.MaxAmountFilter)
                        .WhereIf(input.MinCountFilter != null, e => e.MaxCount >= input.MinCountFilter)
                        .WhereIf(input.MaxCountFilter != null, e => e.MaxCount <= input.MaxCountFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.VasNameFilter), e => e.VasFk != null && e.VasFk.Name == input.VasNameFilter);

            var pagedAndFilteredVasPrices = filteredVasPrices
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var vasPrices = from o in pagedAndFilteredVasPrices
                            join o1 in _lookup_vasRepository.GetAll() on o.VasId equals o1.Id into j1
                            from s1 in j1.DefaultIfEmpty()

                            select new GetVasPriceForViewDto()
                            {
                                VasPrice = new VasPriceDto
                                {
                                    Price = o.Price,
                                    MaxAmount = o.MaxAmount,
                                    MaxCount = o.MaxCount,
                                    Id = o.Id,
                                    VasId = s1.Id
                                },
                                VasName = s1 == null || s1.Name == null ? "" : s1.Name.ToString(),
                                HasCount = s1 == null ? false : s1.HasCount,
                                HasAmount = s1 == null ? false : s1.HasAmount
                            };

            var totalCount = await vasPrices.CountAsync();

            return new PagedResultDto<GetVasPriceForViewDto>(
                totalCount,
                await vasPrices.ToListAsync()
            );
        }

        public async Task<GetVasPriceForViewDto> GetVasPriceForView(int id)
        {
            var vasPrice = await _vasPriceRepository.GetAsync(id);

            var output = new GetVasPriceForViewDto { VasPrice = ObjectMapper.Map<VasPriceDto>(vasPrice) };

            if (output.VasPrice.VasId != null)
            {
                var _lookupVas = await _lookup_vasRepository.FirstOrDefaultAsync((int)output.VasPrice.VasId);
                output.VasName = _lookupVas?.Name?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_VasPrices_Edit)]
        public async Task<GetVasPriceForEditOutput> GetVasPriceForEdit(EntityDto input)
        {
            var vasPrice = await _vasPriceRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetVasPriceForEditOutput { VasPrice = ObjectMapper.Map<CreateOrEditVasPriceDto>(vasPrice) };

            if (output.VasPrice.VasId != null)
            {
                var _lookupVas = await _lookup_vasRepository.FirstOrDefaultAsync((int)output.VasPrice.VasId);
                output.VasName = _lookupVas?.Name?.ToString();

            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditVasPriceDto input)
        {
            if (input.Id == null || input.Id == 0)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_VasPrices_Create)]
        protected virtual async Task Create(CreateOrEditVasPriceDto input)
        {
            var vasPrice = ObjectMapper.Map<VasPrice>(input);


            if (AbpSession.TenantId != null)
            {
                vasPrice.TenantId = (int)AbpSession.TenantId;
            }


            await _vasPriceRepository.InsertAsync(vasPrice);
        }

        [AbpAuthorize(AppPermissions.Pages_VasPrices_Edit)]
        protected virtual async Task Update(CreateOrEditVasPriceDto input)
        {
            var vasPrice = await _vasPriceRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, vasPrice);
        }

        [AbpAuthorize(AppPermissions.Pages_VasPrices_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _vasPriceRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetVasPricesToExcel(GetAllVasPricesForExcelInput input)
        {

            var filteredVasPrices = _vasPriceRepository.GetAll()
                        .Include(e => e.VasFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false)
                        .WhereIf(input.MinPriceFilter != null, e => e.Price >= input.MinPriceFilter)
                        .WhereIf(input.MaxPriceFilter != null, e => e.Price <= input.MaxPriceFilter)
                        .WhereIf(input.MinMaxAmountFilter != null, e => e.MaxAmount >= input.MinMaxAmountFilter)
                        .WhereIf(input.MaxMaxAmountFilter != null, e => e.MaxAmount <= input.MaxMaxAmountFilter)
                        .WhereIf(input.MinMaxCountFilter != null, e => e.MaxCount >= input.MinMaxCountFilter)
                        .WhereIf(input.MaxMaxCountFilter != null, e => e.MaxCount <= input.MaxMaxCountFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.VasNameFilter), e => e.VasFk != null && e.VasFk.Name == input.VasNameFilter);

            var query = (from o in filteredVasPrices
                         join o1 in _lookup_vasRepository.GetAll() on o.VasId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         select new GetVasPriceForViewDto()
                         {
                             VasPrice = new VasPriceDto
                             {
                                 Price = o.Price,
                                 MaxAmount = o.MaxAmount,
                                 MaxCount = o.MaxCount,
                                 Id = o.Id
                             },
                             VasName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
                         });


            var vasPriceListDtos = await query.ToListAsync();

            return _vasPricesExcelExporter.ExportToFile(vasPriceListDtos);
        }


        [AbpAuthorize(AppPermissions.Pages_VasPrices)]
        public async Task<List<VasPriceVasLookupTableDto>> GetAllVasForTableDropdown()
        {
            return await _lookup_vasRepository.GetAll()
                .Select(vas => new VasPriceVasLookupTableDto
                {
                    Id = vas.Id,
                    DisplayName = vas == null || vas.Name == null ? "" : vas.Name.ToString()
                }).ToListAsync();
        }

    }
}