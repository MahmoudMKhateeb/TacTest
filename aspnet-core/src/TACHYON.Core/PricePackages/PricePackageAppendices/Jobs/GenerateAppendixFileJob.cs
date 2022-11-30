using Abp.BackgroundJobs;
using Abp.Domain.Uow;
using Abp.Net.Mail;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;

namespace TACHYON.PricePackages.PricePackageAppendices.Jobs
{
    public class GenerateAppendixFileJob : AsyncBackgroundJob<GenerateAppendixFileJobArgument>
    {
        private readonly IPricePackageAppendixManager _appendixManager;
        private readonly IUserEmailer _userEmailer;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public GenerateAppendixFileJob(
            IPricePackageAppendixManager appendixManager,
            IUserEmailer userEmailer,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _appendixManager = appendixManager;
            _userEmailer = userEmailer;
            _unitOfWorkManager = unitOfWorkManager;
        }

        protected override async Task ExecuteAsync(GenerateAppendixFileJobArgument args)
        {
            using var uow = _unitOfWorkManager.Begin();

            var appendixFile = await _appendixManager.GenerateAppendixFile(args.AppendixId);

            await _userEmailer.SendPricePackageAppendixEmail(args.FileReceiverEmailAddress, appendixFile);
        }
    }
}