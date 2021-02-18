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
using TACHYON.Goods.GoodCategories;
using TACHYON.Goods.GoodsDetails.Dtos;
using TACHYON.Goods.GoodsDetails.Exporting;
using TACHYON.Routs.RoutPoints.Dtos;

namespace TACHYON.Goods.GoodsDetails
{
    [AbpAuthorize(AppPermissions.Pages_GoodsDetails)]
    public class GoodsDetailsAppService : TACHYONAppServiceBase, IGoodsDetailsAppService
    {
        private readonly IRepository<GoodsDetail, long> _goodsDetailRepository;
        private readonly IGoodsDetailsExcelExporter _goodsDetailsExcelExporter;
        private readonly IRepository<GoodCategory, int> _lookup_goodCategoryRepository;


        public GoodsDetailsAppService(IRepository<GoodsDetail, long> goodsDetailRepository, IGoodsDetailsExcelExporter goodsDetailsExcelExporter, IRepository<GoodCategory, int> lookup_goodCategoryRepository)
        {
            _goodsDetailRepository = goodsDetailRepository;
            _goodsDetailsExcelExporter = goodsDetailsExcelExporter;
            _lookup_goodCategoryRepository = lookup_goodCategoryRepository;

        }

        public async Task<PagedResultDto<GetGoodsDetailForViewDto>> GetAll(GetAllGoodsDetailsInput input)
        {

            var filteredGoodsDetails = _goodsDetailRepository.GetAll()
                        .Include(e => e.GoodCategoryFk)
                        .Where(e=>e.RoutPointId==input.RoutPointId)
                        //.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.Quantity.Contains(input.Filter) || e.Weight.Contains(input.Filter) || e.Dimentions.Contains(input.Filter) || e.DangerousGoodsCode.Contains(input.Filter))
                        // .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter)
                        .WhereIf(input.QuantityFilter > 0, e => e.Amount == input.QuantityFilter)
                        .WhereIf(input.WeightFilter.HasValue, e => e.Weight == input.WeightFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DimentionsFilter), e => e.Dimentions == input.DimentionsFilter)
                        .WhereIf(input.IsDangerousGoodFilter > -1, e => (input.IsDangerousGoodFilter == 1 && e.IsDangerousGood) || (input.IsDangerousGoodFilter == 0 && !e.IsDangerousGood))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DangerousGoodsCodeFilter), e => e.DangerousGoodsCode == input.DangerousGoodsCodeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.GoodCategoryDisplayNameFilter), e => e.GoodCategoryFk != null && e.GoodCategoryFk.DisplayName == input.GoodCategoryDisplayNameFilter);

            var pagedAndFilteredGoodsDetails = filteredGoodsDetails
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var goodsDetails = from o in pagedAndFilteredGoodsDetails
                               join o1 in _lookup_goodCategoryRepository.GetAll() on o.GoodCategoryId equals o1.Id into j1
                               from s1 in j1.DefaultIfEmpty()

                               select new GetGoodsDetailForViewDto()
                               {
                                   GoodsDetail = new GoodsDetailDto
                                   {
                                       // Name = o.Name,
                                       Description = o.Description,
                                       Amount = o.Amount,
                                       Weight = o.Weight,
                                       Dimentions = o.Dimentions,
                                       IsDangerousGood = o.IsDangerousGood,
                                       DangerousGoodsCode = o.DangerousGoodsCode,
                                       Id = o.Id
                                   },
                                   GoodCategoryDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString()
                               };

            var totalCount = await filteredGoodsDetails.CountAsync();

            return new PagedResultDto<GetGoodsDetailForViewDto>(
                totalCount,
                await goodsDetails.ToListAsync()
            );
        }

        public async Task<GetGoodsDetailForViewDto> GetGoodsDetailForView(long id)
        {
            var goodsDetail = await _goodsDetailRepository
                .GetAllIncluding(e => e.GoodCategoryFk,
                    e=>e.RoutPointFk)
                .Where(e=>e.Id==id)
                .FirstOrDefaultAsync();

            var output = new GetGoodsDetailForViewDto
            {
                GoodsDetail = ObjectMapper.Map<GoodsDetailDto>(goodsDetail),
                GoodCategoryDisplayName = goodsDetail.GoodCategoryFk.DisplayName,
                RoutPointDisplayName = goodsDetail.RoutPointFk.DisplayName
            };

            if (output.GoodsDetail.GoodCategoryId != null)
            {
                var _lookupGoodCategory = await _lookup_goodCategoryRepository.FirstOrDefaultAsync((int)output.GoodsDetail.GoodCategoryId);
                output.GoodCategoryDisplayName = _lookupGoodCategory?.DisplayName?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_GoodsDetails_Edit)]
        public async Task<GetGoodsDetailForEditOutput> GetGoodsDetailForEdit(EntityDto<long> input)
        {
            var goodsDetail = await _goodsDetailRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetGoodsDetailForEditOutput { GoodsDetail = ObjectMapper.Map<CreateOrEditGoodsDetailDto>(goodsDetail) };

            if (output.GoodsDetail.GoodCategoryId != null)
            {
                var _lookupGoodCategory = await _lookup_goodCategoryRepository.FirstOrDefaultAsync((int)output.GoodsDetail.GoodCategoryId);
                output.GoodCategoryDisplayName = _lookupGoodCategory?.DisplayName?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditGoodsDetailDto input)
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

        [AbpAuthorize(AppPermissions.Pages_GoodsDetails_Create)]
        protected virtual async Task Create(CreateOrEditGoodsDetailDto input)
        {
            var goodsDetail = ObjectMapper.Map<GoodsDetail>(input);


            if (AbpSession.TenantId != null)
            {
                goodsDetail.TenantId = (int)AbpSession.TenantId;
            }


            await _goodsDetailRepository.InsertAsync(goodsDetail);
        }

        [AbpAuthorize(AppPermissions.Pages_GoodsDetails_Edit)]
        protected virtual async Task Update(CreateOrEditGoodsDetailDto input)
        {
            var goodsDetail = await _goodsDetailRepository.FirstOrDefaultAsync((long)input.Id);
            ObjectMapper.Map(input, goodsDetail);
        }

        [AbpAuthorize(AppPermissions.Pages_GoodsDetails_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _goodsDetailRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetGoodsDetailsToExcel(GetAllGoodsDetailsForExcelInput input)
        {

            var filteredGoodsDetails = _goodsDetailRepository.GetAll()
                        .Include(e => e.GoodCategoryFk)
                        //.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.Quantity.Contains(input.Filter) || e.Weight.Contains(input.Filter) || e.Dimentions.Contains(input.Filter) || e.DangerousGoodsCode.Contains(input.Filter))
                        //.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter)
                        .WhereIf(input.QuantityFilter > 0, e => e.Amount == input.QuantityFilter)
                        .WhereIf(input.WeightFilter.HasValue, e => e.Weight == input.WeightFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DimentionsFilter), e => e.Dimentions == input.DimentionsFilter)
                        .WhereIf(input.IsDangerousGoodFilter > -1, e => (input.IsDangerousGoodFilter == 1 && e.IsDangerousGood) || (input.IsDangerousGoodFilter == 0 && !e.IsDangerousGood))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DangerousGoodsCodeFilter), e => e.DangerousGoodsCode == input.DangerousGoodsCodeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.GoodCategoryDisplayNameFilter), e => e.GoodCategoryFk != null && e.GoodCategoryFk.DisplayName == input.GoodCategoryDisplayNameFilter);

            var query = (from o in filteredGoodsDetails
                         join o1 in _lookup_goodCategoryRepository.GetAll() on o.GoodCategoryId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         select new GetGoodsDetailForViewDto()
                         {
                             GoodsDetail = new GoodsDetailDto
                             {
                                 //Name = o.Name,
                                 Description = o.Description,
                                 Amount = o.Amount,
                                 Weight = o.Weight,
                                 Dimentions = o.Dimentions,
                                 IsDangerousGood = o.IsDangerousGood,
                                 DangerousGoodsCode = o.DangerousGoodsCode,
                                 Id = o.Id
                             },
                             GoodCategoryDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString()
                         });


            var goodsDetailListDtos = await query.ToListAsync();

            return _goodsDetailsExcelExporter.ExportToFile(goodsDetailListDtos);
        }


        [AbpAuthorize(AppPermissions.Pages_GoodsDetails)]
        public async Task<List<GoodsDetailGoodCategoryLookupTableDto>> GetAllGoodCategoryForTableDropdown(int? fatherId)
        {
            return await _lookup_goodCategoryRepository.GetAll()
                .Where(x=>x.FatherId==fatherId)
                .Select(goodCategory => new GoodsDetailGoodCategoryLookupTableDto
                {
                    Id = goodCategory.Id,
                    DisplayName = goodCategory == null || goodCategory.DisplayName == null ? "" : goodCategory.DisplayName.ToString()
                }).ToListAsync();
        }

    }
}