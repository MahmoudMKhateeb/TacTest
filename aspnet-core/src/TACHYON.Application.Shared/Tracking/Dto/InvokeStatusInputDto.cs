using Abp;
using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Tracking.Dto
{
    public class InvokeStatusInputDto : EntityDto<long>, IShouldInitialize
    {
        [Required]
        public string Action { get; set; }

        public string Code { get; set; }

        public void Initialize()
        {
            if (Action == WorkFlowActionConst.ReceiverConfirmed || Action == WorkFlowActionConst.DeliveryConfirmationReceiverConfirmed)
            {
                if (Code.IsNullOrEmpty())
                {
                    throw new AbpValidationException("code is required");
                }
            }
        }
    }
}