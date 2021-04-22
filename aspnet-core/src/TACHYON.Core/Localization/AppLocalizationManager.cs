using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;

using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Abp.Localization;
using Abp.Domain.Uow;
using Abp.BackgroundJobs;
using System.Xml;

namespace TACHYON.Localization
{
    public class AppLocalizationManager : TACHYONDomainServiceBase
    {

        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IRepository<AppLocalization> _appLocalizationRepository;
        private readonly IRepository<ApplicationLanguage> _languageRepository;

        private string directory;
        public AppLocalizationManager(IBackgroundJobManager backgroundJobManager,
            IRepository<AppLocalization> appLocalizationRepository,
            IRepository<ApplicationLanguage> languageRepository)
        {
            _backgroundJobManager = backgroundJobManager;
            _appLocalizationRepository = appLocalizationRepository;
            _languageRepository = languageRepository;
            directory = GetDirectory();

        }
 

        public  void Restore()
        {
            _backgroundJobManager.Enqueue<AppLocalizationJob, string>(GetDirectory());
        }

        public async Task Generate()
        {
            var Languages = await _appLocalizationRepository
             .GetAllIncluding(x => x.Translations)
              .AsNoTracking()
              .ToListAsync();

             await GenerateXml(Languages,"en", $"{directory}/TACHYON.xml");
            foreach (var lang in _languageRepository.GetAll().Where(x => !x.IsDisabled && x.Name != "en"))
            {
                await GenerateXml(await GetLanguageLocalization(Languages, lang.Name), lang.Name, $"{directory}/TACHYON-{lang.Name}.xml");
            }

        }
        private  Task<List<AppLocalization>> GetLanguageLocalization(List<AppLocalization> languages,string lang)
        {
            List<AppLocalization> Translations = new List<AppLocalization>();
            languages.ForEach( l =>
            {
                var t =  l.Translations.FirstOrDefault(x => x.Language == lang);
                if (t != null) Translations.Add(new AppLocalization() { MasterKey = l.MasterKey, MasterValue = t.Value });
            });
            return Task.FromResult(Translations);
        }
        public Task GenerateXml(List<AppLocalization> languages,string language,string path)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);
            XmlElement localizationDictionary = doc.CreateElement(string.Empty, "localizationDictionary", string.Empty);
            localizationDictionary.SetAttribute("culture", language);
            doc.AppendChild(localizationDictionary);

            XmlElement texts = doc.CreateElement(string.Empty, "texts", string.Empty);
            localizationDictionary.AppendChild(texts);
            foreach (var lang in languages)
            {
                XmlElement text = doc.CreateElement(string.Empty, "text", string.Empty);
                text.SetAttribute("name", lang.MasterKey);
                text.InnerText = lang.MasterValue;
                texts.AppendChild(text);
            }
            doc.Save(path);
            return Task.CompletedTask;
        }
        private string GetDirectory()
        {
            UriBuilder uri = new UriBuilder(Directory.GetCurrentDirectory());
            return $"{Uri.UnescapeDataString(uri.Path).Replace("Web.Host", "Core")}/Localization/TACHYON";
        }
    }
}
