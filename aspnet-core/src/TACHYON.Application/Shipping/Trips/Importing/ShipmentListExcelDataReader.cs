using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.DataExporting.Excel;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;

namespace TACHYON.Shipping.Trips.Importing
{
    public class ShipmentListExcelDataReader : NpoiExcelImporterBase<ImportTripDto>, IShipmentListExcelDataReader
    {
        private readonly TachyonExcelDataReaderHelper _tachyonExcelDataReaderHelper;
        private readonly ShippingRequestTripManager _shippingRequestTripManager;
        private long ShippingRequestId;

        public ShipmentListExcelDataReader(TachyonExcelDataReaderHelper tachyonExcelDataReaderHelper, ShippingRequestTripManager shippingRequestTripManager)
        {
            _tachyonExcelDataReaderHelper = tachyonExcelDataReaderHelper;
            _shippingRequestTripManager = shippingRequestTripManager;
        }
        public List<ImportTripDto> GetShipmentsFromExcel(byte[] fileBytes, long shippingRequestId)
        {
            ShippingRequestId = shippingRequestId;
            return ProcessExcelFile(fileBytes, ProcessExcelRow);
        }

        private ImportTripDto ProcessExcelRow(ISheet worksheet, int row)
        {
            if (_tachyonExcelDataReaderHelper.IsRowEmpty(worksheet, row))
            {
                return null;
            }
            StringBuilder exceptionMessage = new StringBuilder();
            ImportTripDto trip = new ImportTripDto();
            try
            {
                trip.ShippingRequestId = ShippingRequestId;
                //0
                trip.BulkUploadReference = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                    row, 0, "Reference No", exceptionMessage);
                //1
                trip.StartTripDate = GetDateTimeValueFromTextOrNull(_tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                    row, 1, "Trip Pick up Date Start *", exceptionMessage));

                //2
                trip.EndTripDate= GetDateTimeValueFromTextOrNull(_tachyonExcelDataReaderHelper.GetValueFromRowOrNull<string>(worksheet,
                    row, 2, "Trip Pick up Date End", exceptionMessage));

                //3
                trip.TotalValue= _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                    row, 3, "Approximate Total Value of Goods", exceptionMessage);
                //4
                trip.Note= _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                    row, 4, "Notes to Carrier", exceptionMessage);
                //5
                trip.NeedsDeliveryNote= GetBoolValueFromYesOrNo(_tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                    row, 5, "Needs Delivery Note ?", exceptionMessage));
                //6
                trip.HasAttachment = GetBoolValueFromYesOrNo(_tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                    row, 6, "Has Attchment ?", exceptionMessage));

                _shippingRequestTripManager.ValidateTripDto(trip, exceptionMessage);
                //_shippingRequestTripManager.ValidateDuplicateBulkReferenceFromDB(trip, ShippingRequestId);

                if (exceptionMessage.Length > 0)
                {
                    trip.Exception = exceptionMessage.ToString();
                }
            }
            catch(Exception exception)
            {
                trip.Exception = exception.Message;
            }

            return trip;
        }
    }
}
