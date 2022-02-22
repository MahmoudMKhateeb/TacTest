using Abp.Domain.Repositories;
using Abp.Extensions;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TACHYON.DataExporting.Excel;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Goods;
using TACHYON.Goods.GoodCategories;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.Trips.Importing.Dto;
using TACHYON.UnitOfMeasures;

namespace TACHYON.Shipping.Trips.Importing
{
    public class GoodsDetailsListExcelDataReader : NpoiExcelImporterBase<ImportGoodsDetailsDto>, IGoodsDetailsListExcelDataReader
    {
        private readonly TachyonExcelDataReaderHelper _tachyonExcelDataReaderHelper;
        private readonly IRepository<RoutPoint, long> _routePointRepository;
        private readonly IRepository<GoodCategory> _goodsCategoryRepository;
        private readonly IRepository<DangerousGoodType> _dangerousGoodTypeRepository;
        private readonly IRepository<UnitOfMeasure> _unitOfMesureRepository;

        private long ShippingRequestId;
        private int RequestGoodsCategoryId;
        public GoodsDetailsListExcelDataReader(TachyonExcelDataReaderHelper tachyonExcelDataReaderHelper, IRepository<RoutPoint, long> routePointRepository, IRepository<GoodCategory> goodsCategoryRepository, IRepository<DangerousGoodType> dangerousGoodTypeRepository, IRepository<UnitOfMeasure> unitOfMesureRepository)
        {
            _tachyonExcelDataReaderHelper = tachyonExcelDataReaderHelper;
            _routePointRepository = routePointRepository;
            _goodsCategoryRepository = goodsCategoryRepository;
            _dangerousGoodTypeRepository = dangerousGoodTypeRepository;
            _unitOfMesureRepository = unitOfMesureRepository;
        }

        public List<ImportGoodsDetailsDto> GetGoodsDetailsFromExcel(byte[] fileBytes, long shippingRequestId, int requestGoodsCategoryId)
        {
            ShippingRequestId = shippingRequestId;
            RequestGoodsCategoryId = requestGoodsCategoryId;
            return ProcessExcelFile(fileBytes, ProcessGoodsDetailsExcelRow);
        }

        private ImportGoodsDetailsDto ProcessGoodsDetailsExcelRow(ISheet worksheet, int row)
        {
            if (_tachyonExcelDataReaderHelper.IsRowEmpty(worksheet, row))
            {
                return null;
            }
            StringBuilder exceptionMessage = new StringBuilder();
            ImportGoodsDetailsDto goodsDetail = new ImportGoodsDetailsDto();
            try
            { 
                //0
            var TripReference = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                    row, 0, "Trip Reference*", exceptionMessage);

                goodsDetail.TripReference = TripReference;
            //1
            var pointReference = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                    row, 1, "Point Reference*", exceptionMessage);
                goodsDetail.PointReference = pointReference;
                var pointId = GetRoutePointIdByBulkRef(pointReference,TripReference, exceptionMessage);
                if (pointId != null)
                {
                    goodsDetail.RoutPointId = pointId.Value;
                }

                //2
                var GoodsSubCategory = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                   row, 2, "Goods Sub Category*", exceptionMessage);
                goodsDetail.GoodsSubCategory = GoodsSubCategory;

                var GoodsCategoryId=GetGoodsCategoryId(GoodsSubCategory, exceptionMessage);
                if (GoodsCategoryId != null)
                    goodsDetail.GoodCategoryId = GoodsCategoryId.Value;

                //3
                if (GoodsSubCategory.Contains(TACHYONConsts.OthersDisplayName))
                {
                    goodsDetail.OtherGoodsCategoryName = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                   row, 3, "Other Goods Sub Category", exceptionMessage);
                }

                //4
                goodsDetail.Description = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                   row, 4, "Goods Description*", exceptionMessage);

                //5
                goodsDetail.UnitOfMeasure= _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                   row, 5, "Unit Of Measure*", exceptionMessage);

                var unitOfMesureId = GetUnitOfMesureByName(goodsDetail.UnitOfMeasure, exceptionMessage);
                if (unitOfMesureId != null)
                {
                    goodsDetail.UnitOfMeasureId = unitOfMesureId.Value;
                }

                //6
                goodsDetail.Weight = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<double>(worksheet,
                   row, 6, "Weight Per Item*", exceptionMessage);
                //7
                goodsDetail.Dimentions = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                   row, 7, "Package Dimensions*", exceptionMessage);
                //8
                goodsDetail.Amount= _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<int>(worksheet,
                   row, 8, "Quantity*", exceptionMessage);
                //9
                goodsDetail.IsDangerousGood= GetBoolValueFromYesOrNo(_tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                   row, 9, "Is Dangerous Goods?*", exceptionMessage));

                if (goodsDetail.IsDangerousGood)
                {
                    goodsDetail.DangerousGoodsType = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                   row, 10,"DangerousGoodType", exceptionMessage);

                    var dangerousGoodTypeId = GetDangerousGoodIdTypeByName(goodsDetail.DangerousGoodsType, exceptionMessage);
                    if (dangerousGoodTypeId != null)
                    {
                        goodsDetail.DangerousGoodTypeId = dangerousGoodTypeId;
                    }

                    goodsDetail.DangerousGoodsCode= _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                   row, 11, "DangerousGoodsCode", exceptionMessage);
                }


                if (exceptionMessage.Length > 0)
                {
                    goodsDetail.Exception = exceptionMessage.ToString();
                }
            }
            catch (Exception exception)
            {
                goodsDetail.Exception = exception.Message;
            }

            return goodsDetail;
        }


        private long? GetRoutePointIdByBulkRef(string pointReference, string tripReference, StringBuilder exceptionMessage)
        {
            try
            {
                return _routePointRepository.FirstOrDefault(x => x.BulkUploadReference == pointReference &&
                x.ShippingRequestTripFk.BulkUploadRef==tripReference).Id;
            }
            catch
            {
                exceptionMessage.Append(_tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("Trip"));
                return null;
            }
        }

        private int? GetGoodsCategoryId(string text, StringBuilder exceptionMessage)
        {
            if (text.IsNullOrEmpty())
            {
                exceptionMessage.Append(
                    _tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("GoodsCategory"));
                return null;
            }

            var goodsSubCategory = _goodsCategoryRepository.GetAll()
                .Include(x=>x.Translations)
                .FirstOrDefault(x=>(x.Key==text || x.Translations.Any(x=>x.DisplayName==text)) && x.FatherId== RequestGoodsCategoryId);
            if (goodsSubCategory != null)
            {
                return goodsSubCategory.Id;
            }
            exceptionMessage.Append(_tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("GoodsCategory"));
            return null;
        }

        private int? GetDangerousGoodIdTypeByName(string text, StringBuilder exceptionMessage)
        {
            var item= _dangerousGoodTypeRepository.FirstOrDefault(x => x.Name.ToLower() == text.ToLower());
            if (item != null)
            {
                return item.Id;
            }

            exceptionMessage.Append(
                    _tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("DangerousGoodType"));
            return null;
        }

        private int? GetUnitOfMesureByName(string text, StringBuilder exceptionMessage)
        {
            var item = _unitOfMesureRepository.GetAll()
                .FirstOrDefault(x => x.DisplayName.ToLower() == text.ToLower());
            if (item != null)
            {
                return item.Id;
            }

            exceptionMessage.Append(
                    _tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("UnitOfMesure"));
            return null;
        }
    }
}
