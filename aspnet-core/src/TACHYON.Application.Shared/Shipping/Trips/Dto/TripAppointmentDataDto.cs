﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TACHYON.Commission;
using TACHYON.PriceOffers;

namespace TACHYON.Shipping.Trips.Dto
{
    public class TripAppointmentDataDto: PriceCommissionDtoBase
    {
        public long? ShippingRequestId { get; set; }
        [Required]
        public decimal ItemPrice { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal VatAmount { get; set; }


        public decimal TotalAmountWithCommission { get; set; }
        public decimal SubTotalAmountWithCommission { get; set; }
        public decimal VatAmountWithCommission { get; set; }

        public decimal CommissionAmount { get; set; }
        public PriceOfferCommissionType CommissionType { get; set; }
        public decimal CommissionPercentageOrAddValue { get; set; }

        public DateTime? AppointmentDateTime { get; set; }
        public string AppointmentNumber { get; set; }
        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }
    }
}