using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Shipping.Accidents.Dto;
using TACHYON.Shipping.Trips.Accidents.Dto;

namespace TACHYON.Shipping.Trips.Accidents
{
    public interface IShippingRequestTripAccidentAppService
    {

        ListResultDto<ShippingRequestTripAccidentListDto> GetAll(GetAllForShippingRequestTripAccidentFilterInput Input);

        Task CreateOrEdit(CreateOrEditShippingRequestTripAccidentDto input);
        Task<CreateOrEditShippingRequestTripAccidentDto> GetForEdit(EntityDto input);

        Task CreateOrEditResolve(CreateOrEditShippingRequestTripAccidentResolveDto input);

        Task<FileDto> GetFile(int Id);

    }
}
