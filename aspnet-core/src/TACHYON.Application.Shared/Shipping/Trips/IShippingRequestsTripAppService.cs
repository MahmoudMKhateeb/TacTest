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
<<<<<<< HEAD
        Task<PagedResultDto<ShippingRequestsTripListDto>>  GetAll(ShippingRequestTripFilterInput Input);
        Task<ShippingRequestsTripForViewDto> GetShippingRequestTripForView(int id);
=======
        Task<ShippingRequestTripDto> GetAll(long ShippingRequestId );
        Task<ShippingRequestTripForViewDto> GetShippingRequestTripForView(int id);
>>>>>>> cf46ed7f3a323941a75a4a54511acb64677eb26c
        Task CreateOrEdit(CreateOrEditShippingRequestTripDto input);
        Task<CreateOrEditShippingRequestTripDto> GetShippingRequestTripForEdit(EntityDto input);

        Task Delete(EntityDto input);
    }
}
