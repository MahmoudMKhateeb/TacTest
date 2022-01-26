using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Localization;
using Abp.Localization.Sources;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TACHYON.Authorization.Users;
using TACHYON.Authorization.Users.Importing.Dto;
using TACHYON.DataExporting.Excel;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Documents.DocumentTypes;
using TACHYON.Drivers.importing.Dto;
using TACHYON.Validation;

namespace TACHYON.Drivers.importing
{
    public class DriverListExcelDataReader : NpoiExcelImporterBase<ImportDriverDto>, IDriverListExcelDataReader
    {
        private readonly IRepository<DocumentType, long> _documentTypeRepository;
        private readonly TachyonExcelDataReaderHelper _tachyonExcelDataReaderHelper;


        public DriverListExcelDataReader(IRepository<DocumentType, long> documentTypeRepository, TachyonExcelDataReaderHelper tachyonExcelDataReaderHelper)
        {
            _documentTypeRepository = documentTypeRepository;
            _tachyonExcelDataReaderHelper = tachyonExcelDataReaderHelper;

        }

        // #GetDriversFromExcel
        public List<ImportDriverDto> GetDriversFromExcel(byte[] fileBytes)
        {
            return ProcessExcelFile(fileBytes, ProcessExcelRow);
        }

        private ImportDriverDto ProcessExcelRow(ISheet worksheet, int row)
        {
            if (_tachyonExcelDataReaderHelper.IsRowEmpty(worksheet, row))
            {
                return null;
            }

            var exceptionMessage = new StringBuilder();
            var user = new ImportDriverDto();
            user.CreateOrEditDocumentFileDtos = new List<CreateOrEditDocumentFileDto>();

            #region Required documents initiate

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


            #endregion



            try
            {

                //0
                user.Name = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet, row, 0, nameof(user.Name), exceptionMessage);
                //1
                user.Surname = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet, row, 1, nameof(user.Surname), exceptionMessage);
                //2
                user.PhoneNumber = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet, row, 2, nameof(user.PhoneNumber), exceptionMessage);
                ValidatePhoneNumber(user, exceptionMessage);
                //3
                user.EmailAddress = _tachyonExcelDataReaderHelper.GetValueFromRowOrNull<string>(worksheet, row, 3, nameof(user.EmailAddress), exceptionMessage);
                if (!user.EmailAddress.IsNullOrEmpty() && !ValidationHelper.IsEmail(user.EmailAddress))
                {
                    exceptionMessage.Append("EmailAddress is not valid; ");
                }
                //4
                //5

                user.DateOfBirth = _tachyonExcelDataReaderHelper.GetValueFromRowOrNull<DateTime>(worksheet, row, 5, nameof(user.DateOfBirth), exceptionMessage);

                //iqama document
                //6
                iqamaDocumentFileDto.Number = _tachyonExcelDataReaderHelper.GetRequiredValuesFromRowOrNull<string>(worksheet, row, 6, "ID/Iqama/ NO*", exceptionMessage);

                //7
                iqamaDocumentFileDto.HijriExpirationDate = _tachyonExcelDataReaderHelper.GetValueFromRowOrNull<string>(worksheet, row, 7, "ID/Iqama Expiry Date(Hijri)", exceptionMessage);
                //8
                iqamaDocumentFileDto.ExpirationDate = _tachyonExcelDataReaderHelper.GetValueFromRowOrNull<DateTime?>(worksheet, row, 8, "ID/Iqama Expiry Date(Gregorian)", exceptionMessage);
                ValidateIqamaDocumentFileDto(exceptionMessage, iqamaDocumentFileDto, iqamaDocumentType);

                user.CreateOrEditDocumentFileDtos.Add(iqamaDocumentFileDto);



                //drivingLicense document
                //9
                drivingLicenseDocumentFileDto.Number = _tachyonExcelDataReaderHelper.GetRequiredValuesFromRowOrNull<string>(worksheet, row, 9, "Driving License NO*", exceptionMessage);
                //10
                drivingLicenseDocumentFileDto.HijriExpirationDate = _tachyonExcelDataReaderHelper.GetValueFromRowOrNull<string>(worksheet, row, 10, "Driving License Expiry Date(Hijri)", exceptionMessage);
                //11
                drivingLicenseDocumentFileDto.ExpirationDate = _tachyonExcelDataReaderHelper.GetValueFromRowOrNull<DateTime?>(worksheet, row, 11, "Driving License Expiry Date(Gregorian)", exceptionMessage);
                ValidateDrivingLicenseDocumentFileDto(exceptionMessage, drivingLicenseDocumentFileDto, drivingLicenseDocumentType);

                user.CreateOrEditDocumentFileDtos.Add(drivingLicenseDocumentFileDto);


                //occupationCard document
                //12
                occupationCardDocumentFileDto.Number = _tachyonExcelDataReaderHelper.GetValueFromRowOrNull<string>(worksheet, row, 12, "Occupation Card NO", exceptionMessage);

                user.CreateOrEditDocumentFileDtos.Add(occupationCardDocumentFileDto);

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

        private void ValidatePhoneNumber(ImportDriverDto user, StringBuilder exceptionMessage)
        {
            if (user.PhoneNumber.Length != UserConsts.DriverPhoneNumberLength)
            {
                exceptionMessage.Append("PhoneNumber should be " + UserConsts.DriverPhoneNumberLength.ToString() + " character;");
            }
        }

        private void ValidateIqamaDocumentFileDto(StringBuilder exceptionMessage, CreateOrEditDocumentFileDto iqamaDocumentFileDto, DocumentType iqamaDocumentType)
        {
            if (!iqamaDocumentFileDto.Number.IsNullOrEmpty())
            {
                if (iqamaDocumentFileDto.Number.Length > iqamaDocumentType.NumberMaxDigits ||
                 iqamaDocumentFileDto.Number.Length < iqamaDocumentType.NumberMinDigits)
                {
                    exceptionMessage.Append("ID/Iqama NO should be minimum " +
                                            iqamaDocumentType.NumberMinDigits +
                                            " character; ");
                    exceptionMessage.Append("ID/Iqama NO should be maximum " +
                                            iqamaDocumentType.NumberMaxDigits +
                                            " character; ");
                }
            }

            if (!iqamaDocumentFileDto.HijriExpirationDate.IsNullOrWhiteSpace() &&
                iqamaDocumentFileDto.ExpirationDate == null)
            {
                iqamaDocumentFileDto.ExpirationDate =
                    _tachyonExcelDataReaderHelper.GetGregorianFromHijriDateString(iqamaDocumentFileDto
                        .HijriExpirationDate);
            }

            if (iqamaDocumentFileDto.HijriExpirationDate.IsNullOrWhiteSpace() &&
                iqamaDocumentFileDto.ExpirationDate != null)
            {
                iqamaDocumentFileDto.HijriExpirationDate =
                    _tachyonExcelDataReaderHelper.GetHijriDateStringFromGregorian(iqamaDocumentFileDto
                        .ExpirationDate.Value);
            }
        }

        private void ValidateDrivingLicenseDocumentFileDto(StringBuilder exceptionMessage, CreateOrEditDocumentFileDto drivingLicenseDocumentFileDto, DocumentType drivingLicenseDocumentType)
        {
            if (!drivingLicenseDocumentFileDto.Number.IsNullOrEmpty())
            {
                if (drivingLicenseDocumentFileDto.Number.Length > drivingLicenseDocumentType.NumberMaxDigits ||
                 drivingLicenseDocumentFileDto.Number.Length < drivingLicenseDocumentType.NumberMinDigits)
                {
                    exceptionMessage.Append("drivingLicense NO should be minimum " +
                                            drivingLicenseDocumentType.NumberMinDigits +
                                            " character; ");
                    exceptionMessage.Append("drivingLicense NO should be maximum " +
                                            drivingLicenseDocumentType.NumberMaxDigits +
                                            " character; ");
                }
            }
            if (!drivingLicenseDocumentFileDto.HijriExpirationDate.IsNullOrWhiteSpace() &&
                drivingLicenseDocumentFileDto.ExpirationDate == null)
            {
                drivingLicenseDocumentFileDto.ExpirationDate =
                    _tachyonExcelDataReaderHelper.GetGregorianFromHijriDateString(drivingLicenseDocumentFileDto
                        .HijriExpirationDate);
            }

            if (drivingLicenseDocumentFileDto.HijriExpirationDate.IsNullOrWhiteSpace() &&
                drivingLicenseDocumentFileDto.ExpirationDate != null)
            {
                drivingLicenseDocumentFileDto.HijriExpirationDate =
                    _tachyonExcelDataReaderHelper.GetHijriDateStringFromGregorian(drivingLicenseDocumentFileDto
                        .ExpirationDate.Value);
            }
        }
    }
}