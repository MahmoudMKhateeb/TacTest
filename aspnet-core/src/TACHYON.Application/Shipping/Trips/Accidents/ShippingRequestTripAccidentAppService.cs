using Abp;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
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
using TACHYON.Common;
using TACHYON.Configuration;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.MultiTenancy;
using TACHYON.Notifications;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.Accidents;
using TACHYON.Shipping.Accidents.Dto;
using TACHYON.Shipping.Drivers;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Accidents.Dto;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.Trucks;

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
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<User,long> _userRepository;
        private readonly IRepository<Truck,long> _truckRepository;
        private readonly ShippingRequestDriverManager _shippingRequestDriverManager;
        private readonly CommonManager _CommonManager;
        private readonly IAppNotifier _appNotifier;
        private readonly UserManager _userManager;
        private readonly IShippingRequestsTripAppService _tripAppService;

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
            IAppNotifier appNotifier, 
            IRepository<Tenant> tenantRepository,
            IRepository<User, long> userRepository,
            IRepository<Truck, long> truckRepository,
            IShippingRequestsTripAppService tripAppService)
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
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
            _truckRepository = truckRepository;
            _tripAppService = tripAppService;
        }
        // [AbpAuthorize(AppPermissions.Pages_ShippingRequest_Accidents)]

        public async Task<PagedResultDto<ShippingRequestTripAccidentListDto>> GetAll(
            GetAllForShippingRequestTripAccidentFilterInput input)
        {
            CheckIfCanAccessService(true, AppFeatures.Carrier, AppFeatures.TachyonDealer, AppFeatures.Shipper);
            DisableTenancyFilters();

            var isCurrentUserDriver = AbpSession.UserId.HasValue &&
                                      await _shippingRequestDriverManager.IsCurrentUserDriver(AbpSession.UserId.Value);

            var accidents = _ShippingRequestTripAccidentRepository
                .GetAll()
                .AsNoTracking()
                .Include(t => t.RoutPointFK)
                .ThenInclude(f => f.FacilityFk)
                .ThenInclude(c => c.CityFk)
                .Include(r => r.ResoneFK)
                .ThenInclude(t => t.Translations)
                .Where(x => x.RoutPointFK.ShippingRequestTripId == input.TripId)
                .WhereIf(input.PointId.HasValue, x => x.PointId == input.PointId)
                .WhereIf(input.IsResolve.HasValue, x => x.IsResolve == input.IsResolve)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier),
                    x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper),
                    x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.TenantId == AbpSession.TenantId)
                .WhereIf(isCurrentUserDriver,
                    x => x.RoutPointFK.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId)
                .OrderBy(input.Sorting ?? "id desc");

            var myQuery = await (from accident in accidents.PageBy(input)
                from resolve in _ResolveRepository.GetAll().Where(x => x.AccidentId == accident.Id).DefaultIfEmpty()
                select new Tuple<ShippingRequestTripAccident, TripAccidentResolveListDto>(accident, new TripAccidentResolveListDto()
                {
                    Id = resolve.Id,
                    IsAppliedResolve = resolve != null && resolve.IsApplied,
                    ResolveType = resolve.ResolveType,
                    ApprovedByCarrier = resolve.ApprovedByCarrier,
                    ApprovedByShipper = resolve.ApprovedByShipper,
                })).ToListAsync();
            var myResult = myQuery.GroupBy(x => x.Item1.Id)
                .Select(x => x.FirstOrDefault(i => !i.Item2.IsAppliedResolve) ?? x.FirstOrDefault())
                .ToList();

            return new PagedResultDto<ShippingRequestTripAccidentListDto>()
            {
                Items = ObjectMapper.Map<List<ShippingRequestTripAccidentListDto>>(myResult),
                TotalCount = await accidents.CountAsync()
            };
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequest_Accidents_Edit)]
        public async Task<CreateOrEditShippingRequestTripAccidentDto> GetForEdit(EntityDto input)
        {
            DisableTenancyFilters();
            var query = await _ShippingRequestTripAccidentRepository
                .GetAll()
                .Include(x => x.RoutPointFK)
                .Include(r => r.ResoneFK)
                .AsNoTracking()
                .Where(x => x.Id == input.Id)
                .WhereIf(IsEnabled(AppFeatures.Carrier),
                    x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(IsEnabled(AppFeatures.Shipper),
                    x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.TenantId == AbpSession.TenantId)
                .WhereIf(IsEnabled(AppFeatures.TachyonDealer),
                    x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.IsTachyonDeal)
                .WhereIf(GetCurrentUser().IsDriver,
                    x => x.RoutPointFK.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId)
                .FirstOrDefaultAsync();

            return ObjectMapper.Map<CreateOrEditShippingRequestTripAccidentDto>(query);
        }

        //[AbpAuthorize(AppPermissions.Pages_ShippingRequest_Accidents_Get)]
        public async Task<ViewShippingRequestTripAccidentDto> Get(EntityDto input)
        {
            DisableTenancyFilters();
            var query = await _ShippingRequestTripAccidentRepository
                .GetAll()
                .Include(x => x.RoutPointFK)
                .Include(r => r.ResoneFK)
                .ThenInclude(t => t.Translations)
                .AsNoTracking()
                .Where(x => x.Id == input.Id)
                .WhereIf(IsEnabled(AppFeatures.Carrier),
                    x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(IsEnabled(AppFeatures.Shipper),
                    x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.TenantId == AbpSession.TenantId)
                .WhereIf(IsEnabled(AppFeatures.TachyonDealer),
                    x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.IsTachyonDeal)
                .WhereIf(GetCurrentUser().IsDriver,
                    x => x.RoutPointFK.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId)
                .FirstOrDefaultAsync();

            if (query.ResoneFK != null)
            {
                var reasone = ObjectMapper.Map<ShippingRequestReasonAccidentListDto>(query.ResoneFK);
                query.OtherReasonName = reasone.Name;
            }

            return ObjectMapper.Map<ViewShippingRequestTripAccidentDto>(query);
        }


        public async Task CreateOrEdit(CreateOrEditShippingRequestTripAccidentDto input)
        {
            CheckIfCanAccessService(true, AppFeatures.Carrier, AppFeatures.TachyonDealer, AppFeatures.Shipper);

            DisableTenancyFilters();
            //if (input.ReasoneId.HasValue && input.ReasoneId.Value == 0) input.ReasoneId = default(int?);

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
            var accident = await _ShippingRequestTripAccidentRepository
                .GetAll()
                .AsNoTracking()
                .Include(t => t.RoutPointFK)
                .ThenInclude(T => T.ShippingRequestTripFk)
                .ThenInclude(r => r.ShippingRequestFk)
                .Where(x => x.Id == input.AccidentId).FirstAsync();

            if (accident.IsResolve)
                throw new UserFriendlyException(L("CanNotCreateOrEditResolveForAlreadyResolvedAccident"));


            if (!input.Id.HasValue)
            {
                await CreateResolve(input, accident);
                return;
            }

            await UpdateResolve(input);

        }
        
        public async Task<List<SelectItemDto>> GetAllDriversByAccidentId(int accidentId)
        {
            DisableTenancyFilters();
            var drivers = (from driver in _userRepository.GetAll()
                join accident in _ShippingRequestTripAccidentRepository.GetAll() on accidentId equals accident.Id
                join request in _ShippingRequestRepository.GetAll() on accident.ShippingRequestId equals request.Id
                where driver.TenantId == request.CarrierTenantId
                      && driver.IsDriver && _ShippingRequestRepository.GetAll().SelectMany(x => x.ShippingRequestTrips).Where(x=> x.Status == ShippingRequestTripStatus.InTransit )
                          .All(x =>  x.AssignedDriverUserId != driver.Id)
                select new SelectItemDto {Id = driver.Id.ToString(), DisplayName = $"{driver.Name} {driver.Surname}"});
            return await drivers.AsNoTracking().ToListAsync();
        }
        
        public async Task<List<SelectItemDto>> GetAllTrucksByAccidentId(int accidentId)
        {
            DisableTenancyFilters();

            var trucks = (from truck in _truckRepository.GetAll()
                join accident in _ShippingRequestTripAccidentRepository.GetAll() on accidentId equals accident.Id
                join request in _ShippingRequestRepository.GetAll() on accident.ShippingRequestId equals request.Id
                where truck.TenantId == request.CarrierTenantId
                select new SelectItemDto {DisplayName = truck.GetDisplayName(), Id = truck.Id.ToString()});

            return await trucks.AsNoTracking().ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequest_Accidents_Resolve_EnforceChange)]
        public async Task EnforceApplyChanges(int tripId)
        {
            DisableTenancyFilters();
            
            var tripAccidentResolves = await  _ResolveRepository.GetAll()
                .Include(x => x.AccidentFk)
                .ThenInclude(x => x.RoutPointFK)
                .Where(x => x.AccidentFk.RoutPointFK.ShippingRequestTripId == tripId && !x.IsApplied)
                .ToListAsync();

            if (tripAccidentResolves.Count < 1) 
                throw new UserFriendlyException(L("ThisTripHasNotAnyTripAccidentResolve"));

            var trip = await _TripRepository.GetAllIncluding(x=> x.ShippingRequestFk)
                .SingleAsync(x=> x.Id == tripId);

            // Usually there is one resolve for every accident, but no more than that
            
            foreach (var resolve in tripAccidentResolves)
                await HandleResolve(resolve,trip);
            
            await CheckTripOrShippingRequestHasAnyAccident(trip.ShippingRequestId,trip.Id,tripAccidentResolves.Select(x=> x.Id).ToArray());
        }

        
        private async Task HandleResolve(ShippingRequestTripAccidentResolve resolve, ShippingRequestTrip trip
            ,bool shipperEnabled = false, bool carrierEnabled = false)
        {
            
            switch (resolve.ResolveType)
            {
                case TripAccidentResolveType.ChangeDriver:
                    await ChangeDriver(trip, resolve);
                    break;
                case TripAccidentResolveType.ChangeTruck:
                    await ChangeTruck(trip, resolve);
                    break;
                case TripAccidentResolveType.ChangeDriverAndTruck:
                    await ChangeDriverAndTruck(trip, resolve);
                    break;
                case TripAccidentResolveType.NoActionNeeded:
                    // there is no action here ... we don't need any update on the trip
                    break;
                case TripAccidentResolveType.CancelTrip:
                    await ResolveWithCancelTrip(trip, resolve,shipperEnabled,carrierEnabled);
                    break;
                default: return;
            }

            resolve.IsApplied = true;
            resolve.AccidentFk.IsResolve = true;

            await _appNotifier.TripAccidentResolved(trip.ShippingRequestFk, trip.WaybillNumber.ToString(), resolve.ResolveType);
        }

         [AbpAuthorize(AppPermissions.Pages_ShippingRequest_Accidents_Resolve_ApproveChange)]
        public async Task ApplyResolveChanges(int resolveId)
        {
            DisableTenancyFilters();
            var shipperEnabled = await IsEnabledAsync(AppFeatures.Shipper);
            var carrierEnabled = await IsEnabledAsync(AppFeatures.Carrier);
            
            var resolve = await  _ResolveRepository.GetAll()
                .Include(x => x.AccidentFk)
                .ThenInclude(x => x.RoutPointFK)
                .SingleAsync(x => !x.IsApplied && x.Id == resolveId);
            
            var trip = await _TripRepository.GetAllIncluding(x=> x.ShippingRequestFk)
                .WhereIf(carrierEnabled,x=> x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(shipperEnabled,x=> x.ShippingRequestFk.TenantId == AbpSession.TenantId)
                .SingleAsync(x=> x.Id == resolve.AccidentFk.RoutPointFK.ShippingRequestTripId);


            
            if (shipperEnabled)
                resolve.ApprovedByShipper = true;
            if (carrierEnabled)
                resolve.ApprovedByCarrier = true;

            if (resolve.ApprovedByCarrier && resolve.ApprovedByShipper)
            {
                await HandleResolve(resolve,trip,shipperEnabled,carrierEnabled);
                await CheckTripOrShippingRequestHasAnyAccident(trip.ShippingRequestId,trip.Id,resolve.AccidentId);
            }

        }
        
        

        private async Task ChangeDriver(ShippingRequestTrip trip, ShippingRequestTripAccidentResolve resolve)
        {
            var input = new AssignDriverAndTruckToShippmentByCarrierInput {Id = trip.Id};
            if (trip.AssignedTruckId.HasValue)
                input.AssignedTruckId = trip.AssignedTruckId.Value;
            
            if (!resolve.DriverId.HasValue)
                throw new UserFriendlyException(L("TripAccidentResolveDoesNotContainDriver"));

            input.AssignedDriverUserId = resolve.DriverId.Value;
            await _tripAppService.AssignDriverAndTruckToShippmentByCarrier(input);
        }
        
        private async Task ChangeTruck(ShippingRequestTrip trip, ShippingRequestTripAccidentResolve resolve)
        {
            var input = new AssignDriverAndTruckToShippmentByCarrierInput {Id = trip.Id};
            if (trip.AssignedDriverUserId.HasValue) // here we only need to change truck and we would save old driver value
                input.AssignedDriverUserId = trip.AssignedDriverUserId.Value;
            
            if (!resolve.TruckId.HasValue)
                throw new UserFriendlyException(L("TripAccidentResolveDoesNotContainTruck"));

            input.AssignedTruckId = resolve.TruckId.Value;
            await _tripAppService.AssignDriverAndTruckToShippmentByCarrier(input);
        }

        private async Task ChangeDriverAndTruck(ShippingRequestTrip trip, ShippingRequestTripAccidentResolve resolve)
        {
            var input = new AssignDriverAndTruckToShippmentByCarrierInput {Id = trip.Id};

            if (!resolve.TruckId.HasValue || !resolve.DriverId.HasValue)
                throw new UserFriendlyException(L("TripAccidentResolveMustContainDriverAndTruck"));

            input.AssignedTruckId = resolve.TruckId.Value;
            input.AssignedDriverUserId = resolve.DriverId.Value;
            await _tripAppService.AssignDriverAndTruckToShippmentByCarrier(input);
        }
        
        private async Task ResolveWithCancelTrip(ShippingRequestTrip trip,ShippingRequestTripAccidentResolve resolve,bool shipperEnabled,bool carrierEnabled)
        {
            
            List<UserIdentifier> userIdentifiers = new List<UserIdentifier>();

            trip.IsApproveCancledByTachyonDealer = true;
            trip.IsForcedCanceledByTachyonDealer = true;

            if (trip.ShippingRequestFk.CarrierTenantId.HasValue)
                userIdentifiers.Add(await GetAdminTenant(trip.ShippingRequestFk.CarrierTenantId.Value));
            if (trip.ShippingRequestFk.CreatorUserId.HasValue)
                userIdentifiers.Add(new UserIdentifier(trip.ShippingRequestFk.TenantId,
                    trip.ShippingRequestFk.CreatorUserId.Value));

            userIdentifiers.Add(await _userManager.GetTachyonDealerUserIdentifierAsync());

            var requestHasAnyOtherAccident = _ShippingRequestRepository.GetAll().Any(x => x.Id != trip.Id && x.HasAccident);
            
            if (!requestHasAnyOtherAccident)
                _ShippingRequestRepository.Update(trip.ShippingRequestId, x => x.HasAccident = false);
            

            trip.Status = ShippingRequestTripStatus.Canceled;

            await _appNotifier.ShippingRequestTripCancelByAccident(userIdentifiers, trip, await GetCurrentUserAsync());
        }
        
        #region Heleper

         [AbpAuthorize(AppPermissions.Pages_ShippingRequest_Accidents_Create)]
        private async Task Create(CreateOrEditShippingRequestTripAccidentDto input, RoutPoint routPoint)
        {
            DisableTenancyFilters();
            //entered from web
            var isCurrentUserDriver = AbpSession.UserId.HasValue && await _shippingRequestDriverManager.IsCurrentUserDriver(AbpSession.UserId.Value);
            if (!isCurrentUserDriver)
            {
                var document = await _CommonManager.UploadDocumentAsBase64(ObjectMapper.Map<DocumentUpload>(input),
                    AbpSession.TenantId);
                ObjectMapper.Map(document, input);
            }

            var accident = ObjectMapper.Map<ShippingRequestTripAccident>(input);
            if (input.lat.HasValue && input.lng.HasValue)
            {
                accident.Location =
                    new NetTopologySuite.Geometries.Point(input.lat.Value, input.lng.Value) { SRID = 4326 };
            }

            var trip = routPoint.ShippingRequestTripFk;
            var request = trip.ShippingRequestFk;
            accident.PointId = routPoint.Id;
            accident.ShippingRequestId = trip.ShippingRequestId;
            var accidentId = await _ShippingRequestTripAccidentRepository.InsertAndGetIdAsync(accident);
            trip.HasAccident = true;
            request.HasAccident = true;
            await _shippingRequestDriverManager.SetRoutStatusTransition(routPoint, RoutePointStatus.Issue);
            await SentNotification(routPoint, accidentId);
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequest_Accidents_Edit)]
        private async Task Update(CreateOrEditShippingRequestTripAccidentDto input)
        {
            var accident = await GetAccident(input.Id);
            //entered from web
            var isCurrentUserDriver = AbpSession.UserId.HasValue &&
                                      await _shippingRequestDriverManager.IsCurrentUserDriver(AbpSession.UserId.Value);
            if (!isCurrentUserDriver)
            {
                var document = await _CommonManager.UploadDocumentAsBase64(ObjectMapper.Map<DocumentUpload>(input),
                    AbpSession.TenantId);
                ObjectMapper.Map(document, input);
            }

            ObjectMapper.Map(input, accident);
        }

        private async Task<ShippingRequestTripAccident> GetAccident(int Id)
        {
            DisableTenancyFilters();
            
            var isCurrentUserDriver = AbpSession.UserId.HasValue &&
                                      await _shippingRequestDriverManager.IsCurrentUserDriver(AbpSession.UserId.Value);
            
            var accident = await _ShippingRequestTripAccidentRepository
                .GetAll()
                .Where(x => x.Id == Id && !x.IsResolve)
                .WhereIf(await IsEnabledAsync(AppFeatures.Carrier),
                    x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .WhereIf( await IsEnabledAsync(AppFeatures.Shipper),
                    x => x.RoutPointFK.ShippingRequestTripFk.ShippingRequestFk.TenantId == AbpSession.TenantId)
                .WhereIf(isCurrentUserDriver,
                    x => x.RoutPointFK.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId)
                .FirstOrDefaultAsync();
            if (accident == null) throw new UserFriendlyException(L("NoRecoredFound"));

            return accident;
        }
        
        

        [AbpAuthorize(AppPermissions.Pages_ShippingRequest_Accidents_Resolve_Create)]
        private async Task CreateResolve(CreateOrEditShippingRequestTripAccidentResolveDto input,
            ShippingRequestTripAccident accident)
        {
            var document =
                await _CommonManager.UploadDocumentAsBase64(ObjectMapper.Map<DocumentUpload>(input),
                    AbpSession.TenantId);
            ObjectMapper.Map(document, input);

            var resolveIssue = ObjectMapper.Map<ShippingRequestTripAccidentResolve>(input);
            await _ResolveRepository.InsertAsync(resolveIssue);
            
            await _ShippingRequestTripAccidentRepository.UpdateAsync(accident);

        }

        private async Task CheckTripOrShippingRequestHasAnyAccident(long srId,int tripId,params int[] accidents)
        {
            var isTripHasAnyAccident = await
                _ShippingRequestTripAccidentRepository.GetAll()
                    .AnyAsync(x =>
                        !x.IsResolve && !accidents.Contains(x.Id) && x.RoutPointFK.ShippingRequestTripId == tripId);

            var isShippingRequestHasAnyAccident = await
                _ShippingRequestTripAccidentRepository.GetAll()
                    .AnyAsync(x =>
                        !x.IsResolve && !accidents.Contains(x.Id) && x.ShippingRequestId == srId);

            if (!isTripHasAnyAccident)
                _TripRepository.Update(tripId,x=> x.HasAccident = false);
            

            if (!isShippingRequestHasAnyAccident)
                _ShippingRequestRepository.Update(srId,x=> x.HasAccident = false);
            
        }
        
        [AbpAuthorize(AppPermissions.Pages_ShippingRequest_Accidents_Resolve_Edit)]
        private async Task UpdateResolve(CreateOrEditShippingRequestTripAccidentResolveDto input)
        {
            if (!input.Id.HasValue) return;
            DisableTenancyFilters();
            var resolveIssue = await _ResolveRepository.FirstOrDefaultAsync(input.Id.Value);

            if (resolveIssue != null)
            {
                var document = await _CommonManager.UploadDocumentAsBase64(ObjectMapper.Map<DocumentUpload>(input),
                    AbpSession.TenantId);
                ObjectMapper.Map(document, input);
                ObjectMapper.Map(input, resolveIssue);
                resolveIssue.ApprovedByCarrier = false;
                resolveIssue.ApprovedByShipper = false;
                return;
            }

            throw new UserFriendlyException(L("AccidentResolveNotFound"));
        }
        
        public async Task<CreateOrEditShippingRequestTripAccidentResolveDto> GetAccidentResolveForEdit(int resolveId)
        {
            var resolve = await _ResolveRepository.GetAll().AsNoTracking()
                .SingleAsync(x => x.Id == resolveId);
            return ObjectMapper.Map<CreateOrEditShippingRequestTripAccidentResolveDto>(resolve);
        }

        /// <summary>
        /// Return active point t////o add or edit accident
        /// </summary>
        /// <param name="TripId">Sent by shipper or carrier when create accident</param>
        /// <returns></returns>
        private async Task<RoutPoint> GetActivePoint(int? TripId)
        {
            RoutPoint ActivePoint = await _RoutPointRepository
                .GetAll()
                    .Include(T => T.ShippingRequestTripFk)
                    .ThenInclude(r => r.ShippingRequestFk)
                    .Where(x => x.IsActive && x.ShippingRequestTripFk.Id == TripId && x.ShippingRequestTripFk.Status == ShippingRequestTripStatus.InTransit)
                    .WhereIf(AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier) && !GetCurrentUser().IsDriver, x => x.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                    .WhereIf(AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Shipper), x => x.ShippingRequestTripFk.ShippingRequestFk.TenantId == AbpSession.TenantId)
                    .WhereIf(!AbpSession.TenantId.HasValue || IsEnabled(AppFeatures.TachyonDealer), x => x.ShippingRequestTripFk.ShippingRequestFk.IsTachyonDeal)
                    .WhereIf(GetCurrentUser().IsDriver, x => x.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId)
                    .FirstOrDefaultAsync();
            if (ActivePoint == null) throw new UserFriendlyException(L("YouCanNotAddAccidentBecauseNoActivePoint"));
            return ActivePoint;
        }

        private async Task SentNotification(RoutPoint routPoint, int accidentId)
        {
            List<UserIdentifier> userIdentifiers = new List<UserIdentifier>();

            var driverId = routPoint.ShippingRequestTripFk.AssignedDriverUserId;
            var shipperId = routPoint.ShippingRequestTripFk.ShippingRequestFk.CreatorUserId;
            var carrierTenantId = routPoint.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId;
            var shipperTenantId = routPoint.ShippingRequestTripFk.ShippingRequestFk.TenantId;


            if (shipperId.HasValue)
                userIdentifiers.Add(new UserIdentifier(shipperTenantId, shipperId.Value));

            if (carrierTenantId.HasValue)
            {
                userIdentifiers.Add(await GetAdminTenant(carrierTenantId.Value));
                if (driverId.HasValue)
                    userIdentifiers.Add(new UserIdentifier(carrierTenantId, driverId.Value));
            }
            
            userIdentifiers.AddRange(await GetTachyonDealerAdminUsers());

            userIdentifiers.Add(await GetHost());

            // Remove Incident Reporter Form Notified users list 
            if (AbpSession.TenantId.HasValue && !await FeatureChecker.IsEnabledAsync(AppFeatures.TachyonDealer))
                userIdentifiers.RemoveAll(x => x.TenantId == AbpSession.TenantId && x.UserId == AbpSession.UserId);


            var data = new Dictionary<string, object>
            {
                ["id"] = routPoint.ShippingRequestTripFk.ShippingRequestId, ["accidentid"] = accidentId
            };
            var waybillNumber = routPoint.ShippingRequestTripFk.WaybillNumber.ToString();
            var referenceNumber = routPoint.ShippingRequestTripFk.ShippingRequestFk.ReferenceNumber;

            await _appNotifier.ShippingRequestAccidentsOccure(userIdentifiers, data,waybillNumber,referenceNumber);
        }


        private async Task<UserIdentifier> GetAdminTenant(int TenantId)
        {
            return new UserIdentifier(TenantId, (await _userManager.GetAdminByTenantIdAsync(TenantId)).Id);
        }

        private async Task<List<UserIdentifier>> GetTachyonDealerAdminUsers()
        {
            var settingValue = await SettingManager.GetSettingValueAsync(AppSettings.Editions.TachyonEditionId);
            var tachyonDealerEditionId = int.Parse(settingValue);

            return await  (from tenant in _tenantRepository.GetAll()
                where tenant.EditionId == tachyonDealerEditionId
                join user in _userRepository.GetAll() on tenant.Id equals user.TenantId
                select new UserIdentifier(tenant.Id, user.Id)).ToListAsync();
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
                    .FirstOrDefaultAsync(x => x.CoreId == input.ReasoneId);

                if (reason.Name.ToLower().Contains(TACHYONConsts.OthersDisplayName.ToLower()) &&
                    input.OtherReasonName.IsNullOrEmpty())
                    throw new UserFriendlyException(L("AccidentReasonConNotBeOtherAndEmptyAtTheSameTime"));
            }
        }

        #endregion
    }
}