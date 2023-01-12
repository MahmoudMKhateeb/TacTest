﻿using Abp.Domain.Repositories;
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

        private int RequestGoodsCategoryId;
        private bool IsSingleDropRequest;
        private long ShippingRequestId;
        private bool IsDedicatedRequest;
        public GoodsDetailsListExcelDataReader(TachyonExcelDataReaderHelper tachyonExcelDataReaderHelper, IRepository<RoutPoint, long> routePointRepository, IRepository<GoodCategory> goodsCategoryRepository, IRepository<DangerousGoodType> dangerousGoodTypeRepository, IRepository<UnitOfMeasure> unitOfMesureRepository)
        {
            _tachyonExcelDataReaderHelper = tachyonExcelDataReaderHelper;
            _routePointRepository = routePointRepository;
            _goodsCategoryRepository = goodsCategoryRepository;
            _dangerousGoodTypeRepository = dangerousGoodTypeRepository;
            _unitOfMesureRepository = unitOfMesureRepository;
        }

        public List<ImportGoodsDetailsDto> GetGoodsDetailsFromExcel(byte[] fileBytes, long shippingRequestId,int requestGoodsCategoryId, bool isSingleDropRequest, bool isDedicatedRequest)
        {
            RequestGoodsCategoryId = requestGoodsCategoryId;
            IsSingleDropRequest = isSingleDropRequest;
            ShippingRequestId = shippingRequestId;
            IsDedicatedRequest = isDedicatedRequest;
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
                if (!IsSingleDropRequest || IsDedicatedRequest)
                {
                    var pointReference= "";
                    if (!IsDedicatedRequest) {
                        pointReference = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                            row, 1, "Point Reference*", exceptionMessage);
                    }
                    else
                    {
                        pointReference = _tachyonExcelDataReaderHelper.GetValueFromRowOrNull<string>(worksheet,
                           row, 1, "Point Reference*", exceptionMessage);
                    }
                    goodsDetail.PointReference = pointReference;
                    var pointId = GetRoutePointIdByBulkRef(pointReference, TripReference, exceptionMessage);
                    if (pointId != null)
                    {
                        goodsDetail.RoutPointId = pointId.Value;
                    }
                }
                else
                {
                    var pointId = GetRoutePointIdByTripBulkRef(TripReference, exceptionMessage);
                    if (pointId != null)
                    {
                        goodsDetail.RoutPointId = pointId.Value;
                    }
                }
                //2
                var GoodsSubCategory = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                   row, 2, "Goods Sub Category*", exceptionMessage);
                goodsDetail.GoodsSubCategory = GoodsSubCategory.Trim();

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
                goodsDetail.Weight = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<double>(worksheet,
                   row, 4, "Weight Per Item*", exceptionMessage);
                //5
                goodsDetail.Amount = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<int>(worksheet,
                   row, 5, "Quantity*", exceptionMessage);
                //6
                goodsDetail.UnitOfMeasure = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                   row, 6, "Unit Of Measure*", exceptionMessage);


                var unitOfMesureId = GetUnitOfMesureByName(goodsDetail.UnitOfMeasure, exceptionMessage);
                if (unitOfMesureId != null)
                {
                    goodsDetail.UnitOfMeasureId = unitOfMesureId.Value;
                }
                //7
                if (goodsDetail.UnitOfMeasure.Contains(TACHYONConsts.OthersDisplayName))
                {
                    goodsDetail.OtherUnitOfMeasureName = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                   row, 7, "Other Unit Of Measure", exceptionMessage);
                }
                //8
                goodsDetail.Description = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                   row, 8, "Goods Description*", exceptionMessage);
                //9
                goodsDetail.Dimentions = _tachyonExcelDataReaderHelper.GetValueFromRowOrNull<string>(worksheet,
                   row, 9, "Package Dimensions*", exceptionMessage);

                //10
                goodsDetail.IsDangerousGood= GetBoolValueFromYesOrNo(_tachyonExcelDataReaderHelper.GetValueFromRowOrNull<string>(worksheet,
                   row, 10, "Is Dangerous Goods?*", exceptionMessage));

                if (goodsDetail.IsDangerousGood)
                {
                    goodsDetail.DangerousGoodsType = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                   row, 11,"DangerousGoodType", exceptionMessage);

                    var dangerousGoodTypeId = GetDangerousGoodIdTypeByName(goodsDetail.DangerousGoodsType, exceptionMessage);
                    if (dangerousGoodTypeId != null)
                    {
                        goodsDetail.DangerousGoodTypeId = dangerousGoodTypeId;
                    }

                    goodsDetail.DangerousGoodsCode= _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                   row, 12, "DangerousGoodsCode", exceptionMessage);
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
                x.PickingType == PickingType.Dropoff &&
                x.ShippingRequestTripFk.BulkUploadRef==tripReference &&
                x.ShippingRequestTripFk.ShippingRequestId == ShippingRequestId).Id;
            }
            catch
            {
                exceptionMessage.Append(_tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("Point"));
                return null;
            }
        }

        private long? GetRoutePointIdByTripBulkRef(string tripReference, StringBuilder exceptionMessage)
        {
            try
            {
                return _routePointRepository.Single(x =>x.ShippingRequestTripFk.BulkUploadRef == tripReference &&
                x.ShippingRequestTripFk.ShippingRequestId==ShippingRequestId &&
                x.PickingType==PickingType.Dropoff).Id;
            }
            catch
            {
                exceptionMessage.Append(_tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("Point"));
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
                    _tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("UnitOfMeasure"));
            return null;
        }
    }
}
