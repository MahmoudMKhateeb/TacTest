using Abp.Domain.Services;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.PricePackages.Dto;
using TACHYON.Shipping.DirectRequests.Dto;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PricePackages.PricePackageOffers
{
    public interface IPricePackageOfferManager : IDomainService
    {
        Task ApplyPricingOnShippingRequest(long pricePackageId, long srId);

        Task<bool> HasDirectRequestByPricePackage(long shippingRequestId);

        Task<long> GetParentOfferId(long shippingRequestId);

        IQueryable<long> GetParentOfferIdAsQueryable(long shippingRequestId);

        Task<long> SendPriceOfferByPricePackage(long pricePackageId, long shippingRequestId);

        Task<PricePackageForPriceCalculationDto> GetPricePackageForPriceCalculation(long pricePackageId,
            long shippingRequestId);

        Task<CreateShippingRequestDirectRequestInput> GetDirectRequestToHandleByPricePackage(long pricePackageId,
            long shippingRequestId);

        Task<ShippingRequestDirectRequestStatus> AcceptPricePackageOffer(long pricePackageOfferId);
    }
}