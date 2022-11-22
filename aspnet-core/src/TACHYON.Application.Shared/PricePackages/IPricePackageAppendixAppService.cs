using Abp.Application.Services;
using DevExtreme.AspNet.Data.ResponseModel;
using System.Threading.Tasks;
using TACHYON.Common;
using TACHYON.PricePackages.Dto.PricePackageAppendices;

namespace TACHYON.PricePackages
{
    public interface IPricePackageAppendixAppService : IApplicationService
    {
        Task<LoadResult> GetAll(LoadOptionsInput input);

        Task<AppendixForViewDto> GetForView(int id);

        Task<CreateOrEditAppendixDto> GetForEdit(int id);

        Task CreateOrEdit(CreateOrEditAppendixDto input);

        Task Accept(int appendixId);

        Task Reject(int appendixId);
        Task Delete(int id);
    }
}