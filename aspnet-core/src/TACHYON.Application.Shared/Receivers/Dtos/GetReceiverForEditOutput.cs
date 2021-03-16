using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Receivers.Dtos
{
    public class GetReceiverForEditOutput
    {
        public CreateOrEditReceiverDto Receiver { get; set; }

        public string FacilityName { get; set; }

    }
}