using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Trailers.Dtos;
using TACHYON.Trailers.Exporting;
using TACHYON.Trailers.PayloadMaxWeights;
using TACHYON.Trailers.TrailerStatuses;
using TACHYON.Trailers.TrailerTypes;
using TACHYON.Trucks;

namespace TACHYON.Trailers
{
    [AbpAuthorize(AppPermissions.Pages_Trailers)]
    [RequiresFeature(AppFeatures.Carrier)]

    public class TrailersAppService : TACHYONAppServiceBase, ITrailersAppService
    {
        private readonly IRepository<Trailer, long> _trailerRepository;
        private readonly ITrailersExcelExporter _trailersExcelExporter;
        private readonly IRepository<TrailerStatus, int> _lookup_trailerStatusRepository;
        private readonly IRepository<TrailerType, int> _lookup_trailerTypeRepository;
        private readonly IRepository<PayloadMaxWeight, int> _lookup_payloadMaxWeightRepository;
        private readonly IRepository<Truck, long> _lookup_truckRepository;


        public TrailersAppService(IRepository<Trailer, long> trailerRepository, ITrailersExcelExporter trailersExcelExporter, IRepository<TrailerStatus, int> lookup_trailerStatusRepository, IRepository<TrailerType, int> lookup_trailerTypeRepository, IRepository<PayloadMaxWeight, int> lookup_payloadMaxWeightRepository, IRepository<Truck, long> lookup_truckRepository)
        {
            _trailerRepository = trailerRepository;
            _trailersExcelExporter = trailersExcelExporter;
            _lookup_trailerStatusRepository = lookup_trailerStatusRepository;
            _lookup_trailerTypeRepository = lookup_trailerTypeRepository;
            _lookup_payloadMaxWeightRepository = lookup_payloadMaxWeightRepository;
            _lookup_truckRepository = lookup_truckRepository;

        }

        public async Task<PagedResultDto<GetTrailerForViewDto>> GetAll(GetAllTrailersInput input)
        {

            var filteredTrailers = _trailerRepository.GetAll()
                        .Include(e => e.TrailerStatusFk)
                        .Include(e => e.TrailerTypeFk)
                        .Include(e => e.PayloadMaxWeightFk)
                        .Include(e => e.HookedTruckFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.TrailerCode.Contains(input.Filter) || e.PlateNumber.Contains(input.Filter) || e.Model.Contains(input.Filter) || e.Year.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TrailerCodeFilter), e => e.TrailerCode == input.TrailerCodeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.PlateNumberFilter), e => e.PlateNumber == input.PlateNumberFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ModelFilter), e => e.Model == input.ModelFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.YearFilter), e => e.Year == input.YearFilter)
                        .WhereIf(input.MinWidthFilter != null, e => e.Width >= input.MinWidthFilter)
                        .WhereIf(input.MaxWidthFilter != null, e => e.Width <= input.MaxWidthFilter)
                        .WhereIf(input.MinHeightFilter != null, e => e.Height >= input.MinHeightFilter)
                        .WhereIf(input.MaxHeightFilter != null, e => e.Height <= input.MaxHeightFilter)
                        .WhereIf(input.MinLengthFilter != null, e => e.Length >= input.MinLengthFilter)
                        .WhereIf(input.MaxLengthFilter != null, e => e.Length <= input.MaxLengthFilter)
                        .WhereIf(input.IsLiftgateFilter > -1, e => (input.IsLiftgateFilter == 1 && e.IsLiftgate) || (input.IsLiftgateFilter == 0 && !e.IsLiftgate))
                        .WhereIf(input.IsReeferFilter > -1, e => (input.IsReeferFilter == 1 && e.IsReefer) || (input.IsReeferFilter == 0 && !e.IsReefer))
                        .WhereIf(input.IsVentedFilter > -1, e => (input.IsVentedFilter == 1 && e.IsVented) || (input.IsVentedFilter == 0 && !e.IsVented))
                        .WhereIf(input.IsRollDoorFilter > -1, e => (input.IsRollDoorFilter == 1 && e.IsRollDoor) || (input.IsRollDoorFilter == 0 && !e.IsRollDoor))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TrailerStatusDisplayNameFilter), e => e.TrailerStatusFk != null && e.TrailerStatusFk.DisplayName == input.TrailerStatusDisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TrailerTypeDisplayNameFilter), e => e.TrailerTypeFk != null && e.TrailerTypeFk.DisplayName == input.TrailerTypeDisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.PayloadMaxWeightDisplayNameFilter), e => e.PayloadMaxWeightFk != null && e.PayloadMaxWeightFk.DisplayName == input.PayloadMaxWeightDisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TruckPlateNumberFilter), e => e.HookedTruckFk != null && e.HookedTruckFk.PlateNumber == input.TruckPlateNumberFilter);

            var pagedAndFilteredTrailers = filteredTrailers
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var trailers = from o in pagedAndFilteredTrailers
                           join o1 in _lookup_trailerStatusRepository.GetAll() on o.TrailerStatusId equals o1.Id into j1
                           from s1 in j1.DefaultIfEmpty()

                           join o2 in _lookup_trailerTypeRepository.GetAll() on o.TrailerTypeId equals o2.Id into j2
                           from s2 in j2.DefaultIfEmpty()

                           join o3 in _lookup_payloadMaxWeightRepository.GetAll() on o.PayloadMaxWeightId equals o3.Id into j3
                           from s3 in j3.DefaultIfEmpty()

                           join o4 in _lookup_truckRepository.GetAll() on o.HookedTruckId equals o4.Id into j4
                           from s4 in j4.DefaultIfEmpty()

                           select new GetTrailerForViewDto()
                           {
                               Trailer = new TrailerDto
                               {
                                   TrailerCode = o.TrailerCode,
                                   PlateNumber = o.PlateNumber,
                                   Model = o.Model,
                                   Year = o.Year,
                                   Width = o.Width,
                                   Height = o.Height,
                                   Length = o.Length,
                                   IsLiftgate = o.IsLiftgate,
                                   IsReefer = o.IsReefer,
                                   IsVented = o.IsVented,
                                   IsRollDoor = o.IsRollDoor,
                                   Id = o.Id
                               },
                               TrailerStatusDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString(),
                               TrailerTypeDisplayName = s2 == null || s2.DisplayName == null ? "" : s2.DisplayName.ToString(),
                               PayloadMaxWeightDisplayName = s3 == null || s3.DisplayName == null ? "" : s3.DisplayName.ToString(),
                               TruckPlateNumber = s4 == null || s4.PlateNumber == null ? "" : s4.PlateNumber.ToString()
                           };

            var totalCount = await filteredTrailers.CountAsync();

            return new PagedResultDto<GetTrailerForViewDto>(
                totalCount,
                await trailers.ToListAsync()
            );
        }

        public async Task<GetTrailerForViewDto> GetTrailerForView(long id)
        {
            var trailer = await _trailerRepository.GetAsync(id);

            var output = new GetTrailerForViewDto { Trailer = ObjectMapper.Map<TrailerDto>(trailer) };

            if (output.Trailer.TrailerStatusId != null)
            {
                var _lookupTrailerStatus = await _lookup_trailerStatusRepository.FirstOrDefaultAsync((int)output.Trailer.TrailerStatusId);
                output.TrailerStatusDisplayName = _lookupTrailerStatus?.DisplayName?.ToString();
            }

            if (output.Trailer.TrailerTypeId != null)
            {
                var _lookupTrailerType = await _lookup_trailerTypeRepository.FirstOrDefaultAsync((int)output.Trailer.TrailerTypeId);
                output.TrailerTypeDisplayName = _lookupTrailerType?.DisplayName?.ToString();
            }

            if (output.Trailer.PayloadMaxWeightId != null)
            {
                var _lookupPayloadMaxWeight = await _lookup_payloadMaxWeightRepository.FirstOrDefaultAsync((int)output.Trailer.PayloadMaxWeightId);
                output.PayloadMaxWeightDisplayName = _lookupPayloadMaxWeight?.DisplayName?.ToString();
            }

            if (output.Trailer.HookedTruckId != null)
            {
                var _lookupTruck = await _lookup_truckRepository.FirstOrDefaultAsync((long)output.Trailer.HookedTruckId);
                output.TruckPlateNumber = _lookupTruck?.PlateNumber?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Trailers_Edit)]
        public async Task<GetTrailerForEditOutput> GetTrailerForEdit(EntityDto<long> input)
        {
            var trailer = await _trailerRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTrailerForEditOutput { Trailer = ObjectMapper.Map<CreateOrEditTrailerDto>(trailer) };

            if (output.Trailer.TrailerStatusId != null)
            {
                var _lookupTrailerStatus = await _lookup_trailerStatusRepository.FirstOrDefaultAsync((int)output.Trailer.TrailerStatusId);
                output.TrailerStatusDisplayName = _lookupTrailerStatus?.DisplayName?.ToString();
            }

            if (output.Trailer.TrailerTypeId != null)
            {
                var _lookupTrailerType = await _lookup_trailerTypeRepository.FirstOrDefaultAsync((int)output.Trailer.TrailerTypeId);
                output.TrailerTypeDisplayName = _lookupTrailerType?.DisplayName?.ToString();
            }

            if (output.Trailer.PayloadMaxWeightId != null)
            {
                var _lookupPayloadMaxWeight = await _lookup_payloadMaxWeightRepository.FirstOrDefaultAsync((int)output.Trailer.PayloadMaxWeightId);
                output.PayloadMaxWeightDisplayName = _lookupPayloadMaxWeight?.DisplayName?.ToString();
            }

            if (output.Trailer.HookedTruckId != null)
            {
                var _lookupTruck = await _lookup_truckRepository.FirstOrDefaultAsync((long)output.Trailer.HookedTruckId);
                output.TruckPlateNumber = _lookupTruck?.PlateNumber?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTrailerDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Trailers_Create)]
        protected virtual async Task Create(CreateOrEditTrailerDto input)
        {
            var trailer = ObjectMapper.Map<Trailer>(input);


            if (AbpSession.TenantId != null)
            {
                trailer.TenantId = (int)AbpSession.TenantId;
            }


            await _trailerRepository.InsertAsync(trailer);
        }

        [AbpAuthorize(AppPermissions.Pages_Trailers_Edit)]
        protected virtual async Task Update(CreateOrEditTrailerDto input)
        {
            var trailer = await _trailerRepository.FirstOrDefaultAsync((long)input.Id);
            ObjectMapper.Map(input, trailer);
        }

        [AbpAuthorize(AppPermissions.Pages_Trailers_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _trailerRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetTrailersToExcel(GetAllTrailersForExcelInput input)
        {

            var filteredTrailers = _trailerRepository.GetAll()
                        .Include(e => e.TrailerStatusFk)
                        .Include(e => e.TrailerTypeFk)
                        .Include(e => e.PayloadMaxWeightFk)
                        .Include(e => e.HookedTruckFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.TrailerCode.Contains(input.Filter) || e.PlateNumber.Contains(input.Filter) || e.Model.Contains(input.Filter) || e.Year.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TrailerCodeFilter), e => e.TrailerCode == input.TrailerCodeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.PlateNumberFilter), e => e.PlateNumber == input.PlateNumberFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ModelFilter), e => e.Model == input.ModelFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.YearFilter), e => e.Year == input.YearFilter)
                        .WhereIf(input.MinWidthFilter != null, e => e.Width >= input.MinWidthFilter)
                        .WhereIf(input.MaxWidthFilter != null, e => e.Width <= input.MaxWidthFilter)
                        .WhereIf(input.MinHeightFilter != null, e => e.Height >= input.MinHeightFilter)
                        .WhereIf(input.MaxHeightFilter != null, e => e.Height <= input.MaxHeightFilter)
                        .WhereIf(input.MinLengthFilter != null, e => e.Length >= input.MinLengthFilter)
                        .WhereIf(input.MaxLengthFilter != null, e => e.Length <= input.MaxLengthFilter)
                        .WhereIf(input.IsLiftgateFilter > -1, e => (input.IsLiftgateFilter == 1 && e.IsLiftgate) || (input.IsLiftgateFilter == 0 && !e.IsLiftgate))
                        .WhereIf(input.IsReeferFilter > -1, e => (input.IsReeferFilter == 1 && e.IsReefer) || (input.IsReeferFilter == 0 && !e.IsReefer))
                        .WhereIf(input.IsVentedFilter > -1, e => (input.IsVentedFilter == 1 && e.IsVented) || (input.IsVentedFilter == 0 && !e.IsVented))
                        .WhereIf(input.IsRollDoorFilter > -1, e => (input.IsRollDoorFilter == 1 && e.IsRollDoor) || (input.IsRollDoorFilter == 0 && !e.IsRollDoor))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TrailerStatusDisplayNameFilter), e => e.TrailerStatusFk != null && e.TrailerStatusFk.DisplayName == input.TrailerStatusDisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TrailerTypeDisplayNameFilter), e => e.TrailerTypeFk != null && e.TrailerTypeFk.DisplayName == input.TrailerTypeDisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.PayloadMaxWeightDisplayNameFilter), e => e.PayloadMaxWeightFk != null && e.PayloadMaxWeightFk.DisplayName == input.PayloadMaxWeightDisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TruckPlateNumberFilter), e => e.HookedTruckFk != null && e.HookedTruckFk.PlateNumber == input.TruckPlateNumberFilter);

            var query = (from o in filteredTrailers
                         join o1 in _lookup_trailerStatusRepository.GetAll() on o.TrailerStatusId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_trailerTypeRepository.GetAll() on o.TrailerTypeId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         join o3 in _lookup_payloadMaxWeightRepository.GetAll() on o.PayloadMaxWeightId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         join o4 in _lookup_truckRepository.GetAll() on o.HookedTruckId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()

                         select new GetTrailerForViewDto()
                         {
                             Trailer = new TrailerDto
                             {
                                 TrailerCode = o.TrailerCode,
                                 PlateNumber = o.PlateNumber,
                                 Model = o.Model,
                                 Year = o.Year,
                                 Width = o.Width,
                                 Height = o.Height,
                                 Length = o.Length,
                                 IsLiftgate = o.IsLiftgate,
                                 IsReefer = o.IsReefer,
                                 IsVented = o.IsVented,
                                 IsRollDoor = o.IsRollDoor,
                                 Id = o.Id
                             },
                             TrailerStatusDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString(),
                             TrailerTypeDisplayName = s2 == null || s2.DisplayName == null ? "" : s2.DisplayName.ToString(),
                             PayloadMaxWeightDisplayName = s3 == null || s3.DisplayName == null ? "" : s3.DisplayName.ToString(),
                             TruckPlateNumber = s4 == null || s4.PlateNumber == null ? "" : s4.PlateNumber.ToString()
                         });


            var trailerListDtos = await query.ToListAsync();

            return _trailersExcelExporter.ExportToFile(trailerListDtos);
        }


        [AbpAuthorize(AppPermissions.Pages_Trailers)]
        public async Task<List<TrailerTrailerStatusLookupTableDto>> GetAllTrailerStatusForTableDropdown()
        {
            return await _lookup_trailerStatusRepository.GetAll()
                .Select(trailerStatus => new TrailerTrailerStatusLookupTableDto
                {
                    Id = trailerStatus.Id,
                    DisplayName = trailerStatus == null || trailerStatus.DisplayName == null ? "" : trailerStatus.DisplayName.ToString()
                }).ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_Trailers)]
        public async Task<List<TrailerTrailerTypeLookupTableDto>> GetAllTrailerTypeForTableDropdown()
        {
            return await _lookup_trailerTypeRepository.GetAll()
                .Select(trailerType => new TrailerTrailerTypeLookupTableDto
                {
                    Id = trailerType.Id,
                    DisplayName = trailerType == null || trailerType.DisplayName == null ? "" : trailerType.DisplayName.ToString()
                }).ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_Trailers)]
        public async Task<List<TrailerPayloadMaxWeightLookupTableDto>> GetAllPayloadMaxWeightForTableDropdown()
        {
            return await _lookup_payloadMaxWeightRepository.GetAll()
                .Select(payloadMaxWeight => new TrailerPayloadMaxWeightLookupTableDto
                {
                    Id = payloadMaxWeight.Id,
                    DisplayName = payloadMaxWeight == null || payloadMaxWeight.DisplayName == null ? "" : payloadMaxWeight.DisplayName.ToString()
                }).ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_Trailers)]
        public async Task<List<TrailerTruckLookupTableDto>> GetAllTruckForTableDropdown()
        {
            return await _lookup_truckRepository.GetAll()
                .Select(truck => new TrailerTruckLookupTableDto
                {
                    Id = truck.Id.ToString(),
                    DisplayName = truck == null || truck.PlateNumber == null ? "" : truck.PlateNumber.ToString()
                }).ToListAsync();
        }

    }
}