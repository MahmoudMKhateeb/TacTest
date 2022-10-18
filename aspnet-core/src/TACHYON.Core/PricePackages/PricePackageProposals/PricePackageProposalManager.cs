using Abp.Domain.Repositories;
using Abp.Threading.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using NUglify.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.PricePackages.Dto.PricePackageProposals;

namespace TACHYON.PricePackages.PricePackageProposals
{
     //ToDo: add main methods to the base type
    public class PricePackageProposalManager : TACHYONDomainServiceBase, IPricePackageProposalManager
    {
        private readonly IRepository<PricePackageProposal> _proposalRepository;
        private readonly IRepository<TmsPricePackage> _tmsPricePackageRepository;

        public PricePackageProposalManager(
            IRepository<PricePackageProposal> proposalRepository,
            IRepository<TmsPricePackage> tmsPricePackageRepository)
        {
            _proposalRepository = proposalRepository;
            _tmsPricePackageRepository = tmsPricePackageRepository;
        }

        public async Task<int> CreateProposal(PricePackageProposal createdProposal,List<int> tmsPricePackages)
        {
            
            // check name if already used before

            bool isNameDuplicated = await _proposalRepository.GetAll()
                .AnyAsync(x => x.ProposalName.Equals(createdProposal.ProposalName));

            if (isNameDuplicated) throw new UserFriendlyException(L("ProposalNameAlreadyUsedBefore"));
            // check items if them used in any another proposal

            bool anyItemUsedInAnotherProposal = await _tmsPricePackageRepository.GetAll()
                .AnyAsync(x => tmsPricePackages.Any(i => i == x.Id) && x.ProposalId.HasValue);

            if (anyItemUsedInAnotherProposal)
                throw new UserFriendlyException(L("YouCanNotAddItemUsedInAnotherProposal"));
            
            // check items if them for another shipper 
            bool anyItemNotForSelectedShipper = await _tmsPricePackageRepository.GetAll()
                .AnyAsync(x => tmsPricePackages.Any(i => i == x.Id) && x.ShipperId != createdProposal.ShipperId);
            
            if (anyItemNotForSelectedShipper) 
                throw new UserFriendlyException(L("YouMustSelectItemForSelectedShipper"));
            
            var createdProposalId = await _proposalRepository.InsertAndGetIdAsync(createdProposal);
            
            tmsPricePackages.ForEach(tmsPricePackageId=>
            {
                _tmsPricePackageRepository.Update(tmsPricePackageId, x => x.ProposalId = createdProposalId);
            });
            return createdProposalId;
        }
    }
}