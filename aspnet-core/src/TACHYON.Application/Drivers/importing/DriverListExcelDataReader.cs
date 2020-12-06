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


            var iqamaDocumentFileDto = new CreateOrEditDocumentFileDto();
            var iqamaDocumentType = _documentTypeRepository.FirstOrDefault(x => x.SpecialConstant.ToLower() == "DriverIqama".ToLower());
            iqamaDocumentFileDto.DocumentTypeId = iqamaDocumentType.Id;
            iqamaDocumentFileDto.Name = "_";
            iqamaDocumentFileDto.Extn = " ";


            var drivingLicenseDocumentFileDto = new CreateOrEditDocumentFileDto();
            var drivingLicenseDocumentType = _documentTypeRepository.FirstOrDefault(x => x.SpecialConstant.ToLower() == "DriverDrivingLicense".ToLower());
            drivingLicenseDocumentFileDto.DocumentTypeId = drivingLicenseDocumentType.Id;
            drivingLicenseDocumentFileDto.Name = "_";
            drivingLicenseDocumentFileDto.Extn = " ";

            var occupationCardDocumentFileDto = new CreateOrEditDocumentFileDto();
            var occupationCardDocumentType = _documentTypeRepository.FirstOrDefault(x => x.SpecialConstant.ToLower() == "DriverOccupationCard".ToLower());
            occupationCardDocumentFileDto.DocumentTypeId = occupationCardDocumentType.Id;
            occupationCardDocumentFileDto.Name = "_";
            occupationCardDocumentFileDto.Extn = " ";



            try
            {

                //A0
                user.Name = GetRequiredValueFromRowOrNull(worksheet, row, 0, nameof(user.Name), exceptionMessage);
                //B1
                user.Surname = GetRequiredValueFromRowOrNull(worksheet, row, 1, nameof(user.Surname), exceptionMessage);
                //C2
                user.PhoneNumber = GetRequiredValueFromRowOrNull(worksheet, row, 3, nameof(user.PhoneNumber), exceptionMessage);



                //iqama document
                //D3
                iqamaDocumentFileDto.Number = GetRequiredValueFromRowOrNull(worksheet, row, 3, "ID/Iqama/ NO*", exceptionMessage);
                //E4
                iqamaDocumentFileDto.HijriExpirationDate = GetRequiredValueFromRowOrNull(worksheet, row, 4, "ID/Iqama Expiry Date(Hijri)", exceptionMessage);
                //F5
                iqamaDocumentFileDto.ExpirationDate = DateTime.ParseExact(GetRequiredValueFromRowOrNull(worksheet, row, 5, "ID/Iqama Expiry Date(Gregorian)", exceptionMessage), "dd/MM/yyyy", null).Date;
                if (iqamaDocumentType.HasExpirationDate && iqamaDocumentFileDto.HijriExpirationDate.IsNullOrWhiteSpace())
                {
                    throw new Exception("iqama Expiry  Date (Hijri) is required");
                }

                if (iqamaDocumentType.HasExpirationDate && iqamaDocumentFileDto.ExpirationDate == null)
                {
                    throw new Exception("iqama Expiry  Date (Gregorian) is required");
                }

                user.CreateOrEditDocumentFileDtos.Add(iqamaDocumentFileDto);



                //drivingLicense document
                //G6
                drivingLicenseDocumentFileDto.Number = GetRequiredValueFromRowOrNull(worksheet, row, 6, "Driving License NO*", exceptionMessage);
                //H7
                drivingLicenseDocumentFileDto.HijriExpirationDate = GetRequiredValueFromRowOrNull(worksheet, row, 7, "Driving License Expiry Date(Hijri)", exceptionMessage);
                //I8
                drivingLicenseDocumentFileDto.ExpirationDate = DateTime.ParseExact(GetRequiredValueFromRowOrNull(worksheet, row, 8, "Driving License Expiry Date(Gregorian)", exceptionMessage), "dd/MM/yyyy", null).Date;
                if (drivingLicenseDocumentType.HasExpirationDate && drivingLicenseDocumentFileDto.HijriExpirationDate.IsNullOrWhiteSpace())
                {
                    throw new Exception("drivingLicense Expiry  Date (Hijri) is required");
                }

                if (drivingLicenseDocumentType.HasExpirationDate && drivingLicenseDocumentFileDto.ExpirationDate == null)
                {
                    throw new Exception("drivingLicense Expiry  Date (Gregorian) is required");
                }

                user.CreateOrEditDocumentFileDtos.Add(drivingLicenseDocumentFileDto);





                //occupationCard document
                //J9
                drivingLicenseDocumentFileDto.Number = GetRequiredValueFromRowOrNull(worksheet, row, 9, "Occupation Card NO", exceptionMessage);

                user.CreateOrEditDocumentFileDtos.Add(drivingLicenseDocumentFileDto);



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