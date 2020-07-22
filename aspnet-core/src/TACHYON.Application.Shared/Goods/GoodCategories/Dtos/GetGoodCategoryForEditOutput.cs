using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Goods.GoodCategories.Dtos
{
    public class GetGoodCategoryForEditOutput
    {
        public CreateOrEditGoodCategoryDto GoodCategory { get; set; }


    }
}