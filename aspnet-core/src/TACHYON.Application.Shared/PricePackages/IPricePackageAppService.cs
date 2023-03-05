using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DevExtreme.AspNet.Data.ResponseModel;
using System.Threading.Tasks;
using TACHYON.Common;
using TACHYON.PricePackages.Dto;

namespace TACHYON.PricePackages
{
    public interface IPricePackageAppService : IApplicationService
    {
        Task<LoadResult> GetAll(LoadOptionsInput input);

        Task<PricePackageForViewDto> GetForView(long pricePackageId);
        
        Task<CreateOrEditPricePackageDto> GetForEdit(long pricePackageId);

        Task CreateOrEdit(CreateOrEditPricePackageDto input);

        Task Delete(long pricePackageId);

        Task<PagedResultDto<PricePackageForViewDto>> GetMatchingPricePackages(GetMatchingPricePackagesInput input);

        Task<PricePackageForPricingDto> GetForPricing(long pricePackageId);
    }
}