﻿using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.Dto;
using TACHYON.Storage;

namespace TACHYON.Goods.GoodCategories.Exporting
{
    public class GoodCategoriesExcelExporter : NpoiExcelExporterBase, IGoodCategoriesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public GoodCategoriesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetGoodCategoryForViewDto> goodCategories)
        {
            return CreateExcelPackage(
                "GoodCategories.xlsx",
                excelPackage =>
                {
                    
                    var sheet = excelPackage.CreateSheet(L("GoodCategories"));

                    AddHeader(
                        sheet,
                        L("DisplayName")
                        );

                    AddObjects(
                        sheet, 2, goodCategories,
                        _ => _.GoodCategory.DisplayName
                        );

					
					
                });
        }
    }
}