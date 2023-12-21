using TACHYON.Redemption;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Redemption.Exporting;
using TACHYON.Redemption.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using TACHYON.Storage;

namespace TACHYON.Redemption
{
    [AbpAuthorize(AppPermissions.Pages_Administration_RedemptionCodes)]
    public class RedemptionCodesAppService : TACHYONAppServiceBase, IRedemptionCodesAppService
    {
        private readonly IRepository<RedemptionCode, long> _redemptionCodeRepository;
        private readonly IRedemptionCodesExcelExporter _redemptionCodesExcelExporter;
        private readonly IRepository<RedeemCode, long> _lookup_redeemCodeRepository;

        public RedemptionCodesAppService(IRepository<RedemptionCode, long> redemptionCodeRepository, IRedemptionCodesExcelExporter redemptionCodesExcelExporter, IRepository<RedeemCode, long> lookup_redeemCodeRepository)
        {
            _redemptionCodeRepository = redemptionCodeRepository;
            _redemptionCodesExcelExporter = redemptionCodesExcelExporter;
            _lookup_redeemCodeRepository = lookup_redeemCodeRepository;

        }

        public async Task<PagedResultDto<GetRedemptionCodeForViewDto>> GetAll(GetAllRedemptionCodesInput input)
        {

            var filteredRedemptionCodes = _redemptionCodeRepository.GetAll()
                        .Include(e => e.RedeemCodeFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false)
                        .WhereIf(input.MinRedemptionDateFilter != null, e => e.RedemptionDate >= input.MinRedemptionDateFilter)
                        .WhereIf(input.MaxRedemptionDateFilter != null, e => e.RedemptionDate <= input.MaxRedemptionDateFilter)
                        .WhereIf(input.MinRedemptionTenantIdFilter != null, e => e.RedemptionTenantId >= input.MinRedemptionTenantIdFilter)
                        .WhereIf(input.MaxRedemptionTenantIdFilter != null, e => e.RedemptionTenantId <= input.MaxRedemptionTenantIdFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RedeemCodeCodeFilter), e => e.RedeemCodeFk != null && e.RedeemCodeFk.Code == input.RedeemCodeCodeFilter);

            var pagedAndFilteredRedemptionCodes = filteredRedemptionCodes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var redemptionCodes = from o in pagedAndFilteredRedemptionCodes
                                  join o1 in _lookup_redeemCodeRepository.GetAll() on o.RedeemCodeId equals o1.Id into j1
                                  from s1 in j1.DefaultIfEmpty()

                                  select new
                                  {

                                      o.RedemptionDate,
                                      o.RedemptionTenantId,
                                      Id = o.Id,
                                      RedeemCodeCode = s1 == null || s1.Code == null ? "" : s1.Code.ToString()
                                  };

            var totalCount = await filteredRedemptionCodes.CountAsync();

            var dbList = await redemptionCodes.ToListAsync();
            var results = new List<GetRedemptionCodeForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetRedemptionCodeForViewDto()
                {
                    RedemptionCode = new RedemptionCodeDto
                    {

                        RedemptionDate = o.RedemptionDate,
                        RedemptionTenantId = o.RedemptionTenantId,
                        Id = o.Id,
                    },
                    RedeemCodeCode = o.RedeemCodeCode
                };

                results.Add(res);
            }

            return new PagedResultDto<GetRedemptionCodeForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<GetRedemptionCodeForViewDto> GetRedemptionCodeForView(long id)
        {
            var redemptionCode = await _redemptionCodeRepository.GetAsync(id);

            var output = new GetRedemptionCodeForViewDto { RedemptionCode = ObjectMapper.Map<RedemptionCodeDto>(redemptionCode) };

            if (output.RedemptionCode.RedeemCodeId != null)
            {
                var _lookupRedeemCode = await _lookup_redeemCodeRepository.FirstOrDefaultAsync((long)output.RedemptionCode.RedeemCodeId);
                output.RedeemCodeCode = _lookupRedeemCode?.Code?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_RedemptionCodes_Edit)]
        public async Task<GetRedemptionCodeForEditOutput> GetRedemptionCodeForEdit(EntityDto<long> input)
        {
            var redemptionCode = await _redemptionCodeRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetRedemptionCodeForEditOutput { RedemptionCode = ObjectMapper.Map<CreateOrEditRedemptionCodeDto>(redemptionCode) };

            if (output.RedemptionCode.RedeemCodeId != null)
            {
                var _lookupRedeemCode = await _lookup_redeemCodeRepository.FirstOrDefaultAsync((long)output.RedemptionCode.RedeemCodeId);
                output.RedeemCodeCode = _lookupRedeemCode?.Code?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditRedemptionCodeDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Administration_RedemptionCodes_Create)]
        protected virtual async Task Create(CreateOrEditRedemptionCodeDto input)
        {
            var redemptionCode = ObjectMapper.Map<RedemptionCode>(input);

            await _redemptionCodeRepository.InsertAsync(redemptionCode);

        }

        [AbpAuthorize(AppPermissions.Pages_Administration_RedemptionCodes_Edit)]
        protected virtual async Task Update(CreateOrEditRedemptionCodeDto input)
        {
            var redemptionCode = await _redemptionCodeRepository.FirstOrDefaultAsync((long)input.Id);
            ObjectMapper.Map(input, redemptionCode);

        }

        [AbpAuthorize(AppPermissions.Pages_Administration_RedemptionCodes_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _redemptionCodeRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetRedemptionCodesToExcel(GetAllRedemptionCodesForExcelInput input)
        {

            var filteredRedemptionCodes = _redemptionCodeRepository.GetAll()
                        .Include(e => e.RedeemCodeFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false)
                        .WhereIf(input.MinRedemptionDateFilter != null, e => e.RedemptionDate >= input.MinRedemptionDateFilter)
                        .WhereIf(input.MaxRedemptionDateFilter != null, e => e.RedemptionDate <= input.MaxRedemptionDateFilter)
                        .WhereIf(input.MinRedemptionTenantIdFilter != null, e => e.RedemptionTenantId >= input.MinRedemptionTenantIdFilter)
                        .WhereIf(input.MaxRedemptionTenantIdFilter != null, e => e.RedemptionTenantId <= input.MaxRedemptionTenantIdFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RedeemCodeCodeFilter), e => e.RedeemCodeFk != null && e.RedeemCodeFk.Code == input.RedeemCodeCodeFilter);

            var query = (from o in filteredRedemptionCodes
                         join o1 in _lookup_redeemCodeRepository.GetAll() on o.RedeemCodeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         select new GetRedemptionCodeForViewDto()
                         {
                             RedemptionCode = new RedemptionCodeDto
                             {
                                 RedemptionDate = o.RedemptionDate,
                                 RedemptionTenantId = o.RedemptionTenantId,
                                 Id = o.Id
                             },
                             RedeemCodeCode = s1 == null || s1.Code == null ? "" : s1.Code.ToString()
                         });

            var redemptionCodeListDtos = await query.ToListAsync();

            return _redemptionCodesExcelExporter.ExportToFile(redemptionCodeListDtos);
        }

    }
}