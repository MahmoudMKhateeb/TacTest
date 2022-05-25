using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Penalties.Dto
{
   public class RejectComplaintDto : EntityDto
    {
        public string RejectReason { get; set; }
    }
}
