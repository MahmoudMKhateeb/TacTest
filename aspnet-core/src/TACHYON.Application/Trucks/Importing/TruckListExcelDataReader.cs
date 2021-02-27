using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Localization;
using Abp.Localization.Sources;
using NPOI.SS.UserModel;
using TACHYON.DataExporting.Excel;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Documents.DocumentTypes;
using TACHYON.Trucks.Importing.Dto;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations;

namespace TACHYON.Trucks.Importing
{
    public class TruckListExcelDataReader : NpoiExcelImporterBase<ImportTruckDto>, ITruckListExcelDataReader
    {
        private readonly IRepository<TransportType> _transportTypeRepository;
        private readonly IRepository<TransportTypesTranslation> _transportTypesTranslationRepository;
        private readonly IRepository<TrucksType, long> _trucksTypeRepository;
        private readonly IRepository<TrucksTypesTranslation> _trucksTypesTranslationRepository;
        private readonly IRepository<DocumentType, long> _documentTypeRepository;
        private readonly TachyonExcelDataReaderHelper _tachyonExcelDataReaderHelper;

        private List<TransportType> TransportTypes { get; set; }
        private List<TrucksType> TrucksTypes { get; set; }



        public TruckListExcelDataReader(IRepository<TransportType> transportTypeRepository, IRepository<TrucksType, long> trucksTypeRepository, IRepository<DocumentType, long> documentTypeRepository, TachyonExcelDataReaderHelper tachyonExcelDataReaderHelper, IRepository<TransportTypesTranslation> transportTypesTranslationRepository, IRepository<TrucksTypesTranslation> trucksTypesTranslationRepository)
        {
            _transportTypeRepository = transportTypeRepository;
            _trucksTypeRepository = trucksTypeRepository;
            _documentTypeRepository = documentTypeRepository;
            _tachyonExcelDataReaderHelper = tachyonExcelDataReaderHelper;
            _transportTypesTranslationRepository = transportTypesTranslationRepository;
            _trucksTypesTranslationRepository = trucksTypesTranslationRepository;
        }

        // #GetTrucksFromExcel
        public List<ImportTruckDto> GetTrucksFromExcel(byte[] fileBytes)
        {
            return ProcessExcelFile(fileBytes, ProcessExcelRow);
        }

        private ImportTruckDto ProcessExcelRow(ISheet worksheet,
            int row)
        {
            if (_tachyonExcelDataReaderHelper.IsRowEmpty(worksheet, row))
            {
                return null;
            }

            StringBuilder exceptionMessage = new StringBuilder();
            ImportTruckDto truck = new ImportTruckDto();
            truck.ImportTruckDocumentFileDtos = new List<ImportTruckDocumentFileDto>();

            //TruckIstimara
            ImportTruckDocumentFileDto istimaraDocumentFileDto = new ImportTruckDocumentFileDto();
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
            ImportTruckDocumentFileDto insuranceDocumentFileDto = new ImportTruckDocumentFileDto();
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
                //0
                truck.PlateNumber = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet, row, 0, "Plate NO*", exceptionMessage);
                //1
                truck.ModelName = _tachyonExcelDataReaderHelper.GetValueFromRowOrNull<string>(worksheet, row, 1, "Model Name", exceptionMessage);
                //2
                truck.ModelYear = _tachyonExcelDataReaderHelper.GetValueFromRowOrNull<string>(worksheet, row, 2, "Model year", exceptionMessage);
                //3
                truck.IsAttachable = GetIsAttachable(_tachyonExcelDataReaderHelper.GetValueFromRowOrNull<string>(worksheet, row, 3, "Is attachable", exceptionMessage));
                //4
                truck.TransportTypeId = GetTransportTypeId(_tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet, row, 4, "Transport Type*", exceptionMessage), exceptionMessage);
                //5
                truck.TrucksTypeId = GetTruckTypeId(_tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet, row, 5, "Truck Type*", exceptionMessage), truck.TransportTypeId, exceptionMessage).Value;
                //6
                truck.Length = ToNullableInt(_tachyonExcelDataReaderHelper.GetValueFromRowOrNull<string>(worksheet, row, 6, "Truck length ", exceptionMessage));
                //7
                truck.Capacity = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet, row, 7, "Capacity (Payload)*", exceptionMessage);
                //8
                truck.Note = _tachyonExcelDataReaderHelper.GetValueFromRowOrNull<string>(worksheet, row, 8, "Note", exceptionMessage);

                #region Istimara document

                //9
                istimaraDocumentFileDto.Number = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet, row, 9, " Istimara NO*", exceptionMessage);

                //10
                istimaraDocumentFileDto.HijriExpirationDate = _tachyonExcelDataReaderHelper.GetValueFromRowOrNull<string>(worksheet, row, 10, "Istimara Expiry  Date (Hijri)*", exceptionMessage);
                //11
                istimaraDocumentFileDto.ExpirationDate = _tachyonExcelDataReaderHelper.GetValueFromRowOrNull<DateTime?>(worksheet, row, 11, "Istimara Expiry  Date (Gregorian)*", exceptionMessage);
                ValidateIstimaraDocumentFileDto(exceptionMessage, istimaraDocumentFileDto, istimaraDocumentType);

                truck.ImportTruckDocumentFileDtos.Add(istimaraDocumentFileDto);

                #endregion

                #region Insurance document

                //12
                insuranceDocumentFileDto.Number = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet, row, 12, "3rd Party Insurance Policy NO*", exceptionMessage);

                //13
                insuranceDocumentFileDto.HijriExpirationDate = _tachyonExcelDataReaderHelper.GetValueFromRowOrNull<string>(worksheet, row, 13, " 3rd party Insurance Expiry Date (Hijri)", exceptionMessage);
                //14
                insuranceDocumentFileDto.ExpirationDate = _tachyonExcelDataReaderHelper.GetValueFromRowOrNull<DateTime?>(worksheet, row, 14, "3rd party Insurance Expiry Date (Gregorian)", exceptionMessage);
                ValidateInsuranceDocumentFileDto(exceptionMessage, insuranceDocumentFileDto, insuranceDocumentType);

                truck.ImportTruckDocumentFileDtos.Add(insuranceDocumentFileDto);

                #endregion

                if (exceptionMessage.Length > 0)
                {
                    truck.Exception = exceptionMessage.ToString();
                }

                //default truck status active
                truck.TruckStatusId = 3;
            }
            catch (Exception exception)
            {
                truck.Exception = exception.Message;
            }

            return truck;
        }

        private void ValidateInsuranceDocumentFileDto(StringBuilder exceptionMessage, ImportTruckDocumentFileDto insuranceDocumentFileDto, DocumentType insuranceDocumentType)
        {
            if (!insuranceDocumentFileDto.Number.IsNullOrEmpty())
            {
                if (insuranceDocumentFileDto.Number.Length > insuranceDocumentType.NumberMaxDigits ||
                insuranceDocumentFileDto.Number.Length < insuranceDocumentType.NumberMinDigits)
                {
                    exceptionMessage.Append("Istimara NO should be minimum " +
                                            insuranceDocumentType.NumberMinDigits +
                                            " character; ");
                    exceptionMessage.Append("Istimara NO should be maximum " +
                                            insuranceDocumentType.NumberMaxDigits +
                                            " character; ");
                }
            }


            if (insuranceDocumentType.HasExpirationDate &&
                insuranceDocumentFileDto.HijriExpirationDate.IsNullOrEmpty() &&
                insuranceDocumentFileDto.ExpirationDate == null)
            {
                exceptionMessage.Append("Insurance Expiry  Date (Gregorian OR Hijri) is required; ");
            }

            if (!insuranceDocumentFileDto.HijriExpirationDate.IsNullOrEmpty() &&
                insuranceDocumentFileDto.ExpirationDate == null)
            {
                insuranceDocumentFileDto.ExpirationDate =
                    _tachyonExcelDataReaderHelper.GetGregorianFromHijriDateString(insuranceDocumentFileDto
                        .HijriExpirationDate);
            }

            if (insuranceDocumentFileDto.HijriExpirationDate.IsNullOrEmpty() &&
                insuranceDocumentFileDto.ExpirationDate != null)
            {
                insuranceDocumentFileDto.HijriExpirationDate =
                    _tachyonExcelDataReaderHelper.GetHijriDateStringFromGregorian(insuranceDocumentFileDto
                        .ExpirationDate.Value);
            }
        }

        private void ValidateIstimaraDocumentFileDto(StringBuilder exceptionMessage, ImportTruckDocumentFileDto istimaraDocumentFileDto, DocumentType istimaraDocumentType)
        {
            if (!istimaraDocumentFileDto.Number.IsNullOrEmpty())
            {
                if (istimaraDocumentFileDto.Number.Length > istimaraDocumentType.NumberMaxDigits ||
                 istimaraDocumentFileDto.Number.Length < istimaraDocumentType.NumberMinDigits)
                {
                    exceptionMessage.Append("Istimara NO should be minimum " +
                                            istimaraDocumentType.NumberMinDigits +
                                            " character; ");
                    exceptionMessage.Append("Istimara NO should be maximum " +
                                            istimaraDocumentType.NumberMaxDigits +
                                            " character; ");
                }
            }


            if (istimaraDocumentType.HasExpirationDate &&
                istimaraDocumentFileDto.HijriExpirationDate.IsNullOrEmpty() &&
                istimaraDocumentFileDto.ExpirationDate == null)
            {
                exceptionMessage.Append("Istimara Expiry  Date (Gregorian OR Hijri) is required; ");
            }

            if (!istimaraDocumentFileDto.HijriExpirationDate.IsNullOrEmpty() &&
                istimaraDocumentFileDto.ExpirationDate == null)
            {
                istimaraDocumentFileDto.ExpirationDate =
                    _tachyonExcelDataReaderHelper.GetGregorianFromHijriDateString(istimaraDocumentFileDto
                        .HijriExpirationDate);
            }

            if (istimaraDocumentFileDto.HijriExpirationDate.IsNullOrEmpty() &&
                istimaraDocumentFileDto.ExpirationDate != null)
            {
                istimaraDocumentFileDto.HijriExpirationDate =
                    _tachyonExcelDataReaderHelper.GetHijriDateStringFromGregorian(istimaraDocumentFileDto
                        .ExpirationDate.Value);
            }
        }

        private int? ToNullableInt(string s)
        {
            int i;
            if (int.TryParse(s, out i)) return i;
            return null;
        }

        private bool? GetIsAttachable(string text)
        {
            if (text.IsNullOrEmpty())
            {
                return null;
            }
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
                exceptionMessage.Append(_tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("TransportType"));
                return null;
            }
            //English
            var transportType = _transportTypeRepository.GetAll().FirstOrDefault(x => x.DisplayName.ToLower() == text.ToLower());
            if (transportType != null)
            {
                return transportType.Id;
            }

            //translaterd name
            var transportTypeTranslation = _transportTypesTranslationRepository.GetAll().FirstOrDefault(x => x.TranslatedDisplayName.ToLower().Trim() == text.ToLower().Trim());
            if (transportTypeTranslation != null)
            {
                return transportTypeTranslation.CoreId;
            }

            exceptionMessage.Append(_tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("TransportType"));
            return null;

        }

        private long? GetTruckTypeId(string text, int? transportTypeId, StringBuilder exceptionMessage)
        {
            long? id = null;
            //null check 
            if (text.IsNullOrEmpty())
            {
                exceptionMessage.Append(_tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("TruckType"));
                return null;
            }

            // get truckType English 
            var trucksType = _trucksTypeRepository.GetAll().FirstOrDefault(x => x.DisplayName.ToLower() == text.ToLower());
            if (trucksType != null)
            {
                id = trucksType.Id;
            }
            else
            {
                //Translated name 
                var truckTypeTranslation = _trucksTypesTranslationRepository.GetAll().FirstOrDefault(x => x.TranslatedDisplayName.ToLower().Trim() == text.ToLower().Trim());
                if (truckTypeTranslation != null)
                {
                    id = truckTypeTranslation.CoreId;
                }
            }

            if (id == null)
            {
                exceptionMessage.Append(_tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("TruckType"));

                return null;
            }

            if (trucksType.TransportTypeId == transportTypeId)
            {
                return id;
            }

            exceptionMessage.Append("truckType does not belongs to transportType ");
            return null;


        }


    }
}