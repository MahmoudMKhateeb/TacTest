using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Receivers.Dtos;
using TACHYON.Dto;
using TACHYON.Storage;

namespace TACHYON.Receivers.Exporting
{
    public class ReceiversExcelExporter : NpoiExcelExporterBase, IReceiversExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public ReceiversExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
            base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetReceiverForViewDto> receivers)
        {
            return CreateExcelPackage(
                "Receivers.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet(L("Receivers"));

                    AddHeader(
                        sheet,
                        L("FullName"),
                        L("Email"),
                        L("PhoneNumber"),
                        (L("Facility")) + L("Name")
                    );

                    AddObjects(
                        sheet, 2, receivers,
                        _ => _.Receiver.FullName,
                        _ => _.Receiver.Email,
                        _ => _.Receiver.PhoneNumber,
                        _ => _.FacilityName
                    );
                });
        }
    }
}