using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TACHYON.Localization
{
    public class AppLocalizationJob : BackgroundJob<string>, ITransientDependency
    {
        private readonly IRepository<AppLocalization> _appLocalizationRepository;
        private readonly IRepository<ApplicationLanguage> _languageRepository;
        private string directory;
        public AppLocalizationJob(IRepository<AppLocalization> appLocalizationRepository, IRepository<ApplicationLanguage> languageRepository)
        {
            _appLocalizationRepository = appLocalizationRepository;
            _languageRepository = languageRepository;
        }

        [UnitOfWork]
        public override void Execute(string path)
        {
            directory = path;
            Restore();
            Logger.Debug(path.ToString());
        }

        private void Restore()
        {
            var CurrentMaster = _appLocalizationRepository.GetAll().ToList();
            var master = LoadKeys($"{directory}/TACHYON.xml", "en").Select(x => new AppLocalization { MasterKey = x.Key, MasterValue = x.Value }).ToList();
            

            var data = master.Where(m => !CurrentMaster.Any(c => c.MasterKey.ToLower() == m.MasterKey.ToLower())).ToList();
            if (data == null || data.Count() == 0) return;
            List<TranslationDto> Translations = RestoreTranslations();


            foreach (var lang in data)
            {
                if (!_appLocalizationRepository.GetAll().Any(x => x.MasterKey.ToLower() == lang.MasterKey.ToLower()))
                {
                    lang.Translations = Translations.Where(x => x.Key.ToLower() == lang.MasterKey.ToLower()).Select(x => new AppLocalizationTranslation() { Language = x.Language, Value = x.Value }).ToList();
                    _appLocalizationRepository.Insert(lang);
                }
            }
        }
        private List<TranslationDto> RestoreTranslations()
        {
            List<TranslationDto> Translations = new List<TranslationDto>();


            foreach (var lang in _languageRepository.GetAll().Where(x => !x.IsDisabled && x.Name != "en"))
            {
                string FilePath = $"{directory}/TACHYON-{lang.Name}.xml";
                if (!File.Exists(FilePath))
                {
                    FilePath = $"{directory}/TACHYON-{lang.Name.Split("-")[0]}.xml";
                    if (!File.Exists(FilePath)) FilePath = "";
                }
                if (!string.IsNullOrEmpty(FilePath))
                {
                    Translations.AddRange(LoadKeys(FilePath, lang.Name));
                }
            }
            return Translations;
        }

        private IEnumerable<TranslationDto> LoadKeys(string xmlPath, string lang)
        {
            XElement doc = XElement.Load(xmlPath);
            return doc.Descendants("text").Select(x => new TranslationDto() { Key = x.Attribute("name").Value, Value = x.Value, Language = lang });
        }
        private string GetDirectory()
        {
            UriBuilder uri = new UriBuilder(Directory.GetCurrentDirectory());
            return $"{Uri.UnescapeDataString(uri.Path).Replace("Web.Host", "Core")}/Localization/TACHYON";
        }
    }
    public class TranslationDto
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Language { get; set; }


    }

}
