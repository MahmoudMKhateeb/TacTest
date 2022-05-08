using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using Abp.Timing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class CreateOrEditShippingRequestStep1Dto : EntityDto<long?>, ICustomValidate
    {
        /// <summary>
        /// TAC-1937
        /// Tachyon Create Services Can select shipper
        /// </summary>
        public int? ShipperId { get; set; }

        public virtual bool IsBid { get; set; }

        //Add Bid details If IsBid equals True
        public DateTime? BidStartDate { get; set; }
        public DateTime? BidEndDate { get; set; }
        public virtual bool IsTachyonDeal { get; set; }
        public bool IsDirectRequest { get; set; }

        /// <summary>
        /// this field describes carrier that will send direct request for, when the request isDirectRequest
        /// </summary>
        public int? CarrierTenantIdForDirectRequest { get; set; }

        [Required] public int ShippingTypeId { get; set; }
        [Required] public DateTime StartTripDate { get; set; }
        public DateTime? EndTripDate { get; set; }
        public bool IsDrafted { get; set; }
        public int DraftStep { get; set; }
        public string ShipperReference { get; set; }
        public string ShipperInvoiceNo { get; set; }

        [JsonIgnore] public ShippingRequestType RequestType { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (this.StartTripDate.Date < Clock.Now.Date)
            {
                context.Results.Add(new ValidationResult("Start trip date cannot be before today"));
            }

            if (EndTripDate.HasValue && StartTripDate.Date > EndTripDate.Value.Date)
            {
                context.Results.Add(new ValidationResult("The start date must be or equal to end date."));
            }

            if (IsBid)
            {
                RequestType = ShippingRequestType.Marketplace;
            }
            else if (IsTachyonDeal)
            {
                RequestType = ShippingRequestType.TachyonManageService;
            }
            else
            {
                RequestType = ShippingRequestType.DirectRequest;
            }

            if (IsDirectRequest && CarrierTenantIdForDirectRequest == null)
            {
                context.Results.Add(new ValidationResult("You must choose one carrier to send direct request"));
            }
        }
    }
}