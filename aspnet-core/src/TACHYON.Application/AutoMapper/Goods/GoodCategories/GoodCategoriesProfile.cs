using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Goods.GoodCategories;
using TACHYON.Goods.GoodCategories.Dtos;

namespace TACHYON.AutoMapper.Goods.GoodCategories
{
    public class GoodCategoriesProfile : Profile
    {
        public GoodCategoriesProfile()
        {
            CreateMap<CreateOrEditGoodCategoryDto, GoodCategory>()
                    .ForMember(dest => dest.Translations, opt => opt.MapFrom(src => src.Translations))
                    .ReverseMap();
            CreateMap<GoodCategoryTranslationDto, GoodCategoryTranslation>().ReverseMap();

        }
    }
}
