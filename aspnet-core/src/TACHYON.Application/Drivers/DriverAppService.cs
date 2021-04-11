using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization.Users.Profile;
using TACHYON.Mobile;
using TACHYON.Mobile.Dtos;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Drivers
{
    public class DriverDetailDto
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string PlateNumber { get; set; }
        public string Picture { get; set; }
        public string TrucksType { get; set; }
        public string LangaugeCode { get; set; }
        public string LangaugeName { get; set; }
        public string LangaugeNative { get; set; }

    }

    [AbpAuthorize]
    public class DriverAppService : TACHYONAppServiceBase
    {

        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly ProfileImageServiceFactory _profileImageServiceFactory;
        private readonly UserDeviceTokenManager _userDeviceTokenManager;

        public DriverAppService(
            IRepository<ShippingRequestTrip> shippingRequestTripRepository,
            ProfileImageServiceFactory profileImageServiceFactory,
            UserDeviceTokenManager userDeviceTokenManager)
        {
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _profileImageServiceFactory = profileImageServiceFactory;
            _userDeviceTokenManager = userDeviceTokenManager;

        }

        public async Task<DriverDetailDto> GetDriverDetails()
        {
            var user = await GetCurrentUserAsync();
            DriverDetailDto driverDetail = new DriverDetailDto() 
            {
                FullName= user.FullName,
                PhoneNumber=user.PhoneNumber,
                EmailAddress=user.EmailAddress,
                LangaugeCode = CultureInfo.CurrentCulture.Name,
                LangaugeName= CultureInfo.CurrentCulture.DisplayName,
                LangaugeNative = CultureInfo.CurrentCulture.NativeName
            };

            using (var profileImageService = await _profileImageServiceFactory.Get(AbpSession.ToUserIdentifier()))
            {
                var profilePictureContent = await profileImageService.Object.GetProfilePictureContentForUser(
                    AbpSession.ToUserIdentifier()
                );
                driverDetail.Picture = profilePictureContent;

            }

            var trip = await _shippingRequestTripRepository
                .GetAll()
                .AsTracking()
                .Include(t => t.AssignedTruckFk)
                    .ThenInclude(t => t.TrucksTypeFk)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(d => d.AssignedDriverUserId == user.Id && d.DriverStatus == Shipping.Trips.ShippingRequestTripDriverStatus.Accepted && d.AssignedTruckId != null);
                if (trip !=null)
            {
                driverDetail.PlateNumber = trip.AssignedTruckFk.PlateNumber;
                driverDetail.TrucksType = trip.AssignedTruckFk.TrucksTypeFk.DisplayName ;

            }

            return driverDetail;

        }
        public async Task RefreshDeviceToken(UserDeviceTokenDto Input)
        {
            Input.UserId = AbpSession.UserId.Value;
            await _userDeviceTokenManager.CreateOrEdit(Input);
        }

    }
}
