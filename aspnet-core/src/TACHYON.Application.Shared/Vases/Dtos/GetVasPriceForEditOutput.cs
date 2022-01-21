using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Vases.Dtos
{
    public class GetVasPriceForEditOutput
    {
        public CreateOrEditVasPriceDto VasPrice { get; set; }

        public string VasName { get; set; }
    }
}