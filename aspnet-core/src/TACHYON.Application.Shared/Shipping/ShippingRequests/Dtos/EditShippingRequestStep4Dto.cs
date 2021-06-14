using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Vases.Dtos;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class EditShippingRequestStep4Dto : EntityDto<long>
    {
        public List<CreateOrEditShippingRequestVasListDto> ShippingRequestVasList { get; set; }
    }
}
