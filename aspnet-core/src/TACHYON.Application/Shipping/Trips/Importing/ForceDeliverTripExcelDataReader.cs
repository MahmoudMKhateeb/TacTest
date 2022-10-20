using Abp.Dependency;
using Abp.Extensions;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.UI;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Tracking;
using TACHYON.Tracking.Dto;

namespace TACHYON.Shipping.Trips.Importing
{
    public class ForceDeliverTripExcelDataReader : NpoiExcelImporterBase<ImportTripTransactionFromExcelDto>, ITransientDependency
    {
        private readonly ILocalizationSource _localizationSource;

        public ForceDeliverTripExcelDataReader(ILocalizationManager localizationManager)
        {
            _localizationSource = localizationManager.GetSource(TACHYONConsts.LocalizationSourceName);
        }

        public IEnumerable<ImportTripTransactionFromExcelDto> GetTripDeliveryDetails(byte[] fileBytes)
        {
           return ProcessExcelFile(fileBytes, ProcessExcelRow);
        }

        private  ImportTripTransactionFromExcelDto ProcessExcelRow(ISheet worksheet, int row)
        {
            if (IsRowEmpty(worksheet, row))
            {
                return null;
            }
            
            var point = new ImportTripTransactionFromExcelDto();

            try
            {
                var currentRow = worksheet.GetRow(row);
                var waybillNumber = currentRow.Cells[0]?.StringCellValue?.Trim();

                bool isValidNumber = long.TryParse(waybillNumber, out long result);
                if (!isValidNumber) throw new UserFriendlyException(_localizationSource.GetString("NotValidWaybillNumber",row));
                point.WaybillNumber = result;

                var transactionsDate = new List<DateTime>();

                for (int i = 1; i < (currentRow.Cells.Count); i++)
                {
                    var currentCell = currentRow.Cells[i];

                    if (currentCell.CellType != CellType.String)
                        throw new UserFriendlyException(_localizationSource.GetString("NotSupportedCellFormat", row + 1,
                            i + 1));

                    var date = currentCell.StringCellValue;
                    if (date.IsNullOrEmpty()) break;
                    var isDateValid = DateTime.TryParseExact(date, "dd/MM/yyyy - HH:mm",
                        CultureInfo.CurrentUICulture, DateTimeStyles.None, out DateTime parsedDate);
                    if (!isDateValid)
                        throw new UserFriendlyException(
                            _localizationSource.GetString("NotValidDateFormatMsg", row + 1, i + 1));
                    
                    transactionsDate.Add(parsedDate);
                }

                point.TransactionsDates = transactionsDate;

            }
            catch (Exception exception)
            {
                point.Exception = exception.Message;
            }

            return point;
        }
        
        private static bool IsRowEmpty(ISheet worksheet, int row)
        {
            var cell = worksheet.GetRow(row)?.Cells.FirstOrDefault();
            return cell == null || string.IsNullOrWhiteSpace(cell.StringCellValue);
        }
    }
}