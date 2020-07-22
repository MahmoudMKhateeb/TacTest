using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Countries.Dtos
{
    public class GetCountyForEditOutput
    {
        public CreateOrEditCountyDto County { get; set; }


    }
}