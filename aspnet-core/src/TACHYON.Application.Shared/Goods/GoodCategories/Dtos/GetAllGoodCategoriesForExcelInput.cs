using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Goods.GoodCategories.Dtos
{
    public class GetAllGoodCategoriesForExcelInput
    {
        public string Filter { get; set; }

        public string DisplayNameFilter { get; set; }
    }
}