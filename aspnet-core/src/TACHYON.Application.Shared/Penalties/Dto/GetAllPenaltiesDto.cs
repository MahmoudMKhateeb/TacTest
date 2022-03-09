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
        public decimal Amount { get; set; }
        public string CompanyName { get; set; }
    }
}
