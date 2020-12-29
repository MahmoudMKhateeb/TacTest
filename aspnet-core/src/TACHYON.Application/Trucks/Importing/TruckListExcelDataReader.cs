using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Localization;
using Abp.Localization.Sources;
using NPOI.SS.UserModel;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Documents.DocumentTypes;
using TACHYON.Trucks.Importing.Dto;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TrucksTypes;

namespace TACHYON.Trucks.Importing
{
    public class TruckListExcelDataReader : NpoiExcelImporterBase<ImportTruckDto>, ITruckListExcelDataReader
    {
        private readonly ILocalizationSource _localizationSource;
        private readonly IRepository<TransportType> _transportTypeRepository;
        private readonly IRepository<TrucksType, long> _trucksTypeRepository;
        private readonly IRepository<DocumentType, long> _documentTypeRepository;

        private List<TransportType> TransportTypes { get; set; }
        private List<TrucksType> TrucksTypes { get; set; }




        public TruckListExcelDataReader(ILocalizationManager localizationManager, IRepository<TransportType> transportTypeRepository, IRepository<TrucksType, long> trucksTypeRepository, IRepository<DocumentType, long> documentTypeRepository)
        {
            _transportTypeRepository = transportTypeRepository;
            _trucksTypeRepository = trucksTypeRepository;
            _documentTypeRepository = documentTypeRepository;
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
            truck.ImportTruckDocumentFileDtos = new List<ImportTruckDocumentFileDto>();

            //TruckIstimara
            var istimaraDocumentFileDto = new ImportTruckDocumentFileDto();
            DocumentType istimaraDocumentType = new DocumentType();
            try
            {
                istimaraDocumentType = _documentTypeRepository.GetAll().First(x => x.SpecialConstant.ToLower() == "TruckIstimara".ToLower());
                istimaraDocumentFileDto.DocumentTypeId = istimaraDocumentType.Id;
            }
            catch
            {
                exceptionMessage.Append("cant find document type with constant TruckIstimara;");
            }
            istimaraDocumentFileDto.Name = "_";
            istimaraDocumentFileDto.Extn = " ";




            var insuranceDocumentFileDto = new ImportTruckDocumentFileDto();
            DocumentType insuranceDocumentType = new DocumentType();

            try
            {
                insuranceDocumentType = _documentTypeRepository.GetAll().First(x => x.SpecialConstant.ToLower() == "TruckInsurance".ToLower());

                insuranceDocumentFileDto.DocumentTypeId = insuranceDocumentType.Id;
            }
            catch
            {
                exceptionMessage.Append("cant find document type with constant TruckInsurance;");
            }
            insuranceDocumentFileDto.Name = "_";
            insuranceDocumentFileDto.Extn = " ";


            try
            {
                truck.TruckStatusId = null;

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
                truck.TrucksTypeId = GetTruckTypeId(GetRequiredValueFromRowOrNull(worksheet, row, 5, "Truck Type*", exceptionMessage), truck.TransportTypeId, exceptionMessage).Value;
                //6
                truck.ModelYear = GetRequiredValueFromRowOrNull(worksheet, row, 6, "Truck length ", exceptionMessage);
                //7
                truck.Capacity = GetRequiredValueFromRowOrNull(worksheet, row, 7, "Capacity (Payload)*", exceptionMessage);
                //8
                truck.Note = GetRequiredValueFromRowOrNull(worksheet, row, 8, "Note", exceptionMessage);

                //Istimara document
                //9
                istimaraDocumentFileDto.Number = GetRequiredValueFromRowOrNull(worksheet, row, 9, " Istimara NO*", exceptionMessage);
                //10
                istimaraDocumentFileDto.HijriExpirationDate = GetRequiredValueFromRowOrNull(worksheet, row, 10, "Istimara Expiry  Date (Hijri)*", exceptionMessage);
                //11
                istimaraDocumentFileDto.ExpirationDate = DateTime.ParseExact(GetRequiredValueFromRowOrNull(worksheet, row, 11, "Istimara Expiry  Date (Gregorian)*", exceptionMessage), "dd/MM/yyyy", null).Date;
                if (istimaraDocumentType.HasExpirationDate && istimaraDocumentFileDto.HijriExpirationDate.IsNullOrWhiteSpace())
                {
                    exceptionMessage.Append("Istimara Expiry  Date (Hijri) is required");
                }

                if (istimaraDocumentType.HasExpirationDate && istimaraDocumentFileDto.ExpirationDate == null)
                {
                    exceptionMessage.Append("Istimara Expiry  Date (Gregorian) is required");
                }

                truck.ImportTruckDocumentFileDtos.Add(istimaraDocumentFileDto);


                //Insurance document
                //12
                insuranceDocumentFileDto.Number = GetRequiredValueFromRowOrNull(worksheet, row, 12, "3rd Party Insurance Policy NO*", exceptionMessage);
                //13
                insuranceDocumentFileDto.HijriExpirationDate = GetRequiredValueFromRowOrNull(worksheet, row, 13, " 3rd party Insurance Expiry Date (Hijri)", exceptionMessage);
                //14
                insuranceDocumentFileDto.ExpirationDate = DateTime.ParseExact(GetRequiredValueFromRowOrNull(worksheet, row, 14, "3rd party Insurance Expiry Date (Gregorian)", exceptionMessage), "dd/MM/yyyy", null).Date;

                if (insuranceDocumentType.HasExpirationDate && insuranceDocumentFileDto.HijriExpirationDate.IsNullOrWhiteSpace())
                {
                    exceptionMessage.Append("Insurance Expiry  Date (Hijri) is required");
                }

                if (insuranceDocumentType.HasExpirationDate && insuranceDocumentFileDto.ExpirationDate == null)
                {
                    exceptionMessage.Append("Insurance Expiry  Date (Gregorian) is required");
                }

                truck.ImportTruckDocumentFileDtos.Add(insuranceDocumentFileDto);
                if (exceptionMessage.Length > 0)
                {
                    truck.Exception = exceptionMessage.ToString();
                }

                //default truck status active
                truck.TruckStatusId = 3;
            }
            catch (System.Exception exception)
            {
                truck.Exception = exception.Message;
            }


            return truck;
        }

        private bool GetIsAttachable(string text)
        {
            try
            {
                return text.ToLower() == "yes";
            }
            catch
            {
                // ignored

            }
            return false;
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


        private long? GetTruckTypeId(string text, int? transportTypeId, StringBuilder exceptionMessage)
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

            if (trucksType.TransportTypeId == transportTypeId)
            {
                return trucksType.Id;
            }
            exceptionMessage.Append("truckType does not belongs to transportType ");
            exceptionMessage.Append(GetLocalizedExceptionMessagePart("TruckType"));
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