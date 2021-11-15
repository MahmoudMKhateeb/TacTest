using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Tracking.Dto
{
    public class InvokeStatusInputDto : EntityDto<long>
    {
        [Required]
        public string Action { get; set; }
        public string Code { get; set; }

    }
}