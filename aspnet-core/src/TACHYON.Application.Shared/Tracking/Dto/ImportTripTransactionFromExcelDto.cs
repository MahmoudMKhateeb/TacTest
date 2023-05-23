using Abp;
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
        /// <summary>
        /// These dates field is to show in FrontEnd UI only
        /// </summary>
        public string StartMovingToLoadingLocation { get; set; }
        public string ArriveToLoadingLocation {get; set;}
        public string StartLoading { get; set; }
        public string FinishLoading { get; set; }
        public string StartMovingToOffloadingLocation { get; set;}
        public string ArriveToOffloadingLocation { get; set; }
        public string StartOffloading { get; set; }
        public string FinishOffLoading { get; set;}
        public string RecieverConfirmed { get; set; }
    }
}