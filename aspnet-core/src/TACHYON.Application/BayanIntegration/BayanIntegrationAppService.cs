using Abp.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Integration.BayanIntegration.V2;
using TACHYON.Integration.BayanIntegration.V3;

namespace TACHYON.BayanIntegration
{
    public class BayanIntegrationAppService : TACHYONAppServiceBase
    {
        private readonly BayanIntegrationManagerV3 _bayanIntegrationService;

        public BayanIntegrationAppService(BayanIntegrationManagerV3 bayanIntegrationService)
        {
            _bayanIntegrationService = bayanIntegrationService;
        }

        //public async Task<string> CreateTrip(int shippingRequestTripId)
        //{
        //    using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
        //    {
        //        return await _bayanIntegrationService.CreateTrip(shippingRequestTripId);


        //    }

        //}
    }



}