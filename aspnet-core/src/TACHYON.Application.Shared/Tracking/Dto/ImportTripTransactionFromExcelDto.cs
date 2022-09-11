using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace TACHYON.Tracking.Dto
{
    public class ImportTripTransactionFromExcelDto
    {
        public List<DateTime> TransactionsDates { get; set; }

        public long WaybillNumber { get; set; }
        public string Exception { get; set; }
    }
}