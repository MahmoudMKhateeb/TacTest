using Abp.Application.Services;
using DevExtreme.AspNet.Data.ResponseModel;
using System.Threading.Tasks;
using TACHYON.Common;
using TACHYON.PricePackages.Dto.PricePackageProposals;

namespace TACHYON.PricePackages
{
    public interface IPricePackageProposalAppService : IApplicationService
    {
        Task<LoadResult> GetAll(LoadOptionsInput input);

        Task<ProposalForViewDto> GetForView(int proposalId);
        
        Task<CreateOrEditProposalDto> GetForEdit(int proposalId);

        Task<int> CreateOrEdit(CreateOrEditProposalDto input);

        Task Delete(int proposalId);
    }
}