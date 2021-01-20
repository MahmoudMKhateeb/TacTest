using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.AddressBook.Ports.Dtos;
using TACHYON.Dto;
using TACHYON.Storage;

namespace TACHYON.AddressBook.Ports.Exporting
{
    public class PortsExcelExporter : NpoiExcelExporterBase, IPortsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public PortsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetPortForViewDto> ports)
        {
            return CreateExcelPackage(
                "Ports.xlsx",
                excelPackage =>
                {
                    
                    var sheet = excelPackage.CreateSheet(L("Ports"));

                    AddHeader(
                        sheet,
                        L("Name"),
                        L("Adress"),
                        L("Location"),
                        (L("City")) + L("DisplayName")
                        );

                    AddObjects(
                        sheet, 2, ports,
                        _ => _.Port.Name,
                        _ => _.Port.Adress,
                        _ => _.Port.Location,
                        _ => _.CityDisplayName
                        );

					
					
                });
        }
    }
}
