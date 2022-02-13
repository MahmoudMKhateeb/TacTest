using Abp.Extensions;
using Abp.Localization;
using AutoMapper;
using TACHYON.EntityTemplates;

namespace TACHYON.AutoMapper.EntityTemplates
{
    public class EntityTemplateProfile : Profile
    {
        public EntityTemplateProfile()
        {
            CreateMap<CreateOrEditEntityTemplateInputDto, EntityTemplate>();
            CreateMap<EntityTemplate, EntityTemplateListDto>()
                .ForMember(x=> x.EntityTypeTitle,x=>
                    x.MapFrom(i=> i.EntityType.GetEnumDescription() ?? i.EntityType.ToString()));
            CreateMap<EntityTemplate, EntityTemplateForViewDto>()
                .ForMember(x=> x.EntityTypeTitle,x=>
                    x.MapFrom(i=> i.EntityType.GetEnumDescription() ?? i.EntityType.ToString()));
        }
    }
}