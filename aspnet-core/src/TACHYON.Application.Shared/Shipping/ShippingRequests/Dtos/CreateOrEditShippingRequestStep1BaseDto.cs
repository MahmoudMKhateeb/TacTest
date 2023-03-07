﻿using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public abstract class CreateOrEditShippingRequestStep1BaseDto : EntityDto<long?>, IShouldNormalize
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
        public virtual  bool IsTachyonDeal { get; set; }
        public bool IsDirectRequest { get; set; }

        /// <summary>
        /// this field describes carrier that will send direct request for, when the request isDirectRequest
        /// </summary>
        public int? CarrierTenantIdForDirectRequest { get; set; }

        [Required] public ShippingTypeEnum ShippingTypeId { get; set; }
        public RoundTripType? RoundTripType { get; set; }

        public bool IsDrafted { get; set; }
        public int DraftStep { get; set; }
        
        [JsonIgnore] public ShippingRequestType RequestType { get; set; }
        public int? ShipperActorId { get; set; }
        public int? CarrierActorId { get; set; }

        public bool IsInternalBrokerRequest { get; set; }

        public void Normalize()
        {
            if (IsInternalBrokerRequest)
            {
                IsTachyonDeal = false;
                IsBid = false;
                IsDirectRequest = true;
                RequestType = ShippingRequestType.DirectRequest;
            }

            else if (IsBid)
            {
                RequestType = ShippingRequestType.Marketplace;
                IsTachyonDeal = false;
                IsInternalBrokerRequest = false;
                IsDirectRequest = false;
            }
            else if (IsTachyonDeal)
            {
                RequestType = ShippingRequestType.TachyonManageService;
                IsBid = false;
                IsInternalBrokerRequest = false;
                IsDirectRequest = false;
            }
            else
            {
                RequestType = ShippingRequestType.DirectRequest;
                IsTachyonDeal = false;
                IsBid = false;
                IsDirectRequest = true;
            }

            // to get date only without time
            BidStartDate = BidStartDate?.Date;
        }
    }
}
