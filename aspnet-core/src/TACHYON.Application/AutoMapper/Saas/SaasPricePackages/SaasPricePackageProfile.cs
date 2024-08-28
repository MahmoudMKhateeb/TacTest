using AutoMapper;
using TACHYON.Saas.SaasPricePackages;
using TACHYON.Saas.SaasPricePackages.Dto;

namespace TACHYON.AutoMapper.Saas.SaasPricePackages
{

public class SaasPricePackageProfile : Profile
{
    public  SaasPricePackageProfile ()
    {
        CreateMap<SaasPricePackage, SaasPricePackageListDto>()
            .ReverseMap();
        CreateMap<SaasPricePackage, CreateOrEditSaasPricePackageDto>()
            .ReverseMap();
    }
}
}