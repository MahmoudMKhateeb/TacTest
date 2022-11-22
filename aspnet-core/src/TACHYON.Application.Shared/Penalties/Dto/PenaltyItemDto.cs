using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Penalties.Dto
{
    public class PenaltyItemDto : EntityDto<int?>
    {
        public int? ShippingRequestTripId { get; set; }
        #region Prices
        public decimal ItemPrice { get; set; }
        public decimal ItemTotalAmountPostVat { get; set; }
        public decimal VatAmount { get; set; }
        /// <summary>
        /// Helper for angular to view waybill, return when get for edit
        /// </summary>
        public string WaybillNumber { get; set; }

        #endregion
    }
}
