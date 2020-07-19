using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Trailers.TrailerStatuses.Dtos;
using TACHYON.Dto;
using TACHYON.Storage;

namespace TACHYON.Trailers.TrailerStatuses.Exporting
{
    public class TrailerStatusesExcelExporter : NpoiExcelExporterBase, ITrailerStatusesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public TrailerStatusesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetTrailerStatusForViewDto> trailerStatuses)
        {
            return CreateExcelPackage(
                "TrailerStatuses.xlsx",
                excelPackage =>
                {
                    
                    var sheet = excelPackage.CreateSheet(L("TrailerStatuses"));

                    AddHeader(
                        sheet,
                        L("DisplayName")
                        );

                    AddObjects(
                        sheet, 2, trailerStatuses,
                        _ => _.TrailerStatus.DisplayName
                        );

					
					
                });
        }
    }
}
