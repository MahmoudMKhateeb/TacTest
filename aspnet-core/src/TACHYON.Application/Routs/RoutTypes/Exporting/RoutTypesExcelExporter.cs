using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Routs.RoutTypes.Dtos;
using TACHYON.Dto;
using TACHYON.Storage;

namespace TACHYON.Routs.RoutTypes.Exporting
{
    public class RoutTypesExcelExporter : NpoiExcelExporterBase, IRoutTypesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public RoutTypesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetRoutTypeForViewDto> routTypes)
        {
            return CreateExcelPackage(
                "RoutTypes.xlsx",
                excelPackage =>
                {
                    
                    var sheet = excelPackage.CreateSheet(L("RoutTypes"));

                    AddHeader(
                        sheet,
                        L("DisplayName"),
                        L("Description")
                        );

                    AddObjects(
                        sheet, 2, routTypes,
                        _ => _.RoutType.DisplayName,
                        _ => _.RoutType.Description
                        );

					
					
                });
        }
    }
}
