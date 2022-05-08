using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Authorization.Users.Dto
{
    public class OtpCreatedDto
    {
        public double TotalSeconds { get; set; }

        public OtpCreatedDto(double totalSeconds)
        {
            TotalSeconds = totalSeconds;
        }
    }
}