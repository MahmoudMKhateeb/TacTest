using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Vases.Dtos
{
    public class ShippingRequestVasListDto : EntityDto
    {
        public string VasName { get; set; }

        public bool HasAmount { get; set; }

        public bool HasCount { get; set; }
        public int MaxAmount { get; set; }

        public int MaxCount { get; set; }

    }
}
