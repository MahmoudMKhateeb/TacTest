using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Documents.DocumentFiles.Dtos;

namespace TACHYON.Shipping.Notes.Dto
{
    public class GetAllShippingRequestAndTripNotesDto
    {
        public PagedResultDto<ShippingRequestAndTripNotesDto> Data { get; set; }
       
    }
}