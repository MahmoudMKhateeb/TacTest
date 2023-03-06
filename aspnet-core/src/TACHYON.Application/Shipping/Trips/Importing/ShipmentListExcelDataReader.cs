using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Threading;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.AddressBook;
using TACHYON.Authorization.Users;
using TACHYON.DataExporting.Excel;
using TACHYON.DataExporting.Excel.NPOI;
using TACHYON.Receivers;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.Shipping.Trips.Importing.Dto;

namespace TACHYON.Shipping.Trips.Importing
{
    public class ShipmentListExcelDataReader : NpoiExcelImporterBase<ImportTripDto>, IShipmentListExcelDataReader
    {
        private readonly TachyonExcelDataReaderHelper _tachyonExcelDataReaderHelper;
        private readonly ShippingRequestTripManager _shippingRequestTripManager;
        private readonly UserManager _userManager;

        private long ShippingRequestId;
        private bool IsSingleDropRequest;
        private bool IsDedicatedRequest;

        public ShipmentListExcelDataReader(TachyonExcelDataReaderHelper tachyonExcelDataReaderHelper, ShippingRequestTripManager shippingRequestTripManager, UserManager userManager)
        {
            _tachyonExcelDataReaderHelper = tachyonExcelDataReaderHelper;
            _shippingRequestTripManager = shippingRequestTripManager;
            _userManager = userManager;
        }

        public List<ImportTripDto> GetShipmentsFromExcel(byte[] fileBytes, long shippingRequestId, bool isSingleDropRequest, bool isDedicatedRequest)
        {
            ShippingRequestId = shippingRequestId;
            IsSingleDropRequest = isSingleDropRequest;
            IsDedicatedRequest = isDedicatedRequest;
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
                var SR = _shippingRequestTripManager.GetShippingRequestById(trip.ShippingRequestId.Value);

                //0
                trip.BulkUploadRef = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                    row, 0, "Reference No", exceptionMessage);
                //1
                var startDate = _tachyonExcelDataReaderHelper.GetValueFromRowOrNull<string>(worksheet,
                    row, 1, "Trip Pick up Date Start *", exceptionMessage);
                var startTripDate = GetDateTimeValueFromTextOrNull(startDate);
                if(startTripDate == null)
                {
                    startTripDate = !IsDedicatedRequest ?  SR.StartTripDate : SR.RentalStartDate;
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

                if(endDate!=null && (endTripDate == null || endTripDate == default(DateTime)))
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
                trip.NeedsDeliveryNote= GetBoolValueFromYesOrNo(_tachyonExcelDataReaderHelper.GetValueFromRowOrNull<string>(worksheet,
                    row, 5, "Needs Delivery Note ?*", exceptionMessage));
                //6
                trip.HasAttachment = GetBoolValueFromYesOrNo(_tachyonExcelDataReaderHelper.GetValueFromRowOrNull<string>(worksheet,
                    row, 6, "Has Attachment ?*", exceptionMessage));
                if (IsDedicatedRequest)
                {
                    var routeType = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                        row, 7, "Route type*", exceptionMessage);
                    trip.RouteTypeTitle = routeType;
                    //remove all spaces
                    routeType = routeType.Replace(" ", "");
                    ShippingRequestRouteType routTypeId = default;
                    try
                    {
                         routTypeId = (ShippingRequestRouteType)Enum.Parse(typeof(ShippingRequestRouteType), routeType);
                    }
                    catch
                    {
                        exceptionMessage.Append(
                    _tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("RouteType"));
                    }
                    trip.RouteType = routTypeId;

                    var numberOfDrops = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                        row, 8, "Number of drops*", exceptionMessage);
                    var drops = getNumberOfDropsFromString(numberOfDrops, exceptionMessage);
                    if (drops != null)
                        trip.NumberOfDrops = drops.Value;

                    var originFacility = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                        row, 9, "Original Facility*", exceptionMessage);
                    trip.OriginalFacility = originFacility;
                    trip.OriginFacilityId = GetFacilityId(originFacility, exceptionMessage);

                    var destinationFacility = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                       row, 10, "Destination Facility*", exceptionMessage);
                    trip.DestinationFacility = destinationFacility;
                    trip.DestinationFacilityId = GetFacilityId(destinationFacility, exceptionMessage);

                    if (routTypeId == ShippingRequestRouteType.SingleDrop)
                    {
                        var sender = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                        row, 11, "Sender", exceptionMessage);
                        trip.sender = sender.Trim();
                        if (trip.OriginFacilityId != null)
                        {
                            trip.SenderId = GetReceiverId(sender, exceptionMessage, trip.OriginFacilityId.Value);
                        }

                        var receiver = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                        row, 12, "Receiver", exceptionMessage);
                        trip.Receiver = receiver.Trim();
                        if (trip.DestinationFacilityId != null)
                        {
                            trip.ReceiverId = GetReceiverId(receiver, exceptionMessage, trip.DestinationFacilityId.Value);
                        }
                    }

                    //driver
                    var driver = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                        row, 13, "Driver*", exceptionMessage);
                    trip.Driver = driver.Trim();
                    var driverId = GetDriverId(driver, SR.CarrierTenantId, exceptionMessage);
                    if(driverId !=null)
                        trip.DriverUserId = driverId;

                    //truck
                    var truck = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                       row, 14, "Truck*", exceptionMessage);
                    trip.Truck = truck.Trim();
                    var truckId = GetTruckId(truck, SR.CarrierTenantId, exceptionMessage);
                    if (truckId != null)
                        trip.TruckId = truckId;
                }
                else
                {
                    var originFacility = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                        row, 7, "Original Facility*", exceptionMessage);
                    trip.OriginalFacility = originFacility;
                    trip.OriginFacilityId = GetFacilityId(originFacility, exceptionMessage);

                    var destinationFacility = _tachyonExcelDataReaderHelper.GetRequiredValueFromRowOrNull<string>(worksheet,
                       row, 8, "Destination Facility*", exceptionMessage);
                    trip.DestinationFacility = destinationFacility;
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

        private long? GetDriverId (string text, int? tenantId, StringBuilder exceptionMessage)
        {
            try
            {
                return _shippingRequestTripManager.GeDriverIdByPermission(text, ShippingRequestId, tenantId.Value);
            }
            catch
            {
                exceptionMessage.Append(_tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("DriverIsInvalidOrNotFromAssigned"));
                return null;
            }
        }

        private long? GetTruckId(string text, int? tenantId, StringBuilder exceptionMessage)
        {
            try
            {
                return _shippingRequestTripManager.GetTruckIdByPermission(text, ShippingRequestId, tenantId.Value);
               
            }
            catch
            {
                exceptionMessage.Append(_tachyonExcelDataReaderHelper.GetLocalizedMessagePart("TruckIsInvalidOrNotFromAssigned"));
                return null;
            }
        }

        private int? getNumberOfDropsFromString(string numberOfDrops, StringBuilder exceptionMessage)
        {
            try
            {
                return Convert.ToInt16(numberOfDrops);
            }
            catch
            {
                exceptionMessage.Append(_tachyonExcelDataReaderHelper.GetLocalizedExceptionMessagePart("NumberOfDrops"));
                return null;
            }
        }
    }
}
