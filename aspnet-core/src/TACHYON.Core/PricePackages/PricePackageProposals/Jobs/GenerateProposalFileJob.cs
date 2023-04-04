using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;

namespace TACHYON.PricePackages.PricePackageProposals.Jobs
{
    public class GenerateProposalFileJob : AsyncBackgroundJob<GenerateProposalFileJobArgument>, ITransientDependency
    {
        private readonly IPricePackageProposalManager _proposalManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IUserEmailer _userEmailer;
        private readonly IRepository<PricePackageProposal> _proposalRepository;

        public GenerateProposalFileJob(
            IPricePackageProposalManager proposalManager,
            IUnitOfWorkManager unitOfWorkManager, 
            IUserEmailer userEmailer,
            IRepository<PricePackageProposal> proposalRepository)
        {
            _proposalManager = proposalManager;
            _unitOfWorkManager = unitOfWorkManager;
            _userEmailer = userEmailer;
            _proposalRepository = proposalRepository;
        }

        protected override async Task ExecuteAsync(GenerateProposalFileJobArgument args)
        {
            using var uow = _unitOfWorkManager.Begin();

            CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant);
            
            var proposal = await _proposalRepository.GetAll().Include(x => x.Shipper)
                .Include(x => x.PricePackages).ThenInclude(x=> x.DestinationCity)
                .Include(x => x.PricePackages).ThenInclude(x=> x.OriginCity)
                .Include(x => x.PricePackages).ThenInclude(x=> x.TrucksTypeFk)
                .Include(x => x.PricePackages).ThenInclude(x=> x.ShippingType)
                .FirstOrDefaultAsync(x => x.Id == args.ProposalId);
            var file = await _proposalManager.GenerateProposalPdfFile(proposal);

            if (args.ProposalReceiverEmailAddress.IsNullOrEmpty()) 
                await _userEmailer.SendPricePackageProposalEmail(proposal.ProposalName, file,
                args.ProposalReceiverEmailAddress);
            
            await uow.CompleteAsync();
        }
    }
}