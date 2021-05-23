using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.MarketPlaces.Dto;

namespace TACHYON.MarketPlaces
{
    public interface IMarketPlaceAppService:IApplicationService
    {
        Task<PagedResultDto<MarketPlaceListDto>> GetAll(GetAllMarketPlaceInput Input);
        Task CreateOrEdit(CreateOrEditMarketPlaceBidInput Input);
        Task Delete(EntityDto Input);

    }
}
