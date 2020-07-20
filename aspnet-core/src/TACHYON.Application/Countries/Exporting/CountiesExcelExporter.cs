using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Countries.Dtos;
using TACHYON.Dto;
using TACHYON.Storage;

namespace TACHYON.Countries.Exporting
{
    public class CountiesExcelExporter : NpoiExcelExporterBase, ICountiesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public CountiesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetCountyForViewDto> counties)
        {
            return CreateExcelPackage(
                "Counties.xlsx",
                excelPackage =>
                {
                    
                    var sheet = excelPackage.CreateSheet(L("Counties"));

                    AddHeader(
                        sheet,
                        L("DisplayName"),
                        L("Code")
                        );

                    AddObjects(
                        sheet, 2, counties,
                        _ => _.County.DisplayName,
                        _ => _.County.Code
                        );

					
					
                });
        }
    }
}
