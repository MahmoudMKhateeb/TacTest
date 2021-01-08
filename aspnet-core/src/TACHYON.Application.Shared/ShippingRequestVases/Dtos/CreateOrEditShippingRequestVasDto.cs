using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.ShippingRequestVases.Dtos
{
    public class CreateOrEditShippingRequestVasDto : EntityDto<long?>
    {

        public int VasId { get; set; }

    }
}