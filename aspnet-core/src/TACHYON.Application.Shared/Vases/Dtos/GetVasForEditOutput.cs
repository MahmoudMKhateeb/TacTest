using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Vases.Dtos
{
    public class GetVasForEditOutput
    {
        public CreateOrEditVasDto Vas { get; set; }

    }
}