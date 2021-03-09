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
        Task<ShippingRequestTripDto> GetAll(long ShippingRequestId );
        Task<ShippingRequestTripForViewDto> GetShippingRequestTripForView(int id);
        Task CreateOrEdit(CreateOrEditShippingRequestTripDto input);

        Task Delete(EntityDto input);
    }
}
