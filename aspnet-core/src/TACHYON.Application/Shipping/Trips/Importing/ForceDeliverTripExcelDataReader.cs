using Abp.Dependency;
using Abp.Extensions;
using Abp.Localization.Sources;
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
        
        public IEnumerable<ImportTripTransactionFromExcelDto> GetTripDeliveryDetails(byte[] fileBytes)
        {
           return ProcessExcelFile(fileBytes, ProcessExcelRow);
        }

        private static ImportTripTransactionFromExcelDto ProcessExcelRow(ISheet worksheet, int row)
        {
            if (IsRowEmpty(worksheet, row))
            {
                return null;
            }
            
            var point = new ImportTripTransactionFromExcelDto();

            try
            {
                var currentRow = worksheet.GetRow(row);
                var waybillNumber = currentRow.Cells[0].StringCellValue;

                point.WaybillNumber = Convert.ToInt64(waybillNumber);

                var transactionsDate = new List<DateTime>();

                for (int i = 1; i < (currentRow.Cells.Count); i++)
                {
                    var date = currentRow.Cells[i].StringCellValue;
                    if (date.IsNullOrEmpty()) break;
                    
                    var transactionDate = DateTime.ParseExact(date, "dd/MM/yyyy - HH:mm", CultureInfo.CurrentUICulture);
                    transactionsDate.Add(transactionDate);
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