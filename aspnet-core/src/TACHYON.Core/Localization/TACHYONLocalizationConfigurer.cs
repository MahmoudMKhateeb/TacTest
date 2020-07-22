using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;
using System.Reflection;

namespace TACHYON.Localization
{
    public static class TACHYONLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    TACHYONConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(TACHYONLocalizationConfigurer).GetAssembly(),
                        "TACHYON.Localization.TACHYON"
                    )
                )
            );
        }
    }
}