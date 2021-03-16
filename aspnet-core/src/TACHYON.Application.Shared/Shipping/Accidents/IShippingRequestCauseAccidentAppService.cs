using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Shipping.Accidents.Dto;

namespace TACHYON.Shipping.Accidents
{
  public  interface IShippingRequestCauseAccidentAppService: IApplicationService
    {
        Task<PagedResultDto<ShippingRequestCauseAccidentListDto>> GetAll(GetAllForShippingRequestCauseAccidentFilterInput Input);

        Task CreateOrEdit(CreateOrEditShippingRequestCauseAccidentDto input);
        Task<CreateOrEditShippingRequestCauseAccidentDto> GetForEdit(EntityDto input);


        Task Delete(EntityDto input);
    }
}
