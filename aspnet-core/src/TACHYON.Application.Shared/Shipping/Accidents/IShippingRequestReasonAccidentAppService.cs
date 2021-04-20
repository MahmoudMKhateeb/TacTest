using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.Shipping.Accidents.Dto;

namespace TACHYON.Shipping.Accidents
{
    public  interface IShippingRequestReasonAccidentAppService: IApplicationService
    {
        ListResultDto<ShippingRequestReasonAccidentListDto> GetAll(GetAllForShippingRequestReasonAccidentFilterInput Input);

        Task CreateOrEdit(CreateOrEditShippingRequestReasonAccidentDto input);
        Task<CreateOrEditShippingRequestReasonAccidentDto> GetForEdit(EntityDto input);


        Task Delete(EntityDto input);
    }
}
