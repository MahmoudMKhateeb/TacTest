using Abp;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Tracking.Dto;

namespace TACHYON.Shipping.Trips.Importing.Dto
{
    public class ForceDeliverTripJobArgs
    {
        public List<ImportTripTransactionFromExcelDto> importedTripDeliveryDetails { get; set; }
        public UserIdentifier userIdentifier { get; set; }

    }
}
