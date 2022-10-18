using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DevExtreme.AspNet.Data.ResponseModel;
using System.Threading.Tasks;
using TACHYON.Common;
using TACHYON.PricePackages.Dto.TmsPricePackages;

namespace TACHYON.PricePackages
{
    public interface ITmsPricePackageAppService : IApplicationService
    {
        Task<LoadResult> GetAll(LoadOptionsInput input);

        Task<TmsPricePackageForViewDto> GetForView(int pricePackageId);
        
        Task<CreateOrEditTmsPricePackageDto> GetForEdit(int pricePackageId);

        Task CreateOrEdit(CreateOrEditTmsPricePackageDto input);

        Task Delete(int pricePackageId);
    }
}