using System.Collections.Generic;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.Dto;

namespace TACHYON.Goods.GoodCategories.Exporting
{
    public interface IGoodCategoriesExcelExporter
    {
        FileDto ExportToFile(List<GetGoodCategoryForViewDto> goodCategories);
    }
}