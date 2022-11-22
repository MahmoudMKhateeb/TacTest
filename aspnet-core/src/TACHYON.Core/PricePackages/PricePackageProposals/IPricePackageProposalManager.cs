using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Storage;

namespace TACHYON.PricePackages.PricePackageProposals
{
    public interface IPricePackageProposalManager : IDomainService
    {
        Task<int> CreateProposal(PricePackageProposal createdProposal,List<int> tmsPricePackages,string emailAddress);

        Task UpdateProposal(PricePackageProposal updatedProposal, string emailAddress);
        
        Task<BinaryObject> GenerateProposalPdfFile(PricePackageProposal proposal);
    }
}