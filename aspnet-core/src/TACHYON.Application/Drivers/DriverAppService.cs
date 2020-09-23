using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using TACHYON.Authorization.Users;
using TACHYON.Trucks;

namespace TACHYON.Drivers
{
    public class GetDriverDetailsDto
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string PlateNumber { get; set; }
        public string TrucksType { get; set; }
    }

    [AbpAuthorize]
    public class DriverAppService : TACHYONAppServiceBase
    {

        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Truck, Guid> _truckRepository;

        public DriverAppService(IRepository<User, long> userRepository, IRepository<Truck, Guid> truckRepository)
        {
            _userRepository = userRepository;
            _truckRepository = truckRepository;
        }

        public async Task<GetDriverDetailsDto> GetDriverDetails()
        {
            var query = from driver in _userRepository.GetAll()
                        join truck in _truckRepository.GetAll().Include(x => x.TrucksTypeFk) on driver.Id equals truck.Driver1UserId
                        where driver.Id == AbpSession.UserId.Value
                        select new GetDriverDetailsDto
                        {
                            FullName = driver.FullName,
                            PhoneNumber = driver.PhoneNumber,
                            EmailAddress = driver.EmailAddress,
                            PlateNumber = truck.PlateNumber,
                            TrucksType = truck.TrucksTypeFk.DisplayName
                        };

            return await query.FirstOrDefaultAsync();

        }


    }
}
