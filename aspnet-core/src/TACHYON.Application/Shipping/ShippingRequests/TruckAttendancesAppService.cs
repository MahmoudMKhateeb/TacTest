using Abp.Application.Features;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Features;
using TACHYON.Shipping.Dedicated;
using TACHYON.Shipping.ShippingRequests.Dtos.Dedicated;
using TACHYON.Shipping.ShippingRequests.Dtos.TruckAttendance;

namespace TACHYON.Shipping.ShippingRequests
{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequests)]
    public class TruckAttendancesAppService : TACHYONAppServiceBase, ITruckAttendancesAppService
    {
        private readonly IRepository<DedicatedShippingRequestTruckAttendance, long> _truckAttendanceRepository;
        private readonly IRepository<DedicatedShippingRequestTruck, long> _dedicatedShippingRequestTruckRepository;
        public TruckAttendancesAppService(IRepository<DedicatedShippingRequestTruckAttendance, long> truckAttendanceRepository, IRepository<DedicatedShippingRequestTruck, long> dedicatedShippingRequestTruckRepository)
        {
            _truckAttendanceRepository = truckAttendanceRepository;
            _dedicatedShippingRequestTruckRepository = dedicatedShippingRequestTruckRepository;
        }

        public async Task<LoadResult> GetAll(long dedicatedTruckId, string filter)
        {
            DisableTenancyFilters();
            var query = _truckAttendanceRepository.GetAll()
                .WhereIf(await IsTachyonDealer(), x => true)
                .WhereIf(await IsCarrier(), x => x.DedicatedShippingRequestTruck.ShippingRequest.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(await IsShipper(), x => x.DedicatedShippingRequestTruck.ShippingRequest.TenantId == AbpSession.TenantId)
                .Where(x => x.DedicatedShippingRequestTruckId == dedicatedTruckId)
                .ProjectTo<TruckAttendanceDto>(AutoMapperConfigurationProvider)
                .AsNoTracking();

            return await LoadResultAsync(query, filter);
        }

        [RequiresFeature(AppFeatures.TachyonDealer, AppFeatures.Shipper)]
        public async Task CreateOrEdit(CreateOrEditTruckAttendanceDto input)
        {
            var attendanceTrucks = new List<DedicatedShippingRequestTruckAttendance>();

            for (var dt = input.StartDate.Date; dt <= input.EndDate.Date; dt = dt.AddDays(1))
            {
                attendanceTrucks.Add(new DedicatedShippingRequestTruckAttendance {AttendaceStatus=input.AttendaceStatus , 
                    DedicatedShippingRequestTruckId = input.DedicatedShippingRequestTruckId, AttendanceDate=dt });
            }

            
            if (input.Id == null)
            {
                await Create(attendanceTrucks);
            }
            else
            {
                await Edit(input);
            }
        }

        [RequiresFeature(AppFeatures.TachyonDealer, AppFeatures.Shipper)]
        public async Task<CreateOrEditTruckAttendanceDto> GetTruckAttendanceForEdit(long id)
        {
            var item = await _truckAttendanceRepository.FirstOrDefaultAsync(id);
            return ObjectMapper.Map<CreateOrEditTruckAttendanceDto>(item);
        }

        public async Task Delete(long id)
        {
            var item = await _truckAttendanceRepository.FirstOrDefaultAsync(id);
            await _truckAttendanceRepository.DeleteAsync(item);
        }

        #region Helper

        private async Task Create(List<DedicatedShippingRequestTruckAttendance> input)
        {
            await ValidateTuckAttendance(input);

            var items = ObjectMapper.Map<List<DedicatedShippingRequestTruckAttendance>>(input);
            foreach(var item in items)
            {
                await _truckAttendanceRepository.InsertAsync(item);
            }
        }

        private async Task Edit(CreateOrEditTruckAttendanceDto input)
        {
            var truckAttendanceItem =await _truckAttendanceRepository.FirstOrDefaultAsync(x => x.Id == input.Id);
            ObjectMapper.Map(input, truckAttendanceItem);
        }


        private async Task ValidateTuckAttendance(List<DedicatedShippingRequestTruckAttendance> input)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            var dedicatedTrucks = await _dedicatedShippingRequestTruckRepository.GetAll()
                .Include(x=>x.ShippingRequest)
                .Include(x=>x.DedicatedShippingRequestTruckAttendances)
                .Where(x => x.Id == input.First().DedicatedShippingRequestTruckId)
                .ToListAsync();
            var startRentalDate = dedicatedTrucks.First().ShippingRequest.RentalStartDate;
            var endRentalDate = dedicatedTrucks.First().ShippingRequest.RentalEndDate;

            var outOfRangeItem = input.Where(x => x.AttendanceDate.Date < startRentalDate.Value.Date || 
            x.AttendanceDate.Date > endRentalDate.Value.Date).FirstOrDefault();
            if (outOfRangeItem != null)
            {
                throw new UserFriendlyException(L(String.Format("Attendance {0} DateMustBeInRentalDateRange", outOfRangeItem.AttendanceDate.ToShortDateString())));
            }

            if (dedicatedTrucks.SelectMany(x => x.DedicatedShippingRequestTruckAttendances).Count() == 0) return;
            var exists =dedicatedTrucks.SelectMany(x => x.DedicatedShippingRequestTruckAttendances)
                .Where(x => input.Any(y => y.AttendanceDate.Date == x.AttendanceDate.Date)).FirstOrDefault();
            if (exists !=null)
            {
                throw new UserFriendlyException(L(String.Format("Attendance {0} AlreadyExistsForThisTruck",exists.AttendanceDate.ToShortDateString())));

            };

        }
        #endregion
    }
}
