using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TACHYON.Importing;

namespace TACHYON.Localization.Importing
{
    public class ImportMobileTerminologyToExcelJob : BackgroundJob<byte[]>, ITransientDependency
    {
        private IExcelImportManager<TerminologyMobileDto> _excelImportManager;
        private readonly IRepository<AppLocalization> _appLocalizationRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;


        public ImportMobileTerminologyToExcelJob(IExcelImportManager<TerminologyMobileDto> excelImportManager,
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
        private void Migration(List<TerminologyMobileDto> Terminologies)
        {
            Terminologies.ForEach(t =>
            {
                if (string.IsNullOrEmpty(t.key)) return;
                using (var uow = _unitOfWorkManager.Begin())
                {
                    var Terminology = _appLocalizationRepository.GetAll().Include(x => x.Translations).FirstOrDefault(x => x.MasterKey.ToLower() == t.key.ToLower());
                    if (Terminology != null)
                    {
                        Terminology.AppVersion = TACHYON.Terminologies.TerminologyAppVersion.Mobile;
                    }
                    uow.Complete();
                }


            });
        }
    }
    public class TerminologyMobileDto
    {
        public string key { get; set; }
    }
}
