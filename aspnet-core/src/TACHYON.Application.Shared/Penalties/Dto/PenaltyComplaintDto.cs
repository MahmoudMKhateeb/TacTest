using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Penalties.Dto
{
    public class PenaltyComplaintDto : EntityDto
    {
        public string RejectReason{ get; set; }
        public string Description { get; set; }
        public ComplaintStatus Status { get; set; }
    }
}
