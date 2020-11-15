using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Localization;
using Abp.Localization.Sources;
using NPOI.SS.UserModel;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Trucks.Importing.Dto;
using TACHYON.Trucks.TruckCategories.TransportSubtypes;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TruckCategories.TruckSubtypes;
using TACHYON.Trucks.TrucksTypes;

namespace TACHYON.Trucks.Importing
{
    public class TruckListExcelDataReader : NpoiExcelImporterBase<ImportTruckDto>, ITruckListExcelDataReader
    {
        private readonly ILocalizationSource _localizationSource;
        private readonly IRepository<TransportType> _transportTypeRepository;
        private readonly IRepository<TransportSubtype> _transportSubtypeRepository;
        private readonly IRepository<TrucksType, long> _trucksTypeRepository;
        private readonly IRepository<TruckSubtype> _truckSubtypeRepository;

        private List<TransportType> TransportTypes { get; set; }
        private List<TransportSubtype> TransportSubtypes { get; set; }
        private List<TrucksType> TrucksTypes { get; set; }
        private List<TruckSubtype> TruckSubtypes { get; set; }




        public TruckListExcelDataReader(ILocalizationManager localizationManager, IRepository<TransportType> transportTypeRepository, IRepository<TransportSubtype> transportSubtypeRepository, IRepository<TrucksType, long> trucksTypeRepository, IRepository<TruckSubtype> truckSubtypeRepository)
        {
            _transportTypeRepository = transportTypeRepository;
            _transportSubtypeRepository = transportSubtypeRepository;
            _trucksTypeRepository = trucksTypeRepository;
            _truckSubtypeRepository = truckSubtypeRepository;
            _localizationSource = localizationManager.GetSource(TACHYONConsts.LocalizationSourceName);
        }

        public List<ImportTruckDto> GetTrucksFromExcel(byte[] fileBytes)
        {
            return ProcessExcelFile(fileBytes, ProcessExcelRow);
        }

        private ImportTruckDto ProcessExcelRow(ISheet worksheet, int row)
        {
            if (IsRowEmpty(worksheet, row))
            {
                return null;
            }

            var exceptionMessage = new StringBuilder();
            var truck = new ImportTruckDto();

            try
            {
                //0
                truck.PlateNumber = GetRequiredValueFromRowOrNull(worksheet, row, 0, "Plate NO*", exceptionMessage);
                //1
                truck.ModelName = GetRequiredValueFromRowOrNull(worksheet, row, 1, "Model Name", exceptionMessage);
                //2
                truck.ModelYear = GetRequiredValueFromRowOrNull(worksheet, row, 2, "Model year", exceptionMessage);
                //3
                truck.IsAttachable = GetIsAttachable(GetRequiredValueFromRowOrNull(worksheet, row, 3, "Is attachable", exceptionMessage));
                //4
                truck.TransportTypeId = GetTransportTypeId(GetRequiredValueFromRowOrNull(worksheet, row, 4, "Transport Type*", exceptionMessage), exceptionMessage);
                //5
                truck.TransportSubtypeId = GetTransportSubtypeId(GetRequiredValueFromRowOrNull(worksheet, row, 5, "Transport Subtype", exceptionMessage), truck.TransportTypeId, exceptionMessage);
                //6
                truck.TrucksTypeId = GetTruckTypeId(GetRequiredValueFromRowOrNull(worksheet, row, 6, "Truck Type*", exceptionMessage), truck.TransportSubtypeId, exceptionMessage).Value;
                //7
                truck.TruckSubtypeId = GetTruckSubTypeId(GetRequiredValueFromRowOrNull(worksheet, row, 7, "Truck Subtype", exceptionMessage), truck.TrucksTypeId, exceptionMessage);
                //8
                truck.CapacityId = Convert.ToInt32(GetRequiredValueFromRowOrNull(worksheet, row, 8, "Capacity (Payload)*", exceptionMessage));
                //9
                truck.Note = GetRequiredValueFromRowOrNull(worksheet, row, 9, "Note", exceptionMessage);

            }
            catch (System.Exception exception)
            {
                truck.Exception = exception.Message;
            }

            return truck;
        }

        private bool GetIsAttachable(string text)
        {
            return text.ToLower() == "yes";
        }
        private int? GetTransportTypeId(string text, StringBuilder exceptionMessage)
        {
            if (text.IsNullOrEmpty())
            {
                exceptionMessage.Append(GetLocalizedExceptionMessagePart("TransportType"));
                return null;
            }
            var transportType = _transportTypeRepository.GetAll().FirstOrDefault(x => x.DisplayName.ToLower() == text.ToLower());
            if (transportType != null)
            {
                return transportType.Id;
            }

            exceptionMessage.Append(GetLocalizedExceptionMessagePart("TransportType"));
            return null;

        }

        private int? GetTransportSubtypeId(string text, int? transportTypeId, StringBuilder exceptionMessage)
        {
            if (text.IsNullOrEmpty())
            {
                exceptionMessage.Append(GetLocalizedExceptionMessagePart("TransportSubtype"));
                return null;
            }

            var transportSubtype = _transportSubtypeRepository.GetAll().FirstOrDefault(x => x.DisplayName.ToLower() == text.ToLower());

            if (transportSubtype == null)
            {
                return null;
            }

            if (transportSubtype.TransportTypeId == transportTypeId)
            {
                return transportSubtype.Id;
            }

            exceptionMessage.Append(GetLocalizedExceptionMessagePart("TransportSubtype"));
            return null;

        }

        private long? GetTruckTypeId(string text, int? transportSubtypeId, StringBuilder exceptionMessage)
        {
            if (text.IsNullOrEmpty())
            {
                exceptionMessage.Append(GetLocalizedExceptionMessagePart("TruckType"));
                return null;
            }

            var trucksType = _trucksTypeRepository.GetAll().FirstOrDefault(x => x.DisplayName.ToLower() == text.ToLower());

            if (trucksType == null)
            {
                return null;
            }

            if (trucksType.TransportSubtypeId == transportSubtypeId)
            {
                return trucksType.Id;
            }

            exceptionMessage.Append(GetLocalizedExceptionMessagePart("TruckType"));
            return null;

        }

        private int? GetTruckSubTypeId(string text, long? trucksTypeId, StringBuilder exceptionMessage)
        {
            if (text.IsNullOrEmpty())
            {
                exceptionMessage.Append(GetLocalizedExceptionMessagePart("TruckSubType"));
                return null;
            }

            var truckSubtype = _truckSubtypeRepository.GetAll().FirstOrDefault(x => x.DisplayName.ToLower() == text.ToLower());

            if (truckSubtype == null)
            {
                return null;
            }

            if (truckSubtype.TrucksTypeId == trucksTypeId)
            {
                return truckSubtype.Id;
            }

            exceptionMessage.Append(GetLocalizedExceptionMessagePart("TruckSubType"));
            return null;

        }


        private string GetRequiredValueFromRowOrNull(ISheet worksheet, int row, int column, string columnName, StringBuilder exceptionMessage)
        {


            IRow _row = worksheet.GetRow(row);
            List<ICell> cells = _row.Cells;
            List<string> rowData = new List<string>();
            //Using row.Cells as List / Iterator will only get you the non-empty cells.
            //The solution is to to use row.GetCell with MissingCellPolicy.CREATE_NULL_AS_BLANK and iterate by index over all cells in the row.
            var cell = _row.GetCell(column, MissingCellPolicy.CREATE_NULL_AS_BLANK);
            cell.SetCellType(CellType.String);
            var cellValue = cell.StringCellValue;
            if (cellValue != null && !string.IsNullOrWhiteSpace(cellValue))
            {
                return cellValue;
            }

            exceptionMessage.Append(GetLocalizedExceptionMessagePart(columnName));
            return null;
        }


        private string GetLocalizedExceptionMessagePart(string parameter)
        {
            return _localizationSource.GetString("{0}IsInvalid", _localizationSource.GetString(parameter)) + "; ";
        }

        private bool IsRowEmpty(ISheet worksheet, int row)
        {
            var cell = worksheet.GetRow(row)?.Cells.FirstOrDefault();
            cell.SetCellType(CellType.String);
            var result = cell == null || string.IsNullOrWhiteSpace(cell.StringCellValue);
            return result;
        }
    }
}