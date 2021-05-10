using Abp.BackgroundJobs;
using Abp.Dependency;
using TACHYON.Importing;

namespace TACHYON.Localization.Importing
{
    public class ImportTerminologyToExcelJob : BackgroundJob<byte[]>, ITransientDependency
    {
        public IExcelImportManager<TerminologyDto> _excelImportManager;
        public ImportTerminologyToExcelJob(IExcelImportManager<TerminologyDto> excelImportManager)
        {
            _excelImportManager = excelImportManager;
        }
        public override void Execute(byte[] args)
        {
            var Terminologies = _excelImportManager.ImportFromFile(args);
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
