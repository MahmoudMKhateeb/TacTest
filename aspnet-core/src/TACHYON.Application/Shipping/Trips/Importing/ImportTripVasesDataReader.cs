using Abp.Domain.Repositories;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TACHYON.DataExporting.Excel;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Importing.Dto;
using TACHYON.ShippingRequestVases;
using TACHYON.Vases;

namespace TACHYON.Shipping.Trips.Importing
{
    public class ImportTripVasesDataReader : NpoiExcelImporterBase<ImportTripVasesDto>, IImportTripVasesDataReader
    {
        private readonly TachyonExcelDataReaderHelper _tachyonExcelDataReaderHelper;
        private readonly IRepository<ShippingRequestVas,long> _shippingRequestVasRepository;
        private readonly ShippingRequestTripManager _shippingRequestTripManager;
        private long shippingRequestId;

        public ImportTripVasesDataReader(TachyonExcelDataReaderHelper tachyonExcelDataReaderHelper, IRepository<ShippingRequestVas, long> shippingRequestVasRepository, ShippingRequestTripManager shippingRequestTripManager)
        {
            _tachyonExcelDataReaderHelper = tachyonExcelDataReaderHelper;
            _shippingRequestVasRepository = shippingRequestVasRepository;
            _shippingRequestTripManager = shippingRequestTripManager;
        }

        public List<ImportTripVasesDto> GetTripVasesFromExcel(byte[] fileBytes, long ShippingRequestId)
        {
            shippingRequestId = ShippingRequestId;
            return ProcessExcelFile(fileBytes, ProcessTripVasesExcelRow);
        }

        private ImportTripVasesDto ProcessTripVasesExcelRow(ISheet worksheet, int row)
        {
            if (_tachyonExcelDataReaderHelper.IsRowEmpty(worksheet, row))
            {
                return null;
            }
            StringBuilder exceptionMessage = new StringBuilder();
            ImportTripVasesDto TripVas = new ImportTripVasesDto();
            try
            {
                TripVas.ShippingRequestId = shippingRequestId;
                var TripReference = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                    row, 0, "Trip Reference*", exceptionMessage);

                TripVas.TripReference = TripReference;
                var tripId = GetTripIdFromBulk(TripReference, exceptionMessage, shippingRequestId);
                if (tripId != null)
                {
                    TripVas.ShippingRequestTripId = tripId.Value;
                }

                var VasName = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                            row, 1, "Vas Name*", exceptionMessage);
                TripVas.VasName = VasName.Trim();
                var vasId = GetVasId(VasName, exceptionMessage);
                if (vasId != null)
                {
                    TripVas.ShippingRequestVasId = vasId.Value;
                }

                if (exceptionMessage.Length > 0)
                {
                    TripVas.Exception = exceptionMessage.ToString();
                }
            }
            catch(Exception exception)
            {
                TripVas.Exception = exception.Message;
            }
            return TripVas;
        }

        private long? GetVasId(string vasName, StringBuilder exceptionMessage)
        {
            if(!string.IsNullOrEmpty(vasName)) vasName = vasName.Trim();
            var vasId= _shippingRequestVasRepository.GetAll().Where(x => x.ShippingRequestId == shippingRequestId &&
            (x.VasFk.Name.ToLower() == vasName.ToLower() ||
            x.VasFk.Translations.Any(x => x.DisplayName.ToLower() == vasName.ToLower())
            )).Select(x => x.Id).FirstOrDefault();
            if (vasId != 0)
            {
                return vasId;
            }

            exceptionMessage.Append(_tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("Vas"));
            return null;
        }

        private int? GetTripIdFromBulk(string bulkRef, StringBuilder exceptionMessage, long shippingRequestId)
        {
            var trip = _shippingRequestTripManager.GetShippingRequestTripIdByBulkRef(bulkRef, shippingRequestId);
            if (trip != null)
            {
                return trip.Id;
            }
            exceptionMessage.Append(_tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("Trip"));
            return null;
        }

    }
}
