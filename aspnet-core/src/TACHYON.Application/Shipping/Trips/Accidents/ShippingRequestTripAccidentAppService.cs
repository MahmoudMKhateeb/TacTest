using Abp;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Users;
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
using TACHYON.Shipping.Trips.Accidents.Dto;

namespace TACHYON.Shipping.Trips.Accidents

{
    [AbpAuthorize()]
    public class ShippingRequestTripAccidentAppService : TACHYONAppServiceBase, IShippingRequestTripAccidentAppService
    {
        private readonly IRepository<ShippingRequestTripAccident> _ShippingRequestTripAccidentRepository;
        private readonly IRepository<RoutPoint, long> _RoutPointRepository;
        private readonly IRepository<ShippingRequestTrip> _TripRepository;
        private readonly IRepository<ShippingRequest, long> _ShippingRequestRepository;
        private readonly IRepository<ShippingRequestTripAccidentResolve> _ResolveRepository;
        private readonly IRepository<ShippingRequestReasonAccidentTranslation> _shippingRequestReasonAccidentRepository;
        private readonly ShippingRequestDriverManager _shippingRequestDriverManager;
        private readonly CommonManager _CommonManager;
        private readonly IAppNotifier _appNotifier;
        private readonly UserManager _userManager;

        public ShippingRequestTripAccidentAppService(
            IRepository<ShippingRequestTripAccident> ShippingRequestTripAccidentRepository,
            IRepository<RoutPoint, long> RoutPointRepository,
            IRepository<ShippingRequestTrip> TripRepository,
            IRepository<ShippingRequestTripAccidentResolve> ResolveRepository,
            IRepository<ShippingRequest, long> ShippingRequestRepository,
            IRepository<ShippingRequestReasonAccidentTranslation> shippingRequestReasonAccidentRepository,
            ShippingRequestDriverManager shippingRequestDriverManager,
            CommonManager CommonManager,
            UserManager userManager,
            IAppNotifier appNotifier)
        {
            _ShippingRequestTripAccidentRepository = ShippingRequestTripAccidentRepository;
            _RoutPointRepository = RoutPointRepository;
            _CommonManager = CommonManager;
            _TripRepository = TripRepository;
            _ResolveRepository = ResolveRepository;
            _ShippingRequestRepository = ShippingRequestRepository;
            _shippingRequestReasonAccidentRepository = shippingRequestReasonAccidentRepository;
            _shippingRequestDriverManager = shippingRequestDriverManager;
            _userManager = userManager;
            _appNotifier = appNotifier;



        }
        // [AbpAuthorize(AppPermissions.Pages_ShippingRequest_Accidents)]

        public ListResultDto<ShippingRequestTripAccidentListDto> GetAll(GetAllForShippingRequestTripAccidentFilterInput Input)
        {
            CheckIfCanAccessService(true, AppFeatures.Carrier, AppFeatures.TachyonDealer, AppFeatures.Shipper);
            DisableTenancyFilters();

            var query = _ShippingRequestTripAccidentRepository
              .GetAll()
              .AsNoTracking()
              .Include(t => t.RoutPointFK)
               .ThenInclude(f => f.FacilityFk)
                .ThenInclude(c => c.CityFk)
              .Include(r => r.ResoneFK)
               .ThenInclude(t => t.Translations)
                      .Where(x => x.RoutPointFK.ShippingRequestTripId == Input.TripId)
                      .WhereIf(Input.PointId.HasValue, x => x.PointId == Input.PointId)
                      .WhereIf(Input.IsResolve.HasValue, x => x.IsResolve == Input.IsResolve)
                      .WhereIf(AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier), x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                      .WhereIf(AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Shipper), x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.TenantId == AbpSession.TenantId)
                      .WhereIf(!AbpSession.TenantId.HasValue || IsEnabled(AppFeatures.TachyonDealer), x => true)
                      .WhereIf(GetCurrentUser().IsDriver, x => x.RoutPointFK.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId)
              .OrderBy(Input.Sorting ?? "id asc").ToList();

            query.ForEach(q =>
          {
              if (q.ResoneFK != null)
              {
                  //var reasone = await _shippingRequestReasonAccidentRepository.FirstOrDefaultAsync(x=>x.Language== CurrentLanguage || x.Language== TACHYONConsts.DefaultLanguage);
                  var reasone = ObjectMapper.Map<ShippingRequestReasonAccidentListDto>(q.ResoneFK);
                  q.Description = reasone.Name;
              }
          });

            return new ListResultDto<ShippingRequestTripAccidentListDto>(ObjectMapper.Map<List<ShippingRequestTripAccidentListDto>>(query));


        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequest_Accidents_Edit)]
        public async Task<CreateOrEditShippingRequestTripAccidentDto> GetForEdit(EntityDto input)
        {
            DisableTenancyFilters();
            var query = await _ShippingRequestTripAccidentRepository
              .GetAll()
              .Include(x => x.RoutPointFK)
              .AsNoTracking()
                  .Where(x => x.Id == input.Id)
                  .WhereIf(IsEnabled(AppFeatures.Carrier), x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                  .WhereIf(IsEnabled(AppFeatures.Shipper), x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.TenantId == AbpSession.TenantId)
                  .WhereIf(IsEnabled(AppFeatures.TachyonDealer), x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.IsTachyonDeal)
                  .WhereIf(GetCurrentUser().IsDriver, x => x.RoutPointFK.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId).FirstOrDefaultAsync();

            return ObjectMapper.Map<CreateOrEditShippingRequestTripAccidentDto>(query);

        }


        public async Task CreateOrEdit(CreateOrEditShippingRequestTripAccidentDto input)
        {
            CheckIfCanAccessService(true, AppFeatures.Carrier, AppFeatures.TachyonDealer, AppFeatures.Shipper);

            DisableTenancyFilters();
            if (input.ReasoneId.HasValue && input.ReasoneId.Value == 0) input.ReasoneId = default(int?);

            await ValidateOtherReason(input);

            if (input.Id == 0)
            {

                var ActivePoint = await GetActivePoint(input.TripId);
                await Create(input, ActivePoint);
            }
            else
            {
                await Update(input);
            }

        }

        public async Task<FileDto> GetFile(int Id)
        {
            DisableTenancyFilters();
            var Accident = await GetAccident(Id);
            return await _CommonManager.GetDocument(ObjectMapper.Map<IHasDocument>(Accident));
        }


        public async Task CreateOrEditResolve(CreateOrEditShippingRequestTripAccidentResolveDto input)
        {
            DisableTenancyFilters();
            var Accident = await _ShippingRequestTripAccidentRepository
  .GetAll()
  .AsNoTracking()
     .Include(t => t.RoutPointFK)
       .ThenInclude(T => T.ShippingRequestTripFk)
         .ThenInclude(r => r.ShippingRequestFk)
      .Where(x => x.Id == input.AccidentId)
      .WhereIf(IsEnabled(AppFeatures.Carrier), x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
      .WhereIf(IsEnabled(AppFeatures.Shipper), x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.TenantId == AbpSession.TenantId)
      .WhereIf(IsEnabled(AppFeatures.TachyonDealer), x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.IsTachyonDeal)
      .WhereIf(GetCurrentUser().IsDriver, x => x.RoutPointFK.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId).FirstOrDefaultAsync();

            if (Accident != null)
            {
                if (input.Id == 0)
                {
                    if (!Accident.IsResolve) await CreateResolve(input, Accident);

                }
                else if (Accident.IsResolve)
                {
                    await UpdateResolve(input);
                }

            }
        }
        #region Heleper 
        // [AbpAuthorize(AppPermissions.Pages_ShippingRequest_Accidents_Create)]
        private async Task Create(CreateOrEditShippingRequestTripAccidentDto input, RoutPoint routPoint)
        {
            DisableTenancyFilters();
            var document = await _CommonManager.UploadDocumentAsBase64(ObjectMapper.Map<DocumentUpload>(input), AbpSession.TenantId);
            ObjectMapper.Map(document, input);

            var Accident = ObjectMapper.Map<ShippingRequestTripAccident>(input);
            if (input.lat.HasValue && input.lng.HasValue)
            {
                Accident.Location = new NetTopologySuite.Geometries.Point(input.lat.Value, input.lng.Value) { SRID = 4326 };
            }
            var Trip = routPoint.ShippingRequestTripFk;
            var Request = Trip.ShippingRequestFk;
            Accident.PointId = routPoint.Id;
            var AccidentId = await _ShippingRequestTripAccidentRepository.InsertAndGetIdAsync(Accident);
            Trip.HasAccident = true;
            Request.HasAccident = true;
            await _shippingRequestDriverManager.SetRoutStatusTransition(routPoint, RoutePointStatus.Issue);
            await SentNotification(routPoint, AccidentId);
        }
        [AbpAuthorize(AppPermissions.Pages_ShippingRequest_Accidents_Edit)]

        private async Task Update(CreateOrEditShippingRequestTripAccidentDto input)
        {
            var Accident = await GetAccident(input.Id);
            var document = await _CommonManager.UploadDocumentAsBase64(ObjectMapper.Map<DocumentUpload>(input), AbpSession.TenantId);
            ObjectMapper.Map(document, input);
            ObjectMapper.Map(input, Accident);

        }

        private async Task<ShippingRequestTripAccident> GetAccident(int Id)
        {
            var Accident = await _ShippingRequestTripAccidentRepository
    .GetAll()
            .Where(x => x.Id == Id && !x.IsResolve)
            .WhereIf(IsEnabled(AppFeatures.Carrier), x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
            .WhereIf(IsEnabled(AppFeatures.Shipper), x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.TenantId == AbpSession.TenantId)
            .WhereIf(GetCurrentUser().IsDriver, x => x.RoutPointFK.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId)
    .FirstOrDefaultAsync();
            if (Accident == null) throw new UserFriendlyException(L("NoRecoredFound"));

            return Accident;
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequest_Accidents_Resolve_Create)]

        private async Task CreateResolve(CreateOrEditShippingRequestTripAccidentResolveDto input, ShippingRequestTripAccident Accident)
        {
            var trip = Accident.RoutPointFK.ShippingRequestTripFk;
            var document = await _CommonManager.UploadDocumentAsBase64(ObjectMapper.Map<DocumentUpload>(input), AbpSession.TenantId);
            ObjectMapper.Map(document, input);

            var ResolveIssue = ObjectMapper.Map<ShippingRequestTripAccidentResolve>(input);
            await _ResolveRepository.InsertAsync(ResolveIssue);
            Accident.IsResolve = true;

            await _ShippingRequestTripAccidentRepository.UpdateAsync(Accident);
            if (!(await
    _ShippingRequestTripAccidentRepository.GetAll()
    .AnyAsync(x => !x.IsResolve && x.Id != Accident.Id && x.RoutPointFK.ShippingRequestTripId == trip.Id)))
            {
                trip.HasAccident = false;
                await _TripRepository.UpdateAsync(trip);
            }

            if (!(await
                _ShippingRequestTripAccidentRepository.GetAll()
                .AnyAsync(x => !x.IsResolve && x.Id != Accident.Id && x.RoutPointFK.ShippingRequestTripFk.ShippingRequestId == trip.ShippingRequestId)))
            {
                var Request = trip.ShippingRequestFk;
                Request.HasAccident = false;
                await _ShippingRequestRepository.UpdateAsync(Request);
            }
        }
        [AbpAuthorize(AppPermissions.Pages_ShippingRequest_Accidents_Resolve_Edit)]
        private async Task UpdateResolve(CreateOrEditShippingRequestTripAccidentResolveDto input)
        {
            var ResolveIssue = await _ResolveRepository
            .GetAll()
                    .Where(x => x.Id == input.Id)
                    .WhereIf(IsEnabled(AppFeatures.Carrier), x => x.AccidentFK.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                    .WhereIf(IsEnabled(AppFeatures.Shipper), x => x.AccidentFK.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.TenantId == AbpSession.TenantId)
                    .WhereIf(IsEnabled(AppFeatures.TachyonDealer), x => x.AccidentFK.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.IsTachyonDeal)
                    .WhereIf(GetCurrentUser().IsDriver, x => x.AccidentFK.RoutPointFK.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId)
            .FirstOrDefaultAsync();

            if (ResolveIssue != null)
            {
                var document = await _CommonManager.UploadDocumentAsBase64(ObjectMapper.Map<DocumentUpload>(input), AbpSession.TenantId);
                ObjectMapper.Map(document, input);
                ObjectMapper.Map(input, ResolveIssue);
            }

        }
        /// <summary>
        /// Return active point to add or edit accident
        /// </summary>
        /// <param name="TripId">Sent by shipper or carrier when create accident</param>
        /// <returns></returns>
        private async Task<RoutPoint> GetActivePoint(int? TripId)
        {
            RoutPoint ActivePoint = await _RoutPointRepository
                .GetAll()
                    .Include(T => T.ShippingRequestTripFk)
                    .ThenInclude(r => r.ShippingRequestFk)
                    .Where(x => x.IsActive && x.ShippingRequestTripFk.Id == TripId && x.ShippingRequestTripFk.Status == ShippingRequestTripStatus.Intransit)
                    .WhereIf(AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier) && !GetCurrentUser().IsDriver, x => x.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                    .WhereIf(AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Shipper), x => x.ShippingRequestTripFk.ShippingRequestFk.TenantId == AbpSession.TenantId)
                    .WhereIf(!AbpSession.TenantId.HasValue || IsEnabled(AppFeatures.TachyonDealer), x => x.ShippingRequestTripFk.ShippingRequestFk.IsTachyonDeal)
                    .WhereIf(GetCurrentUser().IsDriver, x => x.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId)
                    .FirstOrDefaultAsync();
            if (ActivePoint == null) throw new UserFriendlyException(L("YouCanNotAddAccidentBecauseNoActivePoint"));
            return ActivePoint;

        }

        private async Task SentNotification(RoutPoint routPoint, int AccidentId)
        {
            List<UserIdentifier> UserIdentifiers = new List<UserIdentifier>();
            if (GetCurrentUser().IsDriver)
            {

                UserIdentifiers.Add(new UserIdentifier(routPoint.ShippingRequestTripFk.ShippingRequestFk.TenantId, (long)routPoint.ShippingRequestTripFk.ShippingRequestFk.CreatorUserId));
                UserIdentifiers.Add(await GetAdminTenant((int)routPoint.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId));
                UserIdentifiers.Add(await GetHost());

            }
            else if (IsEnabled(AppFeatures.Carrier))
            {
                UserIdentifiers.Add(new UserIdentifier(routPoint.ShippingRequestTripFk.ShippingRequestFk.TenantId, (long)routPoint.ShippingRequestTripFk.ShippingRequestFk.CreatorUserId));
                UserIdentifiers.Add(new UserIdentifier(routPoint.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId, (long)routPoint.ShippingRequestTripFk.AssignedDriverUserId));
                UserIdentifiers.Add(await GetHost());
            }
            else if (IsEnabled(AppFeatures.Shipper))
            {
                UserIdentifiers.Add(await GetAdminTenant((int)routPoint.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId));
                UserIdentifiers.Add(new UserIdentifier(routPoint.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId, (long)routPoint.ShippingRequestTripFk.AssignedDriverUserId));
                UserIdentifiers.Add(await GetHost());
            }
            else
            {
                UserIdentifiers.Add(new UserIdentifier(routPoint.ShippingRequestTripFk.ShippingRequestFk.TenantId, (long)routPoint.ShippingRequestTripFk.ShippingRequestFk.CreatorUserId));
                UserIdentifiers.Add(await GetAdminTenant((int)routPoint.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId));
                UserIdentifiers.Add(new UserIdentifier(routPoint.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId, (long)routPoint.ShippingRequestTripFk.AssignedDriverUserId));
            }
            var data = new Dictionary<string, object>();
            data["id"] = routPoint.ShippingRequestTripFk.ShippingRequestId;
            data["accidentid"] = AccidentId;

            await _appNotifier.ShippingRequestAccidentsOccure(UserIdentifiers, data);
        }


        private async Task<UserIdentifier> GetAdminTenant(int TenantId)
        {
            return new UserIdentifier(TenantId, (await _userManager.GetAdminByTenantIdAsync(TenantId)).Id);
        }



        private async Task<UserIdentifier> GetHost()
        {
            return new UserIdentifier(null, (await _userManager.GetAdminHostAsync()).Id);

        }

        private async Task ValidateOtherReason(CreateOrEditShippingRequestTripAccidentDto input)
        {
            if (input.ReasoneId != null)
            {
                var reason = await _shippingRequestReasonAccidentRepository
                    .FirstOrDefaultAsync(input.ReasoneId.Value);

                if (reason.Name.ToLowerContains(AppConsts.OthersDisplayName) && input.OtherReasonName.IsNullOrEmpty())
                    throw new UserFriendlyException(L("AccidentReasonConNotBeOtherAndEmptyAtTheSameTime"));
            }
        }


        #endregion

    }
}