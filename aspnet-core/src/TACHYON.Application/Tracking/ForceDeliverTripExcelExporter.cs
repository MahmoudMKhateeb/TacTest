using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Dto;
using TACHYON.Storage;
using TACHYON.Tracking.Dto;

namespace TACHYON.Tracking
{
    public class ForceDeliverTripExcelExporter : NpoiExcelExporterBase
    {
        public ForceDeliverTripExcelExporter(ITempFileCacheManager tempFileCacheManager) : base(tempFileCacheManager)
        {
        }
        
        public FileDto ExportToFile(List<ExportPointExcelDto> points)
        {
    
            
            return CreateExcelPackage(
                "DeliverTripTemplate",
                workbook =>
                {
                    var sheet = workbook.CreateSheet("TripPointsDetails");
                    var headerStyle = workbook.CreateCellStyle();
                    var headerFont = workbook.CreateFont();
                    headerFont.FontName = "Calibri";
                    headerFont.FontHeightInPoints = 11;
                    headerFont.IsBold = true;
                    headerStyle.SetFont(headerFont);

                    for (var rowIndex = 0; rowIndex < points.Count; rowIndex++)
                    {
                        var realSheetRowIndex = rowIndex == 0 ? rowIndex : rowIndex * 2;
                        var row = sheet.CreateRow(realSheetRowIndex);
                        
                        
                        for (int columnIndex = 0; columnIndex <= points[rowIndex].Transactions.Count; columnIndex++)
                        {
                           
                            // in excel there is an unit of measure (point)
                            // and a one point equals value of (256) as integer number 
                            if (rowIndex == 1) sheet.SetColumnWidth(columnIndex,(33*256));
                            
                            
                            
                            if (columnIndex == 0)
                            {
                                var pointIdCell = row.CreateCell(columnIndex);
                                var pickingTypeCell = row.CreateCell(columnIndex+1);
                                
                                pointIdCell.SetCellValue(L("PointId"));
                                pickingTypeCell.SetCellValue(L("PickingType"));
                                pointIdCell.CellStyle = headerStyle;
                                pickingTypeCell.CellStyle = headerStyle;
                                
                                sheet.CreateRow(realSheetRowIndex+1)
                                    .CreateCell(columnIndex+1)
                                    .SetCellValue(points[rowIndex].PickingType.ToString());
                                
                                continue;
                            }
                            
                            var transactionCell = row.CreateCell(columnIndex+1);
                            transactionCell.SetCellValue(points[rowIndex].Transactions[columnIndex-1]);
                            transactionCell.CellStyle = headerStyle;
                        }
            
                    }
                    
                });
            
            
        }
    }
}