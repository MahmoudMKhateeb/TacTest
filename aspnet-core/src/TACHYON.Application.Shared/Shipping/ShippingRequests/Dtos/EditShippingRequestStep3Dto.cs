﻿using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class EditShippingRequestStep3Dto :EntityDto<long>
    {
        [Required]
        public int GoodCategoryId { get; set; }
        public double TotalWeight { get; set; }
        public int PackingTypeId { get; set; }
        public int NumberOfPacking { get; set; }
        public virtual int? TransportTypeId { get; set; }
        public virtual long TrucksTypeId { get; set; }
        public virtual int? CapacityId { get; set; }

    }
}