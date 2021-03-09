using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Shipping.Trips.Dto;

namespace TACHYON.Shipping.Trips
{
  public  interface IShippingRequestsTripAppService: IApplicationService
    {
        Task<ShippingRequestsTripListDto> GetAll(long RequestId );
        Task<ShippingRequestsTripForViewDto> GetForView(long id);
        Task CreateOrEdit(ShippingRequestsTripCreateOrEditDto input);

        Task Delete(EntityDto input);
    }
}
