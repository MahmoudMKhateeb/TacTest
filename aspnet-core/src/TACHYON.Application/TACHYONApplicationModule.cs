using Abp.AutoMapper;
using Abp.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AutoMapper;
using TACHYON.Authorization;

namespace TACHYON
{
    /// <summary>
    /// Application layer module of the application.
    /// </summary>
    [DependsOn(
        typeof(TACHYONApplicationSharedModule),
        typeof(TACHYONCoreModule),
         typeof(AbpAutoMapperModule)
        )]
    public class TACHYONApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Adding authorization providers
            Configuration.Authorization.Providers.Add<AppAuthorizationProvider>();

            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);

            //Adding multiLingual Mapping configuration 
            Configuration.Modules.AbpAutoMapper().Configurators.Add(configuration =>
            {
                CustomDtoMapper.CreateMultiLingualMappings(configuration, new MultiLingualMapContext(
                    IocManager.Resolve<ISettingManager>()
                ));
            });

        }
        public override void PostInitialize()
        {

            var mapper = IocManager.Resolve<IMapper>();

            CustomDtoMapper.SetMapper(mapper);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(TACHYONApplicationModule).GetAssembly());
        }
    }
}