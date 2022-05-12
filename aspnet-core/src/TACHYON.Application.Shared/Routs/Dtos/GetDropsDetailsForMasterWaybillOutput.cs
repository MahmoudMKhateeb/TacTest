using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Routs.Dtos
{
    public class GetDropsDetailsForMasterWaybillOutput
    {
        public string Code { get; set; }
        public string ReceiverDisplayName { get; set; }
    }
}