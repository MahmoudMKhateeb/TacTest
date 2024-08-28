﻿using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.ShippingRequestTripVases.Dtos;

namespace TACHYON.Shipping.Trips.Dto
{
    public class ShippingRequestsTripListDto : EntityDto<int>
    {
        public string BulkUploadRef { get; set; }
        public DateTime StartTripDate { get; set; }
        public DateTime? EndTripDate { get; set; }
        public DateTime? StartWorking { get; set; }
        public DateTime? EndWorking { get; set; }
        public ShippingRequestTripStatus Status { get; set; }
        public bool HasAttachment { get; set; }
        public bool NeedsDeliveryNote { get; set; }

        public string StatusTitle
        {
            get
            {
                return Enum.GetName(typeof(ShippingRequestTripStatus), Status);
            }
        }

        public string Driver { get; set; }
        public string ReplacedDriver { get; set; }
        public string Truck { get; set; }
        public string OriginCity { get; set; }
        public string DestinationCity { get; set; }
        public bool HasAccident { get; set; }
        public bool IsApproveCancledByShipper { get; set; }
        public bool IsApproveCancledByCarrier { get; set; }
        public bool IsApproveCancledByTachyonDealer { get; set; }
        public string CanceledReason { get; set; }
        public ShippingRequestTripCancelStatus CancelStatus { get; set; }
        public string RejectedCancelingReason { get; set; }
        public ShippingRequestTripDriverStatus DriverStatus { get; set; }
        public string DriverStatusTitle { get; set; }
        public string RejectedReason { get; set; }
        public long? WaybillNumber { get; set; }
        public bool IsTripRateBefore { get; set; }
        public bool CanCreateTemplate { get; set; }

        public DateTime? SupposedPickupDateFrom { get; set; }
        public DateTime? SupposedPickupDateTo { get; set; }
        public int NotesCount { get; set; }

        public string BayanId { get; set; }
        public bool CanAssignTrucksAndDrivers { get; set; }
        public ShippingRequestTripFlag ShippingRequestTripFlag { get; set; }

        public string SabOrderId { get; set; }
        public DateTime? ContainerReturnDate { get; set; }
        public bool? IsContainerReturned {get;set;}

        /// <summary>
        /// This reference shipper add it manually
        /// </summary>
        public string ShipperReference { get; set; }

        /// <summary>
        /// shipper add his invoice number manually, this updated currently to booking number 
        /// </summary>
        public string ShipperInvoiceNo { get; set; }
    }
}