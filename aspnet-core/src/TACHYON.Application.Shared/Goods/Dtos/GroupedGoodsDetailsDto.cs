using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Shipping.Trips.Importing.Dto;

namespace TACHYON.Goods.Dtos
{
    public class GroupedGoodsDetailsDto
    {
        public string tripRef { get; set; }
        public List<ICreateOrEditGoodsDetailDtoBase> importGoodsDetailsDtoList { get; set; }
    }
}
