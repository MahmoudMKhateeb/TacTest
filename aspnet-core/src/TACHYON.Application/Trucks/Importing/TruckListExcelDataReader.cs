using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.Localization;
using Abp.Localization.Sources;
using NPOI.SS.UserModel;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Trucks.Importing.Dto;

namespace TACHYON.Trucks.Importing
{
    public class TruckListExcelDataReader : NpoiExcelImporterBase<ImportTruckDto>, ITruckListExcelDataReader
    {
        private readonly ILocalizationSource _localizationSource;

        public TruckListExcelDataReader(ILocalizationManager localizationManager)
        {
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
                truck.PlateNumber = GetRequiredValueFromRowOrNull(worksheet, row, 0, nameof(truck.PlateNumber), exceptionMessage);
                truck.ModelName = GetRequiredValueFromRowOrNull(worksheet, row, 1, nameof(truck.ModelName), exceptionMessage);
                truck.ModelYear = GetRequiredValueFromRowOrNull(worksheet, row, 2, nameof(truck.ModelYear), exceptionMessage);
                truck.IsAttachable = Convert.ToBoolean(GetRequiredValueFromRowOrNull(worksheet, row, 3, nameof(truck.IsAttachable), exceptionMessage));
                truck.Note = GetRequiredValueFromRowOrNull(worksheet, row, 4, nameof(truck.Note), exceptionMessage);
                truck.TruckStatusId = Convert.ToInt64(GetRequiredValueFromRowOrNull(worksheet, row, 5, nameof(truck.TruckStatusId), exceptionMessage));
                truck.Driver1UserId = Convert.ToInt64(GetRequiredValueFromRowOrNull(worksheet, row, 6, nameof(truck.Driver1UserId), exceptionMessage));
                truck.TransportTypeId = Convert.ToInt32(GetRequiredValueFromRowOrNull(worksheet, row, 7, nameof(truck.TransportTypeId), exceptionMessage));
                truck.TransportSubtypeId = Convert.ToInt32(GetRequiredValueFromRowOrNull(worksheet, row, 8, nameof(truck.TransportSubtypeId), exceptionMessage));
                truck.TrucksTypeId = Convert.ToInt32(GetRequiredValueFromRowOrNull(worksheet, row, 9, nameof(truck.TrucksTypeId), exceptionMessage));
                truck.TruckSubtypeId = Convert.ToInt32(GetRequiredValueFromRowOrNull(worksheet, row, 10, nameof(truck.TruckSubtypeId), exceptionMessage));
                truck.CapacityId = Convert.ToInt32(GetRequiredValueFromRowOrNull(worksheet, row, 11, nameof(truck.CapacityId), exceptionMessage));

            }
            catch (System.Exception exception)
            {
                truck.Exception = exception.Message;
            }

            return truck;
        }

        private string GetRequiredValueFromRowOrNull(ISheet worksheet, int row, int column, string columnName, StringBuilder exceptionMessage)
        {
            var cell = worksheet.GetRow(row).Cells[column];
            cell.SetCellType(CellType.String);
            var cellValue = worksheet.GetRow(row).Cells[column].StringCellValue;
            if (cellValue != null && !string.IsNullOrWhiteSpace(cellValue))
            {
                return cellValue;
            }

            exceptionMessage.Append(GetLocalizedExceptionMessagePart(columnName));
            return null;
        }

        private string[] GetAssignedRoleNamesFromRow(ISheet worksheet, int row, int column)
        {
            var cellValue = worksheet.GetRow(row).Cells[column].StringCellValue;
            if (cellValue == null || string.IsNullOrWhiteSpace(cellValue))
            {
                return new string[0];
            }

            return cellValue.ToString().Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
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