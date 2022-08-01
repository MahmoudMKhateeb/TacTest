using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Penalties.Dto
{
   public class RegisterPenaltyComplaintDto : EntityDto<int?>
    {
        public int PenaltyId { get; set; }
        public string Description { get; set; }

    }
}
