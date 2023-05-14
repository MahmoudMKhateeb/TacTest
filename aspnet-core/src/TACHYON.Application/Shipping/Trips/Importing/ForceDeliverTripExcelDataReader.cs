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
using System.Runtime.CompilerServices;
using System.Text;
using TACHYON.DataExporting.Excel;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Tracking;
using TACHYON.Tracking.Dto;

namespace TACHYON.Shipping.Trips.Importing
{
    public class ForceDeliverTripExcelDataReader : NpoiExcelImporterBase<ImportTripTransactionFromExcelDto>, ITransientDependency
    {
        private readonly ILocalizationSource _localizationSource;
        private readonly TachyonExcelDataReaderHelper _tachyonExcelDataReaderHelper;


        public ForceDeliverTripExcelDataReader(ILocalizationManager localizationManager, TachyonExcelDataReaderHelper tachyonExcelDataReaderHelper)
        {
            _localizationSource = localizationManager.GetSource(TACHYONConsts.LocalizationSourceName);
            _tachyonExcelDataReaderHelper = tachyonExcelDataReaderHelper;
        }

        public IEnumerable<ImportTripTransactionFromExcelDto> GetTripDeliveryDetails(byte[] fileBytes)
        {
           return ProcessExcelFile(fileBytes, ProcessExcelRow);
        }

        private  ImportTripTransactionFromExcelDto ProcessExcelRow(ISheet worksheet, int row)
        {
            if (_tachyonExcelDataReaderHelper.IsRowEmpty(worksheet, row))
            {
                return null;
            }

            var point = new ImportTripTransactionFromExcelDto();

            try
            {
                StringBuilder exceptionMessage = new ();

                var currentRow = worksheet.GetRow(row);
                var waybillNumber = currentRow.Cells[0]?.StringCellValue?.Trim();

                bool isValidNumber = long.TryParse(waybillNumber, out long result);
                if (!isValidNumber)
                {
                    exceptionMessage.Append(
                    _tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("NotValidWaybillNumber") +";");
                }
                point.WaybillNumber = result;

                var transactionsDate = new List<DateTime>();

                string[] fileDates = { "Start moving to loading location", "Arrive to loading location", "Start loading", "Finish Loading", 
                    "Start moving to offloading location", "Arrive to offloading location", " Start offloading", "Finish offloading", " Reciever Confirmed" };
                              

                for (int i = 1; i < (currentRow.Cells.Count); i++)
                {
                    var currentCell = currentRow.Cells[i];

                    if (currentCell.CellType == CellType.Blank) break;


                    var date = currentCell.StringCellValue;
                    if (date.IsNullOrEmpty() || date.IsNullOrWhiteSpace()) break;
                    var isDateValid = DateTime.TryParseExact(date.Trim(), "dd/MM/yyyy - HH:mm",
                        CultureInfo.CurrentUICulture, DateTimeStyles.None, out DateTime parsedDate);
                    if (!isDateValid)
                    {
                        var dateLabel = fileDates[i - 1];
                        exceptionMessage.Append(_localizationSource.GetString("NotValidDateFormatMsg", dateLabel) +"; ");
                    }
                    else
                    {
                        transactionsDate.Add(parsedDate);
                    }
                }

                point.TransactionsDates = transactionsDate;


                if (exceptionMessage.Length > 0)
                {
                    point.Exception = exceptionMessage.ToString();
                }
            }
            catch (Exception exception)
            {
                point.Exception += exception.Message;
            }

            return point;
        }
        
    }
}