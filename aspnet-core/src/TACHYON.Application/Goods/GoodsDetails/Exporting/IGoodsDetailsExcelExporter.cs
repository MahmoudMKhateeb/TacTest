using System.Collections.Generic;
using TACHYON.Dto;
using TACHYON.Goods.GoodsDetails.Dtos;

namespace TACHYON.Goods.GoodsDetails.Exporting
{
    public interface IGoodsDetailsExcelExporter
    {
        FileDto ExportToFile(List<GetGoodsDetailForViewDto> goodsDetails);
    }
}