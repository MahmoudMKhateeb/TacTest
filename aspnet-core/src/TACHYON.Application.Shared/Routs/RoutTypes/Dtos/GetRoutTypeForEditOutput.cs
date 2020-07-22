using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Routs.RoutTypes.Dtos
{
    public class GetRoutTypeForEditOutput
    {
        public CreateOrEditRoutTypeDto RoutType { get; set; }


    }
}