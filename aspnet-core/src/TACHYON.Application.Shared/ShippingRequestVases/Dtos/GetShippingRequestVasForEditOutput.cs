using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.ShippingRequestVases.Dtos
{
    public class GetShippingRequestVasForEditOutput
    {
        public CreateOrEditShippingRequestVasDto ShippingRequestVas { get; set; }

        public string VasName { get; set; }

    }
}