﻿using Abp;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Trips.Importing.Dto
{
    public class ImportShipmentFromExcelInput
    {
        public int? TenantId { get; set; }
        public Guid BinaryObjectId { get; set; }
        public long ShippingRequestId { get; set; }
    }
}