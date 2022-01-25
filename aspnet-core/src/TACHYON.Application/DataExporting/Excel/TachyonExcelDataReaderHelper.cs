using Abp.Dependency;
using Abp.Extensions;
using Abp.Localization;
using Abp.Localization.Sources;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TACHYON.DataExporting.Excel
{
    public class TachyonExcelDataReaderHelper : ITransientDependency
    {
        private readonly ILocalizationSource _localizationSource;

        public TachyonExcelDataReaderHelper(ILocalizationManager localizationManager)
        {
            _localizationSource = localizationManager.GetSource(TACHYONConsts.LocalizationSourceName);
        }

        public T GetRequiredValueFromRowOrNull<T>(ISheet worksheet, int row, int column, string columnName, StringBuilder exceptionMessage)
        {
            var value = GetValueFromRowOrNull<T>(worksheet, row, column, columnName, exceptionMessage);
            if (value != null)
            {
                return value;
            }
            exceptionMessage.Append(GetLocalizedExceptionMessagePart(columnName));
            return default(T);
        }
        public T GetRequiredValuesFromRowOrNull<T>(ISheet worksheet, int row, int column, string columnName, StringBuilder exceptionMessage)
        {
            var value = GetValueFromRowOrNull<T>(worksheet, row, column, columnName, exceptionMessage);
            if (value != null)
            {
                return value;
            }
            return default(T);
        }
        public T GetValueFromRowOrNull<T>(ISheet worksheet, int row, int column, string columnName, StringBuilder exceptionMessage)
        {
            var value = _GetStringValueFromRowOrNull(worksheet, row, column, columnName, exceptionMessage);


            if (value == null)
            {
                return default(T);
            }
            if (typeof(T) == typeof(DateTime))
            {
                object r = DateTime.ParseExact(value, "d/M/yyyy", null);
                return (T)Convert.ChangeType(r, typeof(T));
            }
            else if (typeof(T) == typeof(DateTime?))
            {

                var d = DateTime.ParseExact(value, "d/M/yyyy", null);
                DateTime? dt = d;

                var type = typeof(T);
                type = Nullable.GetUnderlyingType(type) ?? type;

                return (T)Convert.ChangeType(dt, type);
            }
            return (T)Convert.ChangeType(value, typeof(T));
        }


        private string _GetStringValueFromRowOrNull(ISheet worksheet, int row, int column, string columnName, StringBuilder exceptionMessage)
        {
            IRow _row = worksheet.GetRow(row);
            List<ICell> cells = _row.Cells;
            List<string> rowData = new List<string>();
            //Using row.Cells as List / Iterator will only get you the non-empty cells.
            //The solution is to to use row.GetCell with MissingCellPolicy.CREATE_NULL_AS_BLANK and iterate by index over all cells in the row.
            var cell = _row.GetCell(column, MissingCellPolicy.CREATE_NULL_AS_BLANK);
            string cellValue = GetFormattedCellValue(cell);
            return cellValue.IsNullOrEmpty() ? null : cellValue;
        }
        public string GetLocalizedExceptionMessagePart(string parameter)
        {
            return _localizationSource.GetString("{0}IsInvalid", _localizationSource.GetString(parameter)) + "; ";
        }

        public bool IsRowEmpty(ISheet worksheet, int row)
        {
            var cell = worksheet.GetRow(row)?.Cells.FirstOrDefault();
            if (cell == null) return true;

            cell.SetCellType(CellType.String);
            var result = cell == null || string.IsNullOrWhiteSpace(cell.StringCellValue);
            return result;
        }

        public DateTime GetGregorianFromHijriDateString(string dateString)
        {
            var sa = CultureInfo.CreateSpecificCulture("ar-SA");
            return DateTime.ParseExact(dateString, "d/M/yyyy", sa);
        }
        public string GetHijriDateStringFromGregorian(DateTime date)
        {
            var sa = CultureInfo.CreateSpecificCulture("ar-SA");
            return date.ToString("d/M/yyyy", sa);
        }

        public string GetFormattedCellValue(ICell cell, IFormulaEvaluator eval = null)
        {
            if (cell != null)
            {
                switch (cell.CellType)
                {
                    case CellType.String:
                        return cell.StringCellValue;

                    case CellType.Numeric:
                        if (DateUtil.IsCellDateFormatted(cell))
                        {

                            try
                            {
                                string formattedCellValue = cell.DateCellValue.ToString("d/M/yyyy");
                                return formattedCellValue;
                            }
                            catch (NullReferenceException)
                            {
                                string formattedCellValue = DateTime.FromOADate(cell.NumericCellValue).ToString("d/M/yyyy");
                                return formattedCellValue;
                            }
                        }
                        else
                        {
                            return cell.NumericCellValue.ToString();
                        }

                    case CellType.Boolean:
                        return cell.BooleanCellValue ? "TRUE" : "FALSE";

                    case CellType.Formula:
                        if (eval != null)
                            return GetFormattedCellValue(eval.EvaluateInCell(cell));
                        else
                            return cell.CellFormula;

                    case CellType.Error:
                        return FormulaError.ForInt(cell.ErrorCellValue).String;
                }
            }
            // null or blank cell, or unknown cell type
            return string.Empty;
        }


    }
}