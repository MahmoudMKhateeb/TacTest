﻿using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Goods.GoodCategories.Dtos
{
    public class GetAllGoodCategoriesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string DisplayNameFilter { get; set; }



    }
}