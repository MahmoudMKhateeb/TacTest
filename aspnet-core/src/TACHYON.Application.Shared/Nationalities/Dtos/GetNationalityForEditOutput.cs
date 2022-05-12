using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Nationalities.Dtos
{
    public class GetNationalityForEditOutput
    {
        public CreateOrEditNationalityDto Nationality { get; set; }
    }
}