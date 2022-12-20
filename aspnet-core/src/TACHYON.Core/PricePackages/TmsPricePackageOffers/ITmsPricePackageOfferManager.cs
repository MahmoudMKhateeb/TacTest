using Abp.Domain.Services;
using System.Threading.Tasks;

namespace TACHYON.PricePackages.TmsPricePackageOffers
{
    public interface ITmsPricePackageOfferManager : IDomainService
    {
        Task ApplyPricingOnShippingRequest(int pricePackageId, long srId,bool isTmsPricePackage);

        Task<bool> HasDirectRequestByPricePackage(long shippingRequestId);

        Task<long> GetParentOfferId(long shippingRequestId);
    }
}