using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.AddressBook.Dtos;
using TACHYON.Dto;
using TACHYON.Storage;

namespace TACHYON.AddressBook.Exporting
{
    public class FacilitiesExcelExporter : NpoiExcelExporterBase, IFacilitiesExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public FacilitiesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
            base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetFacilityForViewOutput> facilities)
        {
            return CreateExcelPackage(
                "Facilities.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet(L("Facilities"));

                    AddHeader(
                        sheet,
                        L("Name"),
                        L("Address"),
                        L("Longitude"),
                        L("latitude"),
                        ((L("City")) + L("DisplayName")),
                        L("WorkingDaysHours")

                    );

                    AddObjects(
                        sheet, 2, facilities,
                        _ => _.Facility.Name,
                        _ => _.Facility.Address,
                        _ => _.Facility.Longitude,
                        _ => _.Facility.Latitude,
                        _ => _.CityDisplayName,
                        _ => _.Facility.FacilityWorkingHours
                    );
                });
        }
    }
}