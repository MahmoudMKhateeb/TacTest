using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Drivers.Dto
{
    public class UserOtpDto : EntityDto
    {
        public string OTP { get; set; }
    }
}