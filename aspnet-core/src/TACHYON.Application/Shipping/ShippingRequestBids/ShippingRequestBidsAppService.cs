using Abp.Application.Services.Dto;
using Abp.Linq.Extensions;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Shipping.ShippingRequestBids.Dtos;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Exporting;
using Abp.UI;
using Abp.Timing;
using Abp.Authorization;
using TACHYON.Authorization;

namespace TACHYON.Shipping.ShippingRequestBids
{
    public class ShippingRequestBidsAppService: TACHYONAppServiceBase, IShippingRequestBidsAppService
    {
        private readonly IRepository<ShippingRequestBids, long> _ShippingRequestBidsRepository;
        private readonly IRepository<ShippingRequest, long> _ShippingRequestsRepository;

        public ShippingRequestBidsAppService(IRepository<ShippingRequestBids,long> ShippingRequestBidsRepository ,
            IRepository<ShippingRequest,long> ShippingRequestsRepository)
        {
            _ShippingRequestBidsRepository = ShippingRequestBidsRepository;
            _ShippingRequestsRepository = ShippingRequestsRepository;
        }

        public virtual  async Task<PagedResultDto<GetShippingRequestBidsForViewDto>> GetAllBids(GetAllShippingRequestBidsInput input)
        {
            var filterShippingRequestsBids = _ShippingRequestBidsRepository.GetAll()
                  .Include(x => x.ShippingRequestFk)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false)
                .WhereIf(input.MinPrice != null, e => e.price >= input.MinPrice)
                .WhereIf(input.MaxPrice != null, e => e.price <= input.MaxPrice)
                .WhereIf(input.ShipperId != null, e => e.ShippingRequestFk.CreatorUserId == input.ShipperId && e.IsCancled==false)
                .WhereIf(input.CarrierId != null, e => e.CreatorUserId == input.CarrierId);

            var pagedAndFilteredShippingRequestsBids = filterShippingRequestsBids
                //.OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var shippingRequestBids = from o in pagedAndFilteredShippingRequestsBids
                                      select new GetShippingRequestBidsForViewDto()
                                      {
                                          ShippingRequestBid = new ShippingRequestBidsDto
                                          {
                                              Id=o.Id,
                                              ShippingRequestId = o.ShippingRequestId,
                                              IsAccepted = o.IsAccepted,
                                              IsCancled = o.IsCancled,
                                              price = o.price
                                          },
                                      };

            var totalCount =await filterShippingRequestsBids.CountAsync();

            return new PagedResultDto<GetShippingRequestBidsForViewDto>(totalCount, shippingRequestBids.ToList()) ;


        }

        public async Task StopShippingRequestBid(StopShippingRequestBidInput input)
        {
            var Bid =await _ShippingRequestsRepository.FirstOrDefaultAsync(input.ShippingRequestId);
            if (Bid.IsClosedBid == true)
            {
                throw new UserFriendlyException(L("The Bid is already Stopped"));
            }
            else
            {
                Bid.IsClosedBid = true;
               // Bid.ClosedBidDate = Clock.Now;
            }
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestBids)]
        public void CreateOrEditShippingRequestBid(CreatOrEditShippingRequestBidDto input)
        {
            if (input.Id == null)
                Create(input);
            else
                Edit(input);
        }

        public void Create(CreatOrEditShippingRequestBidDto input)
        {

        }

        public void Edit(CreatOrEditShippingRequestBidDto input)
        {

        }
    }
}
