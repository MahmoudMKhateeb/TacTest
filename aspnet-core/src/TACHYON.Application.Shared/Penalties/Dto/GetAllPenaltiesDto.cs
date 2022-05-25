using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Penalties.Dto
{
   public class GetAllPenaltiesDto : EntityDto
    {
        public string PenaltyName { get; set; }
        public string PenaltyDescrption { get; set; }
        public decimal TotalAmount { get; set; }
        public long? WaybillNumber { get; set; }
        public string CompanyName { get; set; }
        public string DestinationCompanyName { get; set; }
        public PenaltyStatus Status { get; set; }
        public PenaltyType Type { get; set; }
        public int? PenaltyComplaintId { get; set; }
        public int TenantId { get; set; }
        public int DestinationTenantId { get; set; }
        public string PenaltyTypeTitle { get { return Type.GetEnumDescription(); } }
        public string PenaltyStatusTitle { get { return Status.GetEnumDescription(); } }
    }
}
