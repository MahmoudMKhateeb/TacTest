using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Invoices.Groups.Dto
{
  public  class GroupPeriodDemandCreateInput:Entity<long>
    {
        [Required]
        public string DocumentBase64 { get; set; }

    }
}
