﻿using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Shipping.Trips.Accidents.Dto
{
    public  class ShippingRequestTripAccidentListDto: EntityDto
    {
        public long PointId { get; set; }
        public string PickingType { get; set; }
        public string Address { get; set; }
        public string Reason { get; set; }
        public string Description { get; set; }
        public bool IsResolve { get; set; }
        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }
    }
}