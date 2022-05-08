using Abp;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Users;
using TACHYON.Authorization.Users.Profile;
using TACHYON.Common;
using TACHYON.Dto;
using TACHYON.Extension;
using TACHYON.Features;
using TACHYON.Notifications;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.Accidents;
using TACHYON.Shipping.Accidents.Dto;
using TACHYON.Shipping.Drivers;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Accidents.Comments.Dto;
using TACHYON.Shipping.Trips.Accidents.Dto;

namespace TACHYON.Shipping.Trips.Accidents.Comments

{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrip_Accident_Comments)]
    public class ShippingRequestTripAccidentCommentsAppService : TACHYONAppServiceBase,
        IShippingRequestTripAccidentCommentAppService
    {
        private readonly IRepository<ShippingRequestTripAccidentComment> _ShippingRequestTripAccidentCommentRepository;
        private readonly ProfileAppService _ProfileAppService;

        public ShippingRequestTripAccidentCommentsAppService(
            ProfileAppService ProfileAppService,
            IRepository<ShippingRequestTripAccidentComment> ShippingRequestTripAccidentCommentRepository
        )
        {
            _ShippingRequestTripAccidentCommentRepository = ShippingRequestTripAccidentCommentRepository;
            _ProfileAppService = ProfileAppService;
        }

        public ListResultDto<ShippingRequestTripAccidentCommentListDto> GetAll(
            GetAllForShippingRequestTripAccidentCommentFilterInput Input)
        {
            CheckIfCanAccessService(true, AppFeatures.Carrier, AppFeatures.TachyonDealer, AppFeatures.Shipper);
            DisableTenancyFilters();

            var query = _ShippingRequestTripAccidentCommentRepository
                .GetAll()
                .Include(r => r.AccidentFK)
                .Include(r => r.TenantFK)
                .Where(r => r.AccidentFK.Id == Input.AccidentId)
                .OrderBy(Input.Sorting ?? "id asc").ToList();

            var list = ObjectMapper.Map<List<ShippingRequestTripAccidentCommentListDto>>(query);
            list.ForEach(e =>
            {
                var date64 = _ProfileAppService.GetProfilePictureByUser(e.CreatorUserId).Result.ProfilePicture;
                e.TenantImage = String.IsNullOrEmpty(date64) ? null : "data:image/jpeg;base64," + date64;
            });

            return new ListResultDto<ShippingRequestTripAccidentCommentListDto>(list);
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequest_Accidents_Comments_Edit)]
        public async Task<CreateOrEditShippingRequestTripAccidentCommentDto> GetForEdit(EntityDto input)
        {
            DisableTenancyFilters();
            var query = await _ShippingRequestTripAccidentCommentRepository
                .GetAll()
                .AsNoTracking()
                .Where(x => x.Id == input.Id)
                .FirstOrDefaultAsync();

            return ObjectMapper.Map<CreateOrEditShippingRequestTripAccidentCommentDto>(query);
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequest_Accidents_Comments_Create)]
        public async Task CreateOrEdit(CreateOrEditShippingRequestTripAccidentCommentDto input)
        {
            CheckIfCanAccessService(true, AppFeatures.Carrier, AppFeatures.TachyonDealer, AppFeatures.Shipper);

            DisableTenancyFilters();

            await ValidateComment(input);

            if (input.Id == 0)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequest_Accidents_Comments_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _ShippingRequestTripAccidentCommentRepository.DeleteAsync((int)input.Id);
        }

        #region Heleper

        private async Task Create(CreateOrEditShippingRequestTripAccidentCommentDto input)
        {
            DisableTenancyFilters();

            var Comment = ObjectMapper.Map<ShippingRequestTripAccidentComment>(input);
            if (AbpSession.TenantId != null)
            {
                Comment.TenantId = (int)AbpSession.TenantId;
            }

            await _ShippingRequestTripAccidentCommentRepository.InsertAndGetIdAsync(Comment);
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequest_Accidents_Comments_Edit)]
        private async Task Update(CreateOrEditShippingRequestTripAccidentCommentDto input)
        {
            var Comment = await GetComment(input.Id);
            ObjectMapper.Map(input, Comment);
        }

        private async Task<ShippingRequestTripAccidentComment> GetComment(int Id)
        {
            var Comment = await _ShippingRequestTripAccidentCommentRepository
                .GetAll()
                .Where(x => x.Id == Id)
                .FirstOrDefaultAsync();
            if (Comment == null) throw new UserFriendlyException(L("NoRecoredFound"));

            return Comment;
        }

        private async Task ValidateComment(CreateOrEditShippingRequestTripAccidentCommentDto input)
        {
            if (input.AccidentId == null)
                throw new UserFriendlyException(L("AccidentIdCanNotBeEmpty"));
            if (String.IsNullOrEmpty(input.Comment))
                throw new UserFriendlyException(L("CommentCanNotBeEmpty"));
        }

        #endregion
    }
}