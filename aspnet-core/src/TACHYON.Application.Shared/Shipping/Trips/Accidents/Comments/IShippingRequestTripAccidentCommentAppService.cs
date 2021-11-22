using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Shipping.Trips.Accidents.Comments.Dto;
using TACHYON.Shipping.Trips.Accidents.Dto;

namespace TACHYON.Shipping.Trips.Accidents
{
    public interface IShippingRequestTripAccidentCommentAppService
    {

        ListResultDto<ShippingRequestTripAccidentCommentListDto> GetAll(GetAllForShippingRequestTripAccidentCommentFilterInput Input);
        Task CreateOrEdit(CreateOrEditShippingRequestTripAccidentCommentDto input);
        Task<CreateOrEditShippingRequestTripAccidentCommentDto> GetForEdit(EntityDto input);

    }
}