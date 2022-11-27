using TACHYON.Shipping.ShippingRequestTrips;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Integration.BayanIntegration.Exporting;
using TACHYON.Integration.BayanIntegration.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using TACHYON.Storage;

namespace TACHYON.Integration.BayanIntegration
{
    [AbpAuthorize(AppPermissions.Pages_Administration_BayanIntegrationResults)]
    public class BayanIntegrationResultsAppService : TACHYONAppServiceBase, IBayanIntegrationResultsAppService
    {
        private readonly IRepository<BayanIntegrationResult, long> _bayanIntegrationResultRepository;
        private readonly IBayanIntegrationResultsExcelExporter _bayanIntegrationResultsExcelExporter;
        private readonly IRepository<ShippingRequestTrip, int> _lookup_shippingRequestTripRepository;

        public BayanIntegrationResultsAppService(IRepository<BayanIntegrationResult, long> bayanIntegrationResultRepository, IBayanIntegrationResultsExcelExporter bayanIntegrationResultsExcelExporter, IRepository<ShippingRequestTrip, int> lookup_shippingRequestTripRepository)
        {
            _bayanIntegrationResultRepository = bayanIntegrationResultRepository;
            _bayanIntegrationResultsExcelExporter = bayanIntegrationResultsExcelExporter;
            _lookup_shippingRequestTripRepository = lookup_shippingRequestTripRepository;

        }

        public async Task<PagedResultDto<GetBayanIntegrationResultForViewDto>> GetAll(GetAllBayanIntegrationResultsInput input)
        {

            var filteredBayanIntegrationResults = _bayanIntegrationResultRepository.GetAll()
                        .Include(e => e.ShippingRequestTripFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.ActionName.Contains(input.Filter) || e.InputJson.Contains(input.Filter) || e.ResponseJson.Contains(input.Filter) || e.Version.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ActionNameFilter), e => e.ActionName == input.ActionNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.InputJsonFilter), e => e.InputJson == input.InputJsonFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ResponseJsonFilter), e => e.ResponseJson == input.ResponseJsonFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.VersionFilter), e => e.Version == input.VersionFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ShippingRequestTripContainerNumberFilter), e => e.ShippingRequestTripFk != null && e.ShippingRequestTripFk.ContainerNumber == input.ShippingRequestTripContainerNumberFilter);

            var pagedAndFilteredBayanIntegrationResults = filteredBayanIntegrationResults
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var bayanIntegrationResults = from o in pagedAndFilteredBayanIntegrationResults
                                          join o1 in _lookup_shippingRequestTripRepository.GetAll() on o.ShippingRequestTripId equals o1.Id into j1
                                          from s1 in j1.DefaultIfEmpty()

                                          select new
                                          {

                                              o.ActionName,
                                              o.InputJson,
                                              o.ResponseJson,
                                              o.Version,
                                              Id = o.Id,
                                              ShippingRequestTripContainerNumber = s1 == null || s1.ContainerNumber == null ? "" : s1.ContainerNumber.ToString(),
                                              o.ShippingRequestTripId
                                          };

            var totalCount = await filteredBayanIntegrationResults.CountAsync();

            var dbList = await bayanIntegrationResults.ToListAsync();
            var results = new List<GetBayanIntegrationResultForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetBayanIntegrationResultForViewDto()
                {
                    BayanIntegrationResult = new BayanIntegrationResultDto
                    {

                        ActionName = o.ActionName,
                        InputJson = o.InputJson,
                        ResponseJson = o.ResponseJson,
                        Version = o.Version,
                        Id = o.Id,
                        ShippingRequestTripId = o.ShippingRequestTripId
                    },
                    ShippingRequestTripContainerNumber = o.ShippingRequestTripContainerNumber
                };

                results.Add(res);
            }

            return new PagedResultDto<GetBayanIntegrationResultForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<GetBayanIntegrationResultForViewDto> GetBayanIntegrationResultForView(long id)
        {
            var bayanIntegrationResult = await _bayanIntegrationResultRepository.GetAsync(id);

            var output = new GetBayanIntegrationResultForViewDto { BayanIntegrationResult = ObjectMapper.Map<BayanIntegrationResultDto>(bayanIntegrationResult) };

            if (output.BayanIntegrationResult.ShippingRequestTripId != null)
            {
                var _lookupShippingRequestTrip = await _lookup_shippingRequestTripRepository.FirstOrDefaultAsync((int)output.BayanIntegrationResult.ShippingRequestTripId);
                output.ShippingRequestTripContainerNumber = _lookupShippingRequestTrip?.ContainerNumber?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_BayanIntegrationResults_Edit)]
        public async Task<GetBayanIntegrationResultForEditOutput> GetBayanIntegrationResultForEdit(EntityDto<long> input)
        {
            var bayanIntegrationResult = await _bayanIntegrationResultRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetBayanIntegrationResultForEditOutput { BayanIntegrationResult = ObjectMapper.Map<CreateOrEditBayanIntegrationResultDto>(bayanIntegrationResult) };

            if (output.BayanIntegrationResult.ShippingRequestTripId != null)
            {
                var _lookupShippingRequestTrip = await _lookup_shippingRequestTripRepository.FirstOrDefaultAsync((int)output.BayanIntegrationResult.ShippingRequestTripId);
                output.ShippingRequestTripContainerNumber = _lookupShippingRequestTrip?.ContainerNumber?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditBayanIntegrationResultDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Administration_BayanIntegrationResults_Create)]
        protected virtual async Task Create(CreateOrEditBayanIntegrationResultDto input)
        {
            var bayanIntegrationResult = ObjectMapper.Map<BayanIntegrationResult>(input);

            await _bayanIntegrationResultRepository.InsertAsync(bayanIntegrationResult);

        }

        [AbpAuthorize(AppPermissions.Pages_Administration_BayanIntegrationResults_Edit)]
        protected virtual async Task Update(CreateOrEditBayanIntegrationResultDto input)
        {
            var bayanIntegrationResult = await _bayanIntegrationResultRepository.FirstOrDefaultAsync((long)input.Id);
            ObjectMapper.Map(input, bayanIntegrationResult);

        }

        [AbpAuthorize(AppPermissions.Pages_Administration_BayanIntegrationResults_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _bayanIntegrationResultRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetBayanIntegrationResultsToExcel(GetAllBayanIntegrationResultsForExcelInput input)
        {

            var filteredBayanIntegrationResults = _bayanIntegrationResultRepository.GetAll()
                        .Include(e => e.ShippingRequestTripFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.ActionName.Contains(input.Filter) || e.InputJson.Contains(input.Filter) || e.ResponseJson.Contains(input.Filter) || e.Version.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ActionNameFilter), e => e.ActionName == input.ActionNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.InputJsonFilter), e => e.InputJson == input.InputJsonFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ResponseJsonFilter), e => e.ResponseJson == input.ResponseJsonFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.VersionFilter), e => e.Version == input.VersionFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ShippingRequestTripContainerNumberFilter), e => e.ShippingRequestTripFk != null && e.ShippingRequestTripFk.ContainerNumber == input.ShippingRequestTripContainerNumberFilter);

            var query = (from o in filteredBayanIntegrationResults
                         join o1 in _lookup_shippingRequestTripRepository.GetAll() on o.ShippingRequestTripId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         select new GetBayanIntegrationResultForViewDto()
                         {
                             BayanIntegrationResult = new BayanIntegrationResultDto
                             {
                                 ActionName = o.ActionName,
                                 InputJson = o.InputJson,
                                 ResponseJson = o.ResponseJson,
                                 Version = o.Version,
                                 Id = o.Id
                             },
                             ShippingRequestTripContainerNumber = s1 == null || s1.ContainerNumber == null ? "" : s1.ContainerNumber.ToString()
                         });

            var bayanIntegrationResultListDtos = await query.ToListAsync();

            return _bayanIntegrationResultsExcelExporter.ExportToFile(bayanIntegrationResultListDtos);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_BayanIntegrationResults)]
        public async Task<PagedResultDto<BayanIntegrationResultShippingRequestTripLookupTableDto>> GetAllShippingRequestTripForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_shippingRequestTripRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.ContainerNumber != null && e.ContainerNumber.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var shippingRequestTripList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<BayanIntegrationResultShippingRequestTripLookupTableDto>();
            foreach (var shippingRequestTrip in shippingRequestTripList)
            {
                lookupTableDtoList.Add(new BayanIntegrationResultShippingRequestTripLookupTableDto
                {
                    Id = shippingRequestTrip.Id,
                    DisplayName = shippingRequestTrip.ContainerNumber?.ToString()
                });
            }

            return new PagedResultDto<BayanIntegrationResultShippingRequestTripLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

    }
}