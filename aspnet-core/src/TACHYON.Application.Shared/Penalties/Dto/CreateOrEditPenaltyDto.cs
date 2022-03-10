using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Penalties.Dto
{
    public class CreateOrEditPenaltyDto : EntityDto<long?>
    {
        [Required]
        public string PenaltyName { get; set; }
        public string PenaltyDescrption { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public int TenantId { get; set; }
        public PenaltyType Type { get; set; }

    }
}
