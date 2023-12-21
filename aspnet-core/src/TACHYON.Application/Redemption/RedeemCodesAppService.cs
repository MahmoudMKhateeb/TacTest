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
    [AbpAuthorize(AppPermissions.Pages_Administration_RedeemCodes)]
    public class RedeemCodesAppService : TACHYONAppServiceBase, IRedeemCodesAppService
    {
        private readonly IRepository<RedeemCode, long> _redeemCodeRepository;
        private readonly IRedeemCodesExcelExporter _redeemCodesExcelExporter;

        public RedeemCodesAppService(IRepository<RedeemCode, long> redeemCodeRepository, IRedeemCodesExcelExporter redeemCodesExcelExporter)
        {
            _redeemCodeRepository = redeemCodeRepository;
            _redeemCodesExcelExporter = redeemCodesExcelExporter;

        }

        public async Task<PagedResultDto<GetRedeemCodeForViewDto>> GetAll(GetAllRedeemCodesInput input)
        {

            var filteredRedeemCodes = _redeemCodeRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Code.Contains(input.Filter) || e.Note.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CodeFilter), e => e.Code == input.CodeFilter)
                        .WhereIf(input.MinExpiryDateFilter != null, e => e.ExpiryDate >= input.MinExpiryDateFilter)
                        .WhereIf(input.MaxExpiryDateFilter != null, e => e.ExpiryDate <= input.MaxExpiryDateFilter)
                        .WhereIf(input.IsActiveFilter.HasValue && input.IsActiveFilter > -1, e => (input.IsActiveFilter == 1 && e.IsActive) || (input.IsActiveFilter == 0 && !e.IsActive))
                        .WhereIf(input.MinValueFilter != null, e => e.Value >= input.MinValueFilter)
                        .WhereIf(input.MaxValueFilter != null, e => e.Value <= input.MaxValueFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NoteFilter), e => e.Note == input.NoteFilter)
                        .WhereIf(input.MinpercentageFilter != null, e => e.percentage >= input.MinpercentageFilter)
                        .WhereIf(input.MaxpercentageFilter != null, e => e.percentage <= input.MaxpercentageFilter);

            var pagedAndFilteredRedeemCodes = filteredRedeemCodes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var redeemCodes = from o in pagedAndFilteredRedeemCodes
                              select new
                              {

                                  o.Code,
                                  o.ExpiryDate,
                                  o.IsActive,
                                  o.Value,
                                  o.Note,
                                  o.percentage,
                                  Id = o.Id
                              };

            var totalCount = await filteredRedeemCodes.CountAsync();

            var dbList = await redeemCodes.ToListAsync();
            var results = new List<GetRedeemCodeForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetRedeemCodeForViewDto()
                {
                    RedeemCode = new RedeemCodeDto
                    {

                        Code = o.Code,
                        ExpiryDate = o.ExpiryDate,
                        IsActive = o.IsActive,
                        Value = o.Value,
                        Note = o.Note,
                        percentage = o.percentage,
                        Id = o.Id,
                    }
                };

                results.Add(res);
            }

            return new PagedResultDto<GetRedeemCodeForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<GetRedeemCodeForViewDto> GetRedeemCodeForView(long id)
        {
            var redeemCode = await _redeemCodeRepository.GetAsync(id);

            var output = new GetRedeemCodeForViewDto { RedeemCode = ObjectMapper.Map<RedeemCodeDto>(redeemCode) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_RedeemCodes_Edit)]
        public async Task<GetRedeemCodeForEditOutput> GetRedeemCodeForEdit(EntityDto<long> input)
        {
            var redeemCode = await _redeemCodeRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetRedeemCodeForEditOutput { RedeemCode = ObjectMapper.Map<CreateOrEditRedeemCodeDto>(redeemCode) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditRedeemCodeDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Administration_RedeemCodes_Create)]
        protected virtual async Task Create(CreateOrEditRedeemCodeDto input)
        {
            var redeemCode = ObjectMapper.Map<RedeemCode>(input);

            redeemCode.Code = GenerateCode(8);

            await _redeemCodeRepository.InsertAsync(redeemCode);

        }

        [AbpAuthorize(AppPermissions.Pages_Administration_RedeemCodes_Edit)]
        protected virtual async Task Update(CreateOrEditRedeemCodeDto input)
        {
            var redeemCode = await _redeemCodeRepository.FirstOrDefaultAsync((long)input.Id);
            ObjectMapper.Map(input, redeemCode);

        }

        [AbpAuthorize(AppPermissions.Pages_Administration_RedeemCodes_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _redeemCodeRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetRedeemCodesToExcel(GetAllRedeemCodesForExcelInput input)
        {

            var filteredRedeemCodes = _redeemCodeRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Code.Contains(input.Filter) || e.Note.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CodeFilter), e => e.Code == input.CodeFilter)
                        .WhereIf(input.MinExpiryDateFilter != null, e => e.ExpiryDate >= input.MinExpiryDateFilter)
                        .WhereIf(input.MaxExpiryDateFilter != null, e => e.ExpiryDate <= input.MaxExpiryDateFilter)
                        .WhereIf(input.IsActiveFilter.HasValue && input.IsActiveFilter > -1, e => (input.IsActiveFilter == 1 && e.IsActive) || (input.IsActiveFilter == 0 && !e.IsActive))
                        .WhereIf(input.MinValueFilter != null, e => e.Value >= input.MinValueFilter)
                        .WhereIf(input.MaxValueFilter != null, e => e.Value <= input.MaxValueFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NoteFilter), e => e.Note == input.NoteFilter)
                        .WhereIf(input.MinpercentageFilter != null, e => e.percentage >= input.MinpercentageFilter)
                        .WhereIf(input.MaxpercentageFilter != null, e => e.percentage <= input.MaxpercentageFilter);

            var query = (from o in filteredRedeemCodes
                         select new GetRedeemCodeForViewDto()
                         {
                             RedeemCode = new RedeemCodeDto
                             {
                                 Code = o.Code,
                                 ExpiryDate = o.ExpiryDate,
                                 IsActive = o.IsActive,
                                 Value = o.Value,
                                 Note = o.Note,
                                 percentage = o.percentage,
                                 Id = o.Id
                             }
                         });

            var redeemCodeListDtos = await query.ToListAsync();

            return _redeemCodesExcelExporter.ExportToFile(redeemCodeListDtos);
        }


        private string GenerateCode(int length)
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}