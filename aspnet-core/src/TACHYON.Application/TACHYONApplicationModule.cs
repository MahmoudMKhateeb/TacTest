using Abp.AutoMapper;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Resources.Embedded;
using AutoMapper;
using System.Linq;
using System.Reflection;
using TACHYON.Authorization;
using TACHYON.Authorization.Permissions;
using TACHYON.Authorization.Permissions.Shipping.Trips;
using TACHYON.Exporting;

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
            //Add all permission provider inherit from base class AppAuthorizationBaseProvider
            foreach (var provider in typeof(AppAuthorizationBaseProvider).Assembly.GetTypes().Where(t => t.BaseType == typeof(AppAuthorizationBaseProvider)))
            {
                Configuration.Authorization.Providers.Add(provider);
            }

            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);

            //Adding multiLingual Mapping configuration 
            Configuration.Modules.AbpAutoMapper().Configurators.Add(configuration =>
            {
                configuration.AddMaps(Assembly.GetExecutingAssembly());
                CustomDtoMapper.CreateMultiLingualMappings(configuration, new MultiLingualMapContext(
                    IocManager.Resolve<ISettingManager>()
                ));
            });

            Configuration.EmbeddedResources.Sources.Add(
                new EmbeddedResourceSet(
                    "/Reports/",
                    Assembly.GetExecutingAssembly(),
                    "TACHYON.Waybills.Reports"
                    )
                );

            Configuration.EmbeddedResources.Sources.Add(
                new EmbeddedResourceSet(
                    "/Reports/",
                    Assembly.GetExecutingAssembly(),
                    "TACHYON.Invoices.Reports"
                )
            );
            IocManager.Register(typeof(IExcelExporterManager<>), typeof(ExcelExporterManager<>), DependencyLifeStyle.Transient);
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