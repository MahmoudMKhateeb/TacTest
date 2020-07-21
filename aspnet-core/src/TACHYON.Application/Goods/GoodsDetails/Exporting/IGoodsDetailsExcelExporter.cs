using System.Collections.Generic;
using TACHYON.Goods.GoodsDetails.Dtos;
using TACHYON.Dto;

namespace TACHYON.Goods.GoodsDetails.Exporting
{
    public interface IGoodsDetailsExcelExporter
    {
        FileDto ExportToFile(List<GetGoodsDetailForViewDto> goodsDetails);
    }
}