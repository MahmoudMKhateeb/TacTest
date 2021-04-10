using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Invoices.Balances.Dto
{

    public  class CreateBalanceRechargeInput
    {
        [Required]
        public int TenantId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string ReferenceNo { get; set; }
    }
}
