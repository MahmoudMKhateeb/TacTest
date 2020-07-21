
using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Goods.GoodsDetails.Dtos
{
    public class GoodsDetailDto : EntityDto<long>
    {
		public string Name { get; set; }

		public string Description { get; set; }

		public string Quantity { get; set; }

		public string Weight { get; set; }

		public string Dimentions { get; set; }

		public bool IsDangerousGood { get; set; }

		public string DangerousGoodsCode { get; set; }


		 public int? GoodCategoryId { get; set; }

		 
    }
}