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
                .ForMember(x=> x.EntityType,x=>
                    x.MapFrom(i=> i.EntityType.GetEnumDescription() ?? i.EntityType.ToString()));
            CreateMap<EntityTemplate, EntityTemplateForViewDto>()
                .ForMember(x=> x.Type,x=>
                    x.MapFrom(i=> i.EntityType.GetEnumDescription() ?? i.EntityType.ToString()));
        }
    }
}