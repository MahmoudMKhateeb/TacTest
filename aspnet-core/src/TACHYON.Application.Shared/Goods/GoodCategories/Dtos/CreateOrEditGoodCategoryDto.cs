﻿
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Goods.GoodCategories.Dtos
{
    public class CreateOrEditGoodCategoryDto : EntityDto<int?>
    {

		[Required]
		[StringLength(GoodCategoryConsts.MaxDisplayNameLength, MinimumLength = GoodCategoryConsts.MinDisplayNameLength)]
		public string DisplayName { get; set; }
		
		

    }
}