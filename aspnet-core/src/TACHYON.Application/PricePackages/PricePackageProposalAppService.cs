﻿using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Common;
using TACHYON.Dto;
using TACHYON.PricePackages.Dto.PricePackageProposals;
using TACHYON.PricePackages.PricePackageProposals;
using TACHYON.PricePackages.TmsPricePackages;

namespace TACHYON.PricePackages
{
    [AbpAuthorize(AppPermissions.Pages_PricePackageProposal)]
    public class PricePackageProposalAppService : TACHYONAppServiceBase, IPricePackageProposalAppService
    {
        private readonly IRepository<PricePackageProposal> _proposalRepository;
        private readonly IRepository<TmsPricePackage> _tmsPricePackageRepository;
        private readonly IPricePackageProposalManager _proposalManager;

        public PricePackageProposalAppService(
            IRepository<PricePackageProposal> proposalRepository,
            IPricePackageProposalManager proposalManager,
            IRepository<TmsPricePackage> tmsPricePackageRepository)
        {
            _proposalRepository = proposalRepository;
            _proposalManager = proposalManager;
            _tmsPricePackageRepository = tmsPricePackageRepository;
        }

        public async Task<LoadResult> GetAll(LoadOptionsInput input)
        {
            DisableTenancyFilters();
            var isTmsOrHost = !AbpSession.TenantId.HasValue || await IsTachyonDealer();
            
            var proposals = _proposalRepository.GetAll().AsNoTracking()
                .WhereIf(!isTmsOrHost,x=> x.ShipperId == AbpSession.TenantId)
                .ProjectTo<ProposalListItemDto>(AutoMapperConfigurationProvider);

           return await LoadResultAsync(proposals, input.LoadOptions);
        }

        public async Task<ProposalForViewDto> GetForView(int proposalId)
        {
            var proposal = await _proposalRepository.GetAllIncluding(x=> x.Shipper)
                .AsNoTracking().SingleAsync(x => x.Id == proposalId);

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
            
            return await _proposalManager.CreateProposal(createdProposal,input.TmsPricePackages,input.EmailAddress);
        }
        
        [AbpAuthorize(AppPermissions.Pages_PricePackageProposal_Update)]
        protected virtual async Task<int> Update(CreateOrEditProposalDto input)
        {
            if (!input.Id.HasValue) throw new UserFriendlyException();
            
            var updatedProposal = await _proposalRepository.GetAllIncluding(x=> x.TmsPricePackages)
                .FirstOrDefaultAsync(x=> x.Id == input.Id.Value);

            if (updatedProposal.Status == ProposalStatus.Approved)
                throw new UserFriendlyException(L("YouCanNotEditApprovedProposal"));
            
            ObjectMapper.Map(input, updatedProposal);
            var addedPackages = input.TmsPricePackages.Where(x => updatedProposal.TmsPricePackages.All(i => i.Id != x));
            var deletedPackages = updatedProposal.TmsPricePackages
                .Where(x => input.TmsPricePackages.All(i => i != x.Id));
            foreach (int addedPackageId in addedPackages)
                _tmsPricePackageRepository.Update(addedPackageId, x => x.ProposalId = updatedProposal.Id);

            foreach (var package in deletedPackages)
                package.ProposalId = null;

            await _proposalManager.UpdateProposal(updatedProposal, input.EmailAddress);
            
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

        [AbpAuthorize(AppPermissions.Pages_PricePackageProposal_Create,AppPermissions.Pages_PricePackageProposal_Update)]
        public async Task<ProposalAutoFillDataDto> GetProposalAutoFillDetails(int proposalId)
        {
            var proposalDetails = await _proposalRepository.GetAll().AsNoTracking()
                .Where(x => x.Id == proposalId)
                .ProjectTo<ProposalAutoFillDataDto>(AutoMapperConfigurationProvider).SingleAsync();

            return proposalDetails;
        }
        public async Task<ListResultDto<SelectItemDto>> GetAllProposalsForDropdown(int shipperId,int? appendixId)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();

            var proposalsList = await _proposalRepository.GetAll().AsNoTracking()
                .Where(x => x.ShipperId == shipperId && x.Status == ProposalStatus.Approved)
                .WhereIf(appendixId.HasValue,x=> !x.AppendixId.HasValue || x.AppendixId == appendixId )
                .WhereIf(!appendixId.HasValue,x=> !x.AppendixId.HasValue)
                .Select(x => new SelectItemDto() { DisplayName = x.ProposalName, Id = x.Id.ToString() })
                .ToListAsync();

            return new ListResultDto<SelectItemDto>(proposalsList);
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