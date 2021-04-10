using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Common.Dto;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.RejectReasons.Dtos;

namespace TACHYON.Shipping.Trips.RejectReasons
{
    [AbpAuthorize()]
    public class ShippingRequestTripRejectReasonAppService: TACHYONAppServiceBase, IShippingRequestTripRejectReasonAppService
    {
        private readonly IRepository<ShippingRequestTripRejectReason> _shippingRequestTripRejectReasonRepository;

        public ShippingRequestTripRejectReasonAppService(IRepository<ShippingRequestTripRejectReason> shippingRequestTripRejectReasonRepository)
        {
            _shippingRequestTripRejectReasonRepository = shippingRequestTripRejectReasonRepository;
        }
        public ListResultDto<ShippingRequestTripRejectReasonListDto> GetAllRejectReason(FilterInput Input)
        {
            var query = _shippingRequestTripRejectReasonRepository
              .GetAll()
              .AsNoTracking()
              .WhereIf(!string.IsNullOrWhiteSpace(Input.Filter), e => e.DisplayName.Contains(Input.Filter.Trim()))
              .OrderBy(Input.Sorting ?? "id asc");

            return new ListResultDto<ShippingRequestTripRejectReasonListDto>(ObjectMapper.Map<List<ShippingRequestTripRejectReasonListDto>>(query));
        }
        public async Task CreateOrEdit(CreateOrEditShippingRequestTripRejectReasonDto input)
        {
          if (input.Id==0)
            {
                await Create(input);
            }
          else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Reject_Reason_Delete)]

        public async Task Delete(EntityDto input)
        {
          await  _shippingRequestTripRejectReasonRepository.DeleteAsync(input.Id);
        }


        #region Heleper
        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Reject_Reason_Create)]

        private async Task Create(CreateOrEditShippingRequestTripRejectReasonDto input)
        {
            var Reason = ObjectMapper.Map<ShippingRequestTripRejectReason>(input);

            await _shippingRequestTripRejectReasonRepository.InsertAsync(Reason);
        }
        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Reject_Reason_Edit)]

        private async Task Update(CreateOrEditShippingRequestTripRejectReasonDto input)
        {
            var Reason = await _shippingRequestTripRejectReasonRepository.SingleAsync(e => e.Id == input.Id);
            ObjectMapper.Map(input, Reason);

        }
        #endregion
    }
}
