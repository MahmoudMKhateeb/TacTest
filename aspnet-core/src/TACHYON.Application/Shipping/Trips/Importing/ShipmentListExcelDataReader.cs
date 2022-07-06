using Abp.Domain.Repositories;
using Abp.Extensions;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.AddressBook;
using TACHYON.DataExporting.Excel;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Receivers;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.Shipping.Trips.Importing.Dto;

namespace TACHYON.Shipping.Trips.Importing
{
    public class ShipmentListExcelDataReader : NpoiExcelImporterBase<ImportTripDto>, IShipmentListExcelDataReader
    {
        private readonly TachyonExcelDataReaderHelper _tachyonExcelDataReaderHelper;
        private readonly ShippingRequestTripManager _shippingRequestTripManager;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;

        private long ShippingRequestId;
        private bool IsSingleDropRequest;

        public ShipmentListExcelDataReader(TachyonExcelDataReaderHelper tachyonExcelDataReaderHelper, ShippingRequestTripManager shippingRequestTripManager, IRepository<ShippingRequestTrip> shippingRequestTripRepository)
        {
            _tachyonExcelDataReaderHelper = tachyonExcelDataReaderHelper;
            _shippingRequestTripManager = shippingRequestTripManager;
            _shippingRequestTripRepository = shippingRequestTripRepository;

        }

        public List<ImportTripDto> GetShipmentsFromExcel(byte[] fileBytes, long shippingRequestId, bool isSingleDropRequest)
        {
            ShippingRequestId = shippingRequestId;
            IsSingleDropRequest = isSingleDropRequest;
            return ProcessExcelFile(fileBytes, ProcessShipmentsExcelRow);
        }

        private ImportTripDto ProcessShipmentsExcelRow(ISheet worksheet, int row)
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
                trip.BulkUploadRef = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                    row, 0, "Reference No", exceptionMessage);
                //1
                var startDate = _tachyonExcelDataReaderHelper.GetValueFromRowOrNull<string>(worksheet,
                    row, 1, "Trip Pick up Date Start *", exceptionMessage);
                var startTripDate = GetDateTimeValueFromTextOrNull(startDate);
                if(startTripDate == null)
                {
                    startTripDate = _shippingRequestTripManager.GetShippingRequestById(trip.ShippingRequestId).StartTripDate;
                }
                if((startTripDate == null || startTripDate == default(DateTime)))
                {
                    exceptionMessage.Append(_tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("StartTripDate"));
                }
                else
                {
                    trip.StartTripDate = startTripDate;
                }

                //2
                var endDate = _tachyonExcelDataReaderHelper.GetValueFromRowOrNull<string>(worksheet,
                    row, 2, "Trip Pick up Date End", exceptionMessage);
                var endTripDate= GetDateTimeValueFromTextOrNull(endDate);

                if(endDate!=null && (trip.EndTripDate==null || trip.EndTripDate==default(DateTime)))
                {
                    exceptionMessage.Append(_tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("EndTripDate"));
                }
                else
                {
                    trip.EndTripDate = endTripDate;
                }

                //3
                trip.TotalValue= _tachyonExcelDataReaderHelper.GetValueFromRowOrNull<string>(worksheet,
                    row, 3, "Approximate Total Value of Goods", exceptionMessage);
                //4
                trip.Note= _tachyonExcelDataReaderHelper.GetValueFromRowOrNull<string>(worksheet,
                    row, 4, "Notes to Carrier", exceptionMessage);
                //5
                trip.NeedsDeliveryNote= GetBoolValueFromYesOrNo(_tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                    row, 5, "Needs Delivery Note ?*", exceptionMessage));
                //6
                trip.HasAttachment = GetBoolValueFromYesOrNo(_tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                    row, 6, "Has Attachment ?*", exceptionMessage));

                var originFacility = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                    row, 7, "Original Facility*", exceptionMessage);
                trip.OriginalFacility = originFacility.Trim();
                trip.OriginFacilityId = GetFacilityId(originFacility, exceptionMessage);

                var destinationFacility = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                   row, 8, "Destination Facility*", exceptionMessage);
                trip.DestinationFacility = destinationFacility.Trim();
                trip.DestinationFacilityId = GetFacilityId(destinationFacility, exceptionMessage);

                if (IsSingleDropRequest)
                {
                    var sender = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                    row, 9, "Sender*", exceptionMessage);
                    trip.sender = sender.Trim();
                    if (trip.OriginFacilityId != null)
                    {
                        trip.SenderId = GetReceiverId(sender, exceptionMessage, trip.OriginFacilityId.Value);
                    }

                    var receiver = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                    row, 10, "Receiver*", exceptionMessage);
                    trip.Receiver = receiver.Trim();
                    if (trip.DestinationFacilityId != null)
                    {
                        trip.ReceiverId = GetReceiverId(receiver, exceptionMessage, trip.DestinationFacilityId.Value);
                    }
                }
                _shippingRequestTripManager.ValidateTripDto(trip, exceptionMessage);

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

        private long? GetFacilityId(string text, StringBuilder exceptionMessage)
        {
            if (text.IsNullOrEmpty())
            {
                exceptionMessage.Append(
                    _tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("Facility"));
                return null;
            }
            text = text.Trim();

            var facility = _shippingRequestTripManager.GetFacilityByPermission(text, ShippingRequestId);
            //_facilityRepository.FirstOrDefault(x => x.Name == text);
            if (facility != null)
                return facility.Id;

            exceptionMessage.Append(_tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("Facility"));
            return null;
        }

        private int? GetReceiverId(string text, StringBuilder exceptionMessage, long facilityId)
        {
            if (text.IsNullOrEmpty())
            {
                exceptionMessage.Append(
                    _tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("Receiver"));
                return null;
            }

            text = text.Trim();
            var receiver = _shippingRequestTripManager.GetReceiverByPermissionAndFacility(text, ShippingRequestId, facilityId);
            //_receiverRepository.FirstOrDefault(x => x.FullName == text);
            if (receiver != null)
                return receiver.Id;

            exceptionMessage.Append(_tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("Receiver"));
            return null;
        }

    }
}
