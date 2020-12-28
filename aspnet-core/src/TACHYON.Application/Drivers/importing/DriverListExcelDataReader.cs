using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Localization;
using Abp.Localization.Sources;
using NPOI.SS.UserModel;
using TACHYON.Authorization.Users.Importing.Dto;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Documents.DocumentTypes;
using TACHYON.Drivers.importing.Dto;

namespace TACHYON.Drivers.importing
{
    public class DriverListExcelDataReader : NpoiExcelImporterBase<ImportDriverDto>, IDriverListExcelDataReader
    {
        private readonly ILocalizationSource _localizationSource;
        private readonly IRepository<DocumentType, long> _documentTypeRepository;


        public DriverListExcelDataReader(ILocalizationManager localizationManager, IRepository<DocumentType, long> documentTypeRepository)
        {
            _documentTypeRepository = documentTypeRepository;
            _localizationSource = localizationManager.GetSource(TACHYONConsts.LocalizationSourceName);
        }

        public List<ImportDriverDto> GetDriversFromExcel(byte[] fileBytes)
        {
            return ProcessExcelFile(fileBytes, ProcessExcelRow);
        }

        private ImportDriverDto ProcessExcelRow(ISheet worksheet, int row)
        {
            if (IsRowEmpty(worksheet, row))
            {
                return null;
            }

            var exceptionMessage = new StringBuilder();
            var user = new ImportDriverDto();
            user.CreateOrEditDocumentFileDtos = new List<CreateOrEditDocumentFileDto>();

            //DriverIqama
            var iqamaDocumentFileDto = new CreateOrEditDocumentFileDto();
            DocumentType iqamaDocumentType = new DocumentType();
            try
            {
                iqamaDocumentType = _documentTypeRepository.GetAll().First(x => x.SpecialConstant.ToLower() == "DriverIqama".ToLower());
                iqamaDocumentFileDto.DocumentTypeId = iqamaDocumentType.Id;
            }
            catch
            {
                exceptionMessage.Append("cant find document type with constant DriverIqama;");
            }

            iqamaDocumentFileDto.Name = "_";
            iqamaDocumentFileDto.Extn = " ";


            //DriverDrivingLicense
            var drivingLicenseDocumentFileDto = new CreateOrEditDocumentFileDto();
            DocumentType drivingLicenseDocumentType = new DocumentType();
            try
            {
                drivingLicenseDocumentType = _documentTypeRepository.GetAll().First(x => x.SpecialConstant.ToLower() == "DriverDrivingLicense".ToLower());
                drivingLicenseDocumentFileDto.DocumentTypeId = drivingLicenseDocumentType.Id;
            }
            catch
            {
                exceptionMessage.Append("cant find document type with constant DriverDrivingLicense;");
            }

            drivingLicenseDocumentFileDto.Name = "_";
            drivingLicenseDocumentFileDto.Extn = " ";


            //DriverOccupationCard
            var occupationCardDocumentFileDto = new CreateOrEditDocumentFileDto();
            DocumentType occupationCardDocumentType = new DocumentType();
            try
            {
                occupationCardDocumentType = _documentTypeRepository.GetAll().First(x => x.SpecialConstant.ToLower() == "DriverOccupationCard".ToLower());
                occupationCardDocumentFileDto.DocumentTypeId = occupationCardDocumentType.Id;
            }
            catch
            {
                exceptionMessage.Append("cant find document type with constant DriverOccupationCard;");
            }

            occupationCardDocumentFileDto.Name = "_";
            occupationCardDocumentFileDto.Extn = " ";



            try
            {

                //0
                user.Name = GetRequiredValueFromRowOrNull(worksheet, row, 0, nameof(user.Name), exceptionMessage);
                //1
                user.Surname = GetRequiredValueFromRowOrNull(worksheet, row, 1, nameof(user.Surname), exceptionMessage);
                //2
                user.PhoneNumber = GetRequiredValueFromRowOrNull(worksheet, row, 2, nameof(user.PhoneNumber), exceptionMessage);
                //3
                user.EmailAddress = GetRequiredValueFromRowOrNull(worksheet, row, 3, nameof(user.EmailAddress), exceptionMessage);
                //4
                user.DateOfBirth = DateTime.ParseExact(GetRequiredValueFromRowOrNull(worksheet, row, 4, nameof(user.DateOfBirth), exceptionMessage), "dd/MM/yyyy", null).Date;);
                //5

                //iqama document
                //6
                iqamaDocumentFileDto.Number = GetRequiredValueFromRowOrNull(worksheet, row, 6, "ID/Iqama/ NO*", exceptionMessage);
                //7
                iqamaDocumentFileDto.HijriExpirationDate = GetRequiredValueFromRowOrNull(worksheet, row, 7, "ID/Iqama Expiry Date(Hijri)", exceptionMessage);
                //8
                iqamaDocumentFileDto.ExpirationDate = DateTime.ParseExact(GetRequiredValueFromRowOrNull(worksheet, row, 8, "ID/Iqama Expiry Date(Gregorian)", exceptionMessage), "dd/MM/yyyy", null).Date;
                if (iqamaDocumentType.HasExpirationDate && iqamaDocumentFileDto.HijriExpirationDate.IsNullOrWhiteSpace())
                {
                    exceptionMessage.Append("iqama Expiry  Date (Hijri) is required;");
                }

                if (iqamaDocumentType.HasExpirationDate && iqamaDocumentFileDto.ExpirationDate == null)
                {
                    exceptionMessage.Append("iqama Expiry  Date (Gregorian) is required;");
                }

                user.CreateOrEditDocumentFileDtos.Add(iqamaDocumentFileDto);



                //drivingLicense document
                //9
                drivingLicenseDocumentFileDto.Number = GetRequiredValueFromRowOrNull(worksheet, row, 9, "Driving License NO*", exceptionMessage);
                //10
                drivingLicenseDocumentFileDto.HijriExpirationDate = GetRequiredValueFromRowOrNull(worksheet, row, 10, "Driving License Expiry Date(Hijri)", exceptionMessage);
                //11
                drivingLicenseDocumentFileDto.ExpirationDate = DateTime.ParseExact(GetRequiredValueFromRowOrNull(worksheet, row, 11, "Driving License Expiry Date(Gregorian)", exceptionMessage), "dd/MM/yyyy", null).Date;
                if (drivingLicenseDocumentType.HasExpirationDate && drivingLicenseDocumentFileDto.HijriExpirationDate.IsNullOrWhiteSpace())
                {
                    exceptionMessage.Append("drivingLicense Expiry  Date (Hijri) is required;");
                }

                if (drivingLicenseDocumentType.HasExpirationDate && drivingLicenseDocumentFileDto.ExpirationDate == null)
                {
                    exceptionMessage.Append("drivingLicense Expiry  Date (Gregorian) is required;");
                }

                user.CreateOrEditDocumentFileDtos.Add(drivingLicenseDocumentFileDto);





                //occupationCard document
                //12
                drivingLicenseDocumentFileDto.Number = GetRequiredValueFromRowOrNull(worksheet, row, 12, "Occupation Card NO", exceptionMessage);

                user.CreateOrEditDocumentFileDtos.Add(drivingLicenseDocumentFileDto);

                if (exceptionMessage.Length > 0)
                {
                    user.Exception = exceptionMessage.ToString();
                }

            }
            catch (System.Exception exception)
            {
                user.Exception = exception.Message;
            }

            return user;
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
            return cell == null || string.IsNullOrWhiteSpace(cell.StringCellValue);
        }
    }
}