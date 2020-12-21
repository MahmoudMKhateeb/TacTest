using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using System.Collections.Generic;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Dto;
using TACHYON.Storage;
using TACHYON.Trucks.Dtos;

namespace TACHYON.Trucks.Exporting
{
    public class TrucksExcelExporter : NpoiExcelExporterBase, ITrucksExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public TrucksExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetTruckForViewDto> trucks)
        {
            return CreateExcelPackage(
                "Trucks.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("Trucks"));

                    AddHeader(
                        sheet,
                        L("PlateNumber"),
                       // L("Driver"),
                        L("TrucksType"),
                        L("ModelName"),
                        L("ModelYear"),
                        L("TruckStatus") ,
                        L("Note")
                        );

                    AddObjects(
                        sheet, 2, trucks,
                        _ => _.Truck.PlateNumber,
                       // _ => _.UserName,
                        _ => _.TrucksTypeDisplayName,
                        _ => _.Truck.ModelName,
                        _ => _.Truck.ModelYear,
                        _ => _.TruckStatusDisplayName,
                        _ => _.Truck.Note

                        );


                    //for (var i = 1; i <= trucks.Count; i++)
                    //{
                    //    SetCellDataFormat(sheet.GetRow(i).Cells[5], "yyyy-mm-dd");
                    //}
                    sheet.AutoSizeColumn(2);
                });
        }
    }
}