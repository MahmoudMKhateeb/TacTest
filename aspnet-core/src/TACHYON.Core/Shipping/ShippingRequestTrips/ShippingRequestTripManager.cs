using Abp.Application.Features;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Features;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.Trips.Dto;

namespace TACHYON.Shipping.ShippingRequestTrips
{
    public class ShippingRequestTripManager: TACHYONDomainServiceBase
    {
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IRepository<ShippingRequest,long> _shippingRequestRepository;
        private readonly IFeatureChecker _featureChecker;
        private IAbpSession _AbpSession { get; set; }


        public ShippingRequestTripManager(IRepository<ShippingRequestTrip> shippingRequestTripRepository, IRepository<ShippingRequest, long> shippingRequestRepository, IFeatureChecker featureChecker, IAbpSession abpSession)
        {
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _featureChecker = featureChecker;
            _AbpSession = abpSession;
        }

        public async Task<ShippingRequestTrip> CreateAsync(ShippingRequestTrip trip)
        {
            var existedTrip = await _shippingRequestTripRepository.FirstOrDefaultAsync(x => x.BulkUploadRef == trip.BulkUploadRef);
            if (existedTrip != null)
            {
                throw new UserFriendlyException(L("truck.DuplicatePlateNumber"));
            }

            return await _shippingRequestTripRepository.InsertAsync(trip);
        }

        public void ValidateTripDto(ImportTripDto importTripDto, StringBuilder exceptionMessage)
        {
            DisableTenancyFilters();
            var SR = _shippingRequestRepository.GetAll()
                .WhereIf(_featureChecker.IsEnabled(AppFeatures.TachyonDealer), x=>x.IsTachyonDeal)
                .WhereIf(_featureChecker.IsEnabled(AppFeatures.Shipper), x=>x.TenantId== _AbpSession.TenantId)
                .FirstOrDefault(x=>x.Id== importTripDto.ShippingRequestId);

            //StringBuilder exceptionMessage = new StringBuilder();

            if (importTripDto.EndTripDate != null && importTripDto.StartTripDate?.Date > importTripDto.EndTripDate.Value.Date)
            {
                exceptionMessage.Append("The start date must be or equal to end date." + "; ");
            }

            try
            {
                ValidateTripDates(importTripDto, SR);
            }
            catch (Exception e)
            {
                exceptionMessage.Append(e.Message);
            }

            ValidateDuplicateBulkReferenceFromDB(importTripDto, exceptionMessage);
            //ValidateDuplicatedReference(importTripDtoList, exceptionMessage);

            importTripDto.Exception = exceptionMessage.ToString();
        }

        public string DuplicatedReferenceFromList(List<ImportTripDto> importTripDtoList)
        {
            if (importTripDtoList != null && importTripDtoList.Count > 0)
            {
                foreach (var trip in importTripDtoList)
                {
                    if (importTripDtoList.Count(x => x.BulkUploadReference == trip.BulkUploadReference) > 1)
                    {
                        return trip.BulkUploadReference;
                    }
                }
            }
            return "";
        }

        private void ValidateDuplicateBulkReferenceFromDB(ImportTripDto importTripDto, StringBuilder exceptionMessage)
        {
            var trip = _shippingRequestTripRepository.GetAll()
               .Where(x => x.ShippingRequestId == importTripDto.ShippingRequestId && x.BulkUploadRef == importTripDto.BulkUploadReference).FirstOrDefault();
            if (trip != null)
            {
                exceptionMessage.Append("The Bulk reference is already exists");
                importTripDto.Exception = exceptionMessage.ToString();
            }
        }




        public void ValidateTripDates(ICreateOrEditTripDtoBase input, ShippingRequest request)
        {
            if (
                input.StartTripDate?.Date > request.EndTripDate?.Date ||
                input.StartTripDate?.Date < request.StartTripDate?.Date ||
                (input.EndTripDate != null && input.EndTripDate.Value.Date > request.EndTripDate?.Date) ||
                (input.EndTripDate != null && input.EndTripDate.Value.Date < request.StartTripDate?.Date)
            )
            {
                throw new UserFriendlyException(L("The trip date range must between shipping request range date"));
            }
        }
    }
}
