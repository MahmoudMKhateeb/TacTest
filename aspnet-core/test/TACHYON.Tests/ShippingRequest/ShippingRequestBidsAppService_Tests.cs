using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Shouldly;
using TACHYON.Shipping.ShippingRequestBids;
using TACHYON.Shipping.ShippingRequestBids.Dtos;
using System.Threading.Tasks;
using Abp.UI;

namespace TACHYON.Tests.ShippingRequest
{
    public class ShippingRequestBidsAppService_Tests: AppTestBase
    {
        private readonly IShippingRequestBidsAppService _shippingRequestBidsAppService;
        public ShippingRequestBidsAppService_Tests()
        {
            LoginAsTenant("shipper", "admin");
            _shippingRequestBidsAppService = Resolve<IShippingRequestBidsAppService>();
        }
        [Fact]
        public async Task CreateShouldNotWorkAsync()
        {

            var id= _shippingRequestBidsAppService.GetShipperbidsRequestDetailsForView(
                new Abp.Application.Services.Dto.PagedAndSortedResultRequestDto {
                    SkipCount=5,Sorting=null,MaxResultCount=10 })
                .Id;
            var newBid=await _shippingRequestBidsAppService.CreateOrEditShippingRequestBid(
                new CreatOrEditShippingRequestBidDto {
                    price = 50, ShippingRequestId = id 
                });
            var againBid=await _shippingRequestBidsAppService.CreateOrEditShippingRequestBid(
                new CreatOrEditShippingRequestBidDto { 
                    price = 50, ShippingRequestId = id });

            Assert.Throws<UserFriendlyException>(()=>againBid);
        }
    }
}
