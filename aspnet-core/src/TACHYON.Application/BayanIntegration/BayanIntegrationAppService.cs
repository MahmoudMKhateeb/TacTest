using Abp.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TACHYON.BayanIntegration
{
    public class BayanIntegrationAppService : TACHYONAppServiceBase
    {
        private readonly BayanIntegrationService _bayanIntegrationService;

        public BayanIntegrationAppService(BayanIntegrationService bayanIntegrationService)
        {
            _bayanIntegrationService = bayanIntegrationService;
        }

        //public async Task<Task<string>> Test(int shippingRequestTripId)
        //{
        //    using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
        //    {
        //      //  var r = await _bayanIntegrationService.CreateConsignmentNote(shippingRequestTripId);

             
        //    }

        //}

    }



}
