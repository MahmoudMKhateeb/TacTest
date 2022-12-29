﻿using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Abp.EntityHistory;
using Abp.Linq.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using TACHYON.AddressBook.Dtos;
using TACHYON.Authorization;
using TACHYON.Routs.Dtos;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Tracking;

namespace TACHYON.Routs.RoutPoints
{
    public class RoutPointsAppService : TACHYONAppServiceBase, IRoutPointAppService
    {
        private IRepository<RoutPoint, long> _routPointsRepository;
        private readonly IEntitySnapshotManager _snapshotManager;

        public RoutPointsAppService(IRepository<RoutPoint, long> routPointsRepository,
            IEntitySnapshotManager snapshotManager)
        {
            _routPointsRepository = routPointsRepository;
            _snapshotManager = snapshotManager;
        }

        public async Task<PagedResultDto<GetRoutPointForViewOutput>> GetAll(GetAllRoutPointInput input)
        {
            var filteredRoutPoints = _routPointsRepository.GetAll()
                .Include(x => x.FacilityFk)
                .ThenInclude(x => x.CityFk)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), x => x.DisplayName == input.Filter)
                .WhereIf(input.PickingType != null, x => x.PickingType == input.PickingType);


            var PagedAndFilteredRoutPoints = filteredRoutPoints
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var routPoints = PagedAndFilteredRoutPoints.Select(x => new GetRoutPointForViewOutput
            {
                RoutPointDto = ObjectMapper.Map<RoutPointDto>(x),
                facilityDto = new GetFacilityForViewOutput
                {
                    CityDisplayName = x.FacilityFk.CityFk.DisplayName,
                    FacilityName = x.FacilityFk.Name,
                    Facility = ObjectMapper.Map<FacilityDto>(x.FacilityFk)
                }
            });

            var totalCount = await routPoints.CountAsync();
            return new PagedResultDto<GetRoutPointForViewOutput>(totalCount, await routPoints.ToListAsync());
        }

        public async Task CreateOrEditRoutPoint(CreateOrEditRoutPointDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Edit(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_RoutPoints_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _routPointsRepository.DeleteAsync(input.Id);
        }

        #region Waybills

        public IEnumerable<GetDropsDetailsForMasterWaybillOutput> GetDropsDetailsForMasterWaybill(
            long shippingRequestTripId)
        {
            var points = _routPointsRepository.GetAll()
                .Where(e => e.ShippingRequestTripId == shippingRequestTripId)
                .Where(e => e.PickingType == PickingType.Dropoff);

            var query = points.Select(x => new
            {
                Id = x.WaybillNumber,
                ReceiverName = x.ReceiverFk != null ? x.ReceiverFk.FullName : x.ReceiverFullName
            });
            return query.ToList().Select(x => new GetDropsDetailsForMasterWaybillOutput
            {
                Code = x.Id.ToString(), ReceiverDisplayName = x.ReceiverName
            });
            var ll = "";
        }

        #endregion

        [AbpAuthorize(AppPermissions.Pages_RoutPoints_Create)]
        private async Task Create(CreateOrEditRoutPointDto input)
        {
            // Note: This Service is not used 
            // if you want use it you must know that (delivery note & home delivery not handled here)
            var routPoint = ObjectMapper.Map<RoutPoint>(input);
            routPoint.WorkFlowVersion = routPoint.PickingType == PickingType.Pickup
                ? WorkflowVersionConst.PickupPointWorkflowVersion
                : WorkflowVersionConst.DropOffWithoutDeliveryNotePointWorkflowVersion;
            await _routPointsRepository.InsertAsync(routPoint);
        }

        [AbpAuthorize(AppPermissions.Pages_RoutPoints_Edit)]
        private async Task Edit(CreateOrEditRoutPointDto input)
        {
            var routPoint = await _routPointsRepository.FirstOrDefaultAsync((long)input.Id);
            ObjectMapper.Map(input, routPoint);
        }

        public async Task<dynamic> GetRoutPointSnapshot(long routePointId,
            int tripId,
            DateTime dateTime)
        {
            var pointSnapshot = await _snapshotManager.GetSnapshotAsync<RoutPoint, long>(routePointId, dateTime);
            var tripSnapshot = await _snapshotManager.GetSnapshotAsync<ShippingRequestTrip, int>(tripId, dateTime);
            return new List<object>() { pointSnapshot, tripSnapshot };
        }
    }
}