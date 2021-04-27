using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.Common.Dto;
using TACHYON.Dto;
using TACHYON.Shipping.Accidents.Dto;

namespace TACHYON.Shipping.Accidents
{
    public  interface IShippingRequestReasonAccidentAppService: IApplicationService
    {
        ListResultDto<ShippingRequestReasonAccidentListDto> GetAll(FilterInput Input);

        Task CreateOrEdit(CreateOrEditShippingRequestReasonAccidentDto input);
        Task<CreateOrEditShippingRequestReasonAccidentDto> GetForEdit(EntityDto input);

        FileDto Exports(FilterInput Input);

        Task Delete(EntityDto input);
    }
}
