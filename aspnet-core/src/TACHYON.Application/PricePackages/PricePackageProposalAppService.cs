using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Common;
using TACHYON.PricePackages.Dto.PricePackageProposals;
using TACHYON.PricePackages.PricePackageProposals;

namespace TACHYON.PricePackages
{
    [AbpAuthorize(AppPermissions.Pages_PricePackageProposal)]
    public class PricePackageProposalAppService : TACHYONAppServiceBase, IPricePackageProposalAppService
    {
        private readonly IRepository<PricePackageProposal> _proposalRepository;
        private readonly IPricePackageProposalManager _proposalManager;

        public PricePackageProposalAppService(
            IRepository<PricePackageProposal> proposalRepository,
            IPricePackageProposalManager proposalManager)
        {
            _proposalRepository = proposalRepository;
            _proposalManager = proposalManager;
        }

        public async Task<LoadResult> GetAll(LoadOptionsInput input)
        {
            var proposals = _proposalRepository.GetAll().AsNoTracking()
                .ProjectTo<ProposalListItemDto>(AutoMapperConfigurationProvider);

           return await LoadResultAsync(proposals, input.LoadOptions);
        }

        public async Task<ProposalForViewDto> GetForView(int proposalId)
        {
            var proposal = await _proposalRepository.GetAll().AsNoTracking()
                .SingleAsync(x => x.Id == proposalId);

            return ObjectMapper.Map<ProposalForViewDto>(proposal);
        }

        [AbpAuthorize(AppPermissions.Pages_PricePackageProposal_Update)]
        public async Task<CreateOrEditProposalDto> GetForEdit(int proposalId)
        {
            var proposal = await _proposalRepository.GetAllIncluding(x=> x.TmsPricePackages).AsNoTracking()
                .SingleAsync(x => x.Id == proposalId);

            return ObjectMapper.Map<CreateOrEditProposalDto>(proposal);
        }

        public async Task<int> CreateOrEdit(CreateOrEditProposalDto input)
        {
            if (!input.Id.HasValue)
            { 
                return await Create(input);
            }
            
            return await Update(input);
        }
        
        [AbpAuthorize(AppPermissions.Pages_PricePackageProposal_Create)]
        protected virtual async Task<int> Create(CreateOrEditProposalDto input)
        {
            var createdProposal = ObjectMapper.Map<PricePackageProposal>(input);
            
            return await _proposalManager.CreateProposal(createdProposal,input.TmsPricePackages);
        }
        
        [AbpAuthorize(AppPermissions.Pages_PricePackageProposal_Update)]
        protected virtual async Task<int> Update(CreateOrEditProposalDto input)
        {
            if (!input.Id.HasValue) throw new UserFriendlyException();
            
            var updatedProposal = await _proposalRepository.FirstOrDefaultAsync(input.Id.Value);

            if (updatedProposal.Status == ProposalStatus.Approved)
                throw new UserFriendlyException(L("YouCanNotEditApprovedProposal"));
            
            ObjectMapper.Map(input, updatedProposal);
            
            return updatedProposal.Id;
        }

        [AbpAuthorize(AppPermissions.Pages_PricePackageProposal_Delete)]
        public async Task Delete(int proposalId)
        {
            var isExist = await _proposalRepository.GetAll().AnyAsync(x => x.Id == proposalId);

            if (!isExist) throw new EntityNotFoundException(L("NotFound"));

            await _proposalRepository.DeleteAsync(x => x.Id == proposalId);
        }

        [AbpAuthorize(AppPermissions.Pages_PricePackageProposal_Accept)]
        public async Task ConfirmProposal(int proposalId)
        {
            // todo after confirm proposal, an appendix will generated (Hint: use manager) ==> It's too Important
            await CheckCanChangeStatus(proposalId);
            _proposalRepository.Update(proposalId,x => x.Status = ProposalStatus.Approved);
        }


        [AbpAuthorize(AppPermissions.Pages_PricePackageProposal_Reject)]
        public async Task RejectProposal(int proposalId)
        {
            await CheckCanChangeStatus(proposalId);
            _proposalRepository.Update(proposalId,x => x.Status = ProposalStatus.Rejected);
        }

        private async Task CheckCanChangeStatus(int proposalId)
        {
            var status = await _proposalRepository.GetAll().Where(x=> x.Id == proposalId)
                .Select(x=> x.Status).FirstOrDefaultAsync();

            if (status == default) throw new EntityNotFoundException(L("NotFound"));
            if (status == ProposalStatus.Approved) throw new UserFriendlyException(L("YouCanNotChangeConfirmedProposalStatus"));
        }
    }
}