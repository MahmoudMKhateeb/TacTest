
using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Goods.GoodCategories.Dtos
{
    public class GoodCategoryDto : EntityDto
    {
        public string DisplayName { get; set; }

        public int? FatherId { get; set; }
        public bool IsActive { get; set; }
    }
}