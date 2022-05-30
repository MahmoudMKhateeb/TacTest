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
using TACHYON.Shipping.Trips.Importing.Dto;

namespace TACHYON.Shipping.Trips.Importing
{
    public class RoutePointListDataReader : NpoiExcelImporterBase<ImportRoutePointDto>, IRoutePointListDataReader
    {
        private readonly TachyonExcelDataReaderHelper _tachyonExcelDataReaderHelper;
        private readonly ShippingRequestTripManager _shippingRequestTripManager;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;

        private long ShippingRequestId;
        public RoutePointListDataReader(TachyonExcelDataReaderHelper tachyonExcelDataReaderHelper, IRepository<ShippingRequestTrip> shippingRequestTripRepository, ShippingRequestTripManager shippingRequestTripManager)
        {
            _tachyonExcelDataReaderHelper = tachyonExcelDataReaderHelper;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _shippingRequestTripManager = shippingRequestTripManager;
        }

        public List<ImportRoutePointDto> GetPointsFromExcel(byte[] fileBytes, long shippingRequestId)
        {
            ShippingRequestId = shippingRequestId;
            return ProcessExcelFile(fileBytes, ProcessPointsExcelRow);
        }

        private ImportRoutePointDto ProcessPointsExcelRow(ISheet worksheet, int row)
        {
            if (_tachyonExcelDataReaderHelper.IsRowEmpty(worksheet, row))
            {
                return null;
            }
            StringBuilder exceptionMessage = new StringBuilder();
            ImportRoutePointDto point = new ImportRoutePointDto();
            try
            {
                var tripReference = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                    row, 0, "Trip Reference*", exceptionMessage);
                point.TripReference = tripReference;
                //var tripId = GetShippingRequestTripIdByBulkRef(tripReference, exceptionMessage);
                //if(tripId!=null)
                //{
                //    point.ShippingRequestTripId = tripId.Value;
                //}

                //0
                point.BulkUploadReference = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                    row, 1, "Point Reference*", exceptionMessage);

                var pickingType = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                   row, 2, "Point Type*", exceptionMessage);
                point.PickingTypeDisplayName = pickingType;
                point.PickingType = GetPickingTypeEnumFromString(pickingType, exceptionMessage).Value;

                var facility = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                    row, 3, "Facility*", exceptionMessage);
                point.Facility = facility;
                var facilityId = GetFacilityId(facility, exceptionMessage);
                if (facilityId != null)
                {
                    point.FacilityId = facilityId.Value;
                }

                var receiver = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                row, 4, "Agent*", exceptionMessage);
                point.Receiver = receiver;
                var receiverId = GetReceiverId(receiver, exceptionMessage,facilityId.Value);
                if (receiverId != null)
                {
                    point.ReceiverId = receiverId;
                }

                //_shippingRequestTripManager.ValidateTripDto(trip, exceptionMessage);

                if (exceptionMessage.Length > 0)
                {
                    point.Exception = exceptionMessage.ToString();
                }
            }
            catch (Exception exception)
            {
                point.Exception = exception.Message;
            }

            return point;
        }

        private int? GetShippingRequestTripIdByBulkRef(string tripReference, StringBuilder exceptionMessage)
        {
            try
            {
               return _shippingRequestTripRepository.FirstOrDefault(x => x.BulkUploadRef == tripReference).Id;
            }
            catch
            {
                exceptionMessage.Append(_tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("Trip"));
                return null;
            }
        }

        private long? GetFacilityId(string text, StringBuilder exceptionMessage)
        {
            if (text.IsNullOrEmpty())
            {
                exceptionMessage.Append(
                    _tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("Facility"));
                return null;
            }

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

            var receiver = _shippingRequestTripManager.GetReceiverByPermissionAndFacility(text, ShippingRequestId, facilityId);
                //_receiverRepository.FirstOrDefault(x => x.FullName == text);
            if (receiver != null)
                return receiver.Id;

            exceptionMessage.Append(_tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("Receiver"));
            return null;
        }

        private PickingType? GetPickingTypeEnumFromString(string text, StringBuilder exceptionMessage)
        {
            try
            {
                return (PickingType)Enum.Parse(typeof(PickingType), text);
            }
            catch
            {
                exceptionMessage.Append(_tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("PointType"));
                return null;
            }
        }

    }
}
