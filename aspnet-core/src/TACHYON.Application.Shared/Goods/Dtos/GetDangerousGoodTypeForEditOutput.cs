using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Goods.Dtos
{
    public class GetDangerousGoodTypeForEditOutput
    {
        public CreateOrEditDangerousGoodTypeDto DangerousGoodType { get; set; }
    }
}