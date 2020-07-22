using System.Collections.Generic;
using TACHYON.Dto;
using TACHYON.Goods.GoodCategories.Dtos;

namespace TACHYON.Goods.GoodCategories.Exporting
{
    public interface IGoodCategoriesExcelExporter
    {
        FileDto ExportToFile(List<GetGoodCategoryForViewDto> goodCategories);
    }
}