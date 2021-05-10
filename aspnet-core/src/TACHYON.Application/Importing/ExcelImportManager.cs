using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TACHYON.Importing
{
    public class ExcelImportManager<TEntity> : IExcelImportManager<TEntity>
    {
        public List<TEntity> ImportFromFile(byte[] fileBytes)
        {
            ISheet sheet;
            using (var stream = new MemoryStream(fileBytes))
            {
                var workbook = new XSSFWorkbook(stream);
                sheet = workbook.GetSheetAt(0);
            }
            IRow headerRow = sheet.GetRow(0); //Get Header Row
            List<ReportColumn> Columns = new List<ReportColumn>();
            foreach (var cell in headerRow.Cells)
            {
                if (string.IsNullOrWhiteSpace(cell.ToString())) continue;

                Columns.Add(new ReportColumn { Name = cell.ToString().Replace(" ", "").Trim(), Index = cell.ColumnIndex });
            }
            int cellCount = headerRow.LastCellNum;

            List<TEntity> Entities = new List<TEntity>();
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                TEntity Entity = (TEntity)Activator.CreateInstance(typeof(TEntity));
                foreach (var Col in Columns)
                {
                    var prop = Entity.GetType().GetProperty(Col.Name);
                    if (prop == null) continue;
                    ICell cell = row.GetCell(Col.Index);
                    if (cell != null)
                        prop.SetValue(Entity, Convert.ChangeType(GetCellAsString(cell), prop.PropertyType), null);
                    else
                        prop.SetValue(Entity, Convert.ChangeType(null, prop.PropertyType), null);
                }
                Entities.Add(Entity);
            }
            return Entities;
        }


        private object GetCellAsString(ICell cell)
        {
            switch (cell.CellType)
            {
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                case CellType.Error:
                    return cell.ErrorCellValue.ToString();
                case CellType.Formula:
                    return cell.CellFormula.ToString();
                //ALL CELLS IN THE EXEMPEL ARE NUMERIC AND GOES HERE
                case CellType.Numeric:

                    return DateUtil.IsCellDateFormatted(cell)
                            ? (object)cell.DateCellValue
                            : (object)cell.NumericCellValue;
                case CellType.String:
                    return cell.StringCellValue.ToString().Trim();
                case CellType.Blank:
                case CellType.Unknown:
                default:
                    return "";
            }
        }
    }

    public class ReportColumn
    {
        public string Name { get; set; }
        public int Index { get; set; }
    }
}
