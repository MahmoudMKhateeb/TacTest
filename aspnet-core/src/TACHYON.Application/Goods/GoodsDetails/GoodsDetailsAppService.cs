using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Castle.MicroKernel.ModelBuilder.Descriptors;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Dto;
using TACHYON.Extension;
using TACHYON.Goods.GoodCategories;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.Goods.GoodsDetails.Dtos;
using TACHYON.Goods.GoodsDetails.Exporting;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.Routs.RoutSteps;
using TACHYON.UnitOfMeasures;

namespace TACHYON.Goods.GoodsDetails
{
    [AbpAuthorize(AppPermissions.Pages_GoodsDetails)]
    public class GoodsDetailsAppService : TACHYONAppServiceBase, IGoodsDetailsAppService
    {
        private readonly IRepository<GoodsDetail, long> _goodsDetailRepository;
        private readonly IRepository<RoutPoint, long> _routPointRepository;
        private readonly IGoodsDetailsExcelExporter _goodsDetailsExcelExporter;
        private readonly IRepository<GoodCategory, int> _lookup_goodCategoryRepository;
        private readonly IRepository<UnitOfMeasure> _lookup_UnitOfMeasureRepository;


        public GoodsDetailsAppService(IRepository<GoodsDetail, long> goodsDetailRepository,
            IGoodsDetailsExcelExporter goodsDetailsExcelExporter,
            IRepository<GoodCategory, int> lookup_goodCategoryRepository,
            IRepository<RoutPoint, long> routPointRepository,
            IRepository<UnitOfMeasure> lookupUnitOfMeasureRepository)
        {
            _goodsDetailRepository = goodsDetailRepository;
            _goodsDetailsExcelExporter = goodsDetailsExcelExporter;
            _lookup_goodCategoryRepository = lookup_goodCategoryRepository;
            _routPointRepository = routPointRepository;
            _lookup_UnitOfMeasureRepository = lookupUnitOfMeasureRepository;
        }

        public async Task<PagedResultDto<GetGoodsDetailForViewDto>> GetAll(GetAllGoodsDetailsInput input)
        {

            var filteredGoodsDetails = _goodsDetailRepository.GetAll()
                        .Include(e => e.GoodCategoryFk)
                        .ThenInclude(e => e.Translations)
                        .Where(e => e.RoutPointId == input.RoutPointId)
                        //.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.Quantity.Contains(input.Filter) || e.Weight.Contains(input.Filter) || e.Dimentions.Contains(input.Filter) || e.DangerousGoodsCode.Contains(input.Filter))
                        // .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter)
                        .WhereIf(input.QuantityFilter > 0, e => e.Amount == input.QuantityFilter)
                        .WhereIf(input.WeightFilter.HasValue, e => e.Weight == input.WeightFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DimentionsFilter), e => e.Dimentions == input.DimentionsFilter)
                        .WhereIf(input.IsDangerousGoodFilter > -1, e => (input.IsDangerousGoodFilter == 1 && e.IsDangerousGood) || (input.IsDangerousGoodFilter == 0 && !e.IsDangerousGood))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DangerousGoodsCodeFilter), e => e.DangerousGoodsCode == input.DangerousGoodsCodeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.GoodCategoryDisplayNameFilter), e => e.GoodCategoryFk != null && e.GoodCategoryFk.Translations.Any(x => x.DisplayName.Contains(input.GoodCategoryDisplayNameFilter)));

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
                                       Id = o.Id,
                                       GoodCategoryId = o.GoodCategoryId,
                                       GoodCategory = ObjectMapper.Map<GoodCategoryDto>(o.GoodCategoryFk).DisplayName
                                   },
                                   GoodCategoryDisplayName = ObjectMapper.Map<GoodCategoryDto>(o.GoodCategoryFk).DisplayName
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
                .GetAll().Include(e => e.GoodCategoryFk)
                .ThenInclude(e => e.Translations)
                .Include(e => e.RoutPointFk)
                .Where(e => e.Id == id)
                .FirstOrDefaultAsync();

            var output = new GetGoodsDetailForViewDto
            {
                GoodsDetail = ObjectMapper.Map<GoodsDetailDto>(goodsDetail),
                GoodCategoryDisplayName = ObjectMapper.Map<GoodCategoryDto>(goodsDetail.GoodCategoryFk).DisplayName,
                RoutPointDisplayName = goodsDetail.RoutPointFk.DisplayName
            };

            if (output.GoodsDetail.GoodCategoryId != null)
            {
                var _lookupGoodCategory = await _lookup_goodCategoryRepository.FirstOrDefaultAsync((int)output.GoodsDetail.GoodCategoryId);
                output.GoodCategoryDisplayName = ObjectMapper.Map<GoodCategoryDto>(_lookupGoodCategory).DisplayName;
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_GoodsDetails_Edit)]
        public async Task<GetGoodsDetailForEditOutput> GetGoodsDetailForEdit(EntityDto<long> input)
        {
            var goodsDetail = await _goodsDetailRepository.GetAll()
                .Include(x => x.GoodCategoryFk)
                .ThenInclude(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            var output = new GetGoodsDetailForEditOutput { GoodsDetail = ObjectMapper.Map<CreateOrEditGoodsDetailDto>(goodsDetail) };

            if (output.GoodsDetail.GoodCategoryId != null)
            {
                var _lookupGoodCategory = await _lookup_goodCategoryRepository.FirstOrDefaultAsync((int)output.GoodsDetail.GoodCategoryId);
                output.GoodCategoryDisplayName = ObjectMapper.Map<GoodCategoryDto>(_lookupGoodCategory).DisplayName;//_lookupGoodCategory?.DisplayName?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditGoodsDetailDto input)
        {
            //? This Validation Can't Added In Custom Validation
            var unitOfMeasure = await _lookup_UnitOfMeasureRepository.GetAll()
                .SingleAsync(x => x.Id == input.UnitOfMeasureId);

            if (unitOfMeasure.DisplayName.ToLower().Contains(TACHYONConsts.OthersDisplayName)
                && input.OtherUnitOfMeasureName.IsNullOrEmpty())
                throw new UserFriendlyException(L("OtherNameIsRequired"));

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


            //if (AbpSession.TenantId != null)
            //{
            //    goodsDetail.TenantId = (int)AbpSession.TenantId;
            //}


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
                        .WhereIf(!string.IsNullOrWhiteSpace(input.GoodCategoryDisplayNameFilter), e => e.GoodCategoryFk != null && e.GoodCategoryFk.Translations.Any(x => x.DisplayName == input.GoodCategoryDisplayNameFilter));

            var query = (from o in filteredGoodsDetails
                         join o1 in _lookup_goodCategoryRepository.GetAll() on o.GoodCategoryId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         select new GetGoodsDetailForViewDto()
                         {
                             GoodsDetail = new GoodsDetailDto
                             {
                                 Description = o.Description,
                                 Amount = o.Amount,
                                 Weight = o.Weight,
                                 Dimentions = o.Dimentions,
                                 IsDangerousGood = o.IsDangerousGood,
                                 DangerousGoodsCode = o.DangerousGoodsCode,
                                 Id = o.Id
                             },
                             GoodCategoryDisplayName = ObjectMapper.Map<GoodCategoryDto>(o.GoodCategoryFk).DisplayName// s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString()
                         });


            var goodsDetailListDtos = await query.ToListAsync();

            return _goodsDetailsExcelExporter.ExportToFile(goodsDetailListDtos);
        }


        [AbpAuthorize(AppPermissions.Pages_GoodsDetails)]
        public async Task<List<GetAllGoodsCategoriesForDropDownOutput>> GetAllGoodCategoryForTableDropdown(int? fatherId)
        {

            var list = await _lookup_goodCategoryRepository.GetAll()
                .Include(x => x.Translations)
                .Where(x => x.FatherId == fatherId)
                .ToListAsync();

            return ObjectMapper.Map<List<GetAllGoodsCategoriesForDropDownOutput>>(list);
        }

        #region Waybills
        public IEnumerable<GetGoodsDetailsForWaybillsOutput> GetShippingrequestGoodsDetailsForSingleDropWaybill(int shippingRequestTripId)
        {
            var dropPoint = _routPointRepository.Single(x =>
                x.ShippingRequestTripId == shippingRequestTripId && x.PickingType == PickingType.Dropoff);

            var goods = _goodsDetailRepository.GetAll()
                .Include(x => x.GoodCategoryFk)
                .ThenInclude(x => x.Translations)
                .Where(x => x.RoutPointId == dropPoint.Id);

            var query = goods.Select(x => new
            {
                Description = x.Description,
                TotalAmount = x.Amount,
                Weight = x.Weight,
                UnitOfMeasureDisplayName = x.UnitOfMeasureFk.DisplayName,
                SubCategory = x.GoodCategoryFk
            });

            var output = query.ToList().Select(e =>
                new GetGoodsDetailsForWaybillsOutput()
                {
                    UnitOfMeasureDisplayName = e.UnitOfMeasureDisplayName,
                    TotalAmount = e.TotalAmount,
                    Description = e.Description,
                    SubCategory = ObjectMapper.Map<GoodCategoryDto>(e.SubCategory)?.DisplayName

                });
            return output;
        }

        public IEnumerable<GetGoodsDetailsForWaybillsOutput> GetShippingrequestGoodsDetailsForMultipleDropWaybill(long RoutPointId)
        {
            var goods = _goodsDetailRepository.GetAll()
                .Include(x => x.GoodCategoryFk)
                .ThenInclude(x => x.Translations)
                .Where(x => x.RoutPointId == RoutPointId);

            var query = goods.Select(x => new
            {
                Description = x.Description,
                //Amount which will be dropped
                TotalAmount = x.Amount,
                UnitOfMeasureDisplayName = x.UnitOfMeasureFk.DisplayName,
                SubCategory = x.GoodCategoryFk
            });

            var output = query.ToList().Select(e =>
                new GetGoodsDetailsForWaybillsOutput()
                {
                    UnitOfMeasureDisplayName = e.UnitOfMeasureDisplayName,
                    TotalAmount = e.TotalAmount,
                    Description = e.Description,
                    SubCategory = ObjectMapper.Map<GoodCategoryDto>(e.SubCategory)?.DisplayName
                });
            return output;
        }



        #endregion

    }
}