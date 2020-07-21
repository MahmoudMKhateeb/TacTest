using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Goods.GoodsDetails.Dtos
{
    public class GetGoodsDetailForEditOutput
    {
		public CreateOrEditGoodsDetailDto GoodsDetail { get; set; }

		public string GoodCategoryDisplayName { get; set;}


    }
}