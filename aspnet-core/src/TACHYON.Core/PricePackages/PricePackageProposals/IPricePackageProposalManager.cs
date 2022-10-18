using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TACHYON.PricePackages.PricePackageProposals
{
    public interface IPricePackageProposalManager : IDomainService
    {
        Task<int> CreateProposal(PricePackageProposal createdProposal,List<int> tmsPricePackages);
    }
}