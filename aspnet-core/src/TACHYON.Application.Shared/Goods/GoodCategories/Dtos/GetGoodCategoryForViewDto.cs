namespace TACHYON.Goods.GoodCategories.Dtos
{
    public class GetGoodCategoryForViewDto
    {
        public GoodCategoryDto GoodCategory { get; set; }

        public string FatherCategoryName { get; set; }
    }
}