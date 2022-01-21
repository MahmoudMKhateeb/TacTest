using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TACHYON.Importing;

namespace TACHYON.Localization.Importing
{
    public class ImportTerminologyToExcelJob : BackgroundJob<byte[]>, ITransientDependency
    {
        private IExcelImportManager<TerminologyDto> _excelImportManager;
        private readonly IRepository<AppLocalization> _appLocalizationRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;


        public ImportTerminologyToExcelJob(IExcelImportManager<TerminologyDto> excelImportManager,
            IRepository<AppLocalization> appLocalizationRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _excelImportManager = excelImportManager;
            _appLocalizationRepository = appLocalizationRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public override void Execute(byte[] args)
        {
            var Terminologies = _excelImportManager.ImportFromFile(args);
            Migration(Terminologies);
        }

        private void Migration(List<TerminologyDto> Terminologies)
        {
            Terminologies.ForEach(t =>
            {
                if (string.IsNullOrEmpty(t.Masterkey)) return;
                using (var uow = _unitOfWorkManager.Begin())
                {
                    var Terminology = _appLocalizationRepository.GetAll().Include(x => x.Translations)
                        .FirstOrDefault(x => x.MasterKey == t.Masterkey);
                    if (Terminology != null)
                    {
                        if (!string.IsNullOrEmpty(t.CorrectEnglish)) t.English = t.CorrectEnglish;
                        if (!string.IsNullOrEmpty(t.CorrectArabic)) t.Arabic = t.CorrectArabic;
                        Terminology.MasterValue = t.English;
                        if (!string.IsNullOrEmpty(t.Arabic))
                        {
                            if (Terminology.Translations.Count > 0)
                            {
                                Terminology.Translations.First().Value = t.Arabic;
                            }
                            else
                            {
                                Terminology.Translations = new Collection<AppLocalizationTranslation>();
                                Terminology.Translations.Add(
                                    new AppLocalizationTranslation() { Value = t.Arabic, Language = "ar-EG" });
                            }
                        }
                    }

                    uow.Complete();
                }
            });
        }
    }

    public class TerminologyDto
    {
        public string Masterkey { get; set; }
        public string English { get; set; }
        public string CorrectEnglish { get; set; }
        public string Arabic { get; set; }
        public string CorrectArabic { get; set; }
    }
}